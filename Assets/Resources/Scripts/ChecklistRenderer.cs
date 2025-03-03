#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// TODO da se nadje gde treba kad se ucita checklist da se pokazu/sakriju page buttons (SetPageButtons i RemovePageButtons)
// TODO low priority kako izgledaju buttons za non normal checklists i sta rade
// TODO resets da se vidi sta radi

public class ChecklistRenderer : MonoBehaviour
{
    public int characterCount = 50;
    public int splitNameLimit = 20;
    public int checksPerPage = 8;

    public GameObject checkPrefab;
    public GameObject conditionalCheckPrefab;
    public GameObject pageNumberPrefab;
    public GameObject pageButtons;
    public GameObject checklistStatus;
    public GameObject title;
    public GameObject titleContainer;
    public GameObject checkContainer;
    public GameObject menuManager;
    public HorizontalLayoutGroup horizontalLayoutGroup;

    private int _checklistIndex = 0;
    private List<Checklist> _checklists = new();
    private string _menuName = "";
    private Checklist? _currentChecklist;
    private int _currentPage = 1;
    private int _pagesCount = 1;
    private int _highestPage = -1;
    private List<GameObject> _checkObjects = new();

    public void SetChecklists(List<Checklist> checklists, string menuName)
    {
        _checklists = checklists;
        _menuName = menuName;
    }

    public bool LoadNextNormalChecklist()
    {
        var checklist = _checklists.FirstOrDefault(checklist => !checklist.IsDone());
        if (checklist == null)
        {
            return false;
        }

        LoadChecklist(checklist);
        return true;
    }

    public void LoadChecklistByIndex(int index = -1)
    {
        if (index != -1)
            _checklistIndex = index;
        if (_checklistIndex >= _checklists.Count || _checklistIndex < 0)
        {
            _checklistIndex = 0;
        }

        LoadChecklist(_checklists[_checklistIndex]);
        _checklistIndex++;
    }

    private void LoadChecklist(Checklist checklist)
    {
        UnloadCurrentChecklist();
        _currentChecklist = checklist;
        _pagesCount = 1;
        _currentChecklist.SetListeners();
        // znam da ovo ne treba ovde ali ne znam kako drugacije TODO da se ispravi
        for (int i = 0; i < titleContainer.transform.childCount - 1; i++)
        {
            Destroy(titleContainer.transform.GetChild(i).gameObject);
        }
        titleContainer.transform.GetChild(0).GetComponentInChildren<TMP_Text>().text = checklist.name;
        checklist.OnCheckChecked += OnCheckboxCheck;
        var conditionalChecks = checklist.checks.Where(check => check.IsConditional).ToList();



        var indentCount = new Dictionary<string, int>();
        foreach (var check in conditionalChecks)
        {
            var indent = 0;
            if (indentCount.TryGetValue(check.name, out var value))
                indent = value;
            if(check.conditionalChecksNo != null)
                foreach (var child in check.conditionalChecksNo)
                {
                    indentCount[child] = indent + 1;
                }
            if(check.conditionalChecksYes != null)
                foreach (var child in check.conditionalChecksYes)
                {
                    indentCount[child] = indent + 1;
                }
        }
        
        

        for (var i = 0; i < checklist.checks.Count; i++)
        {
            checklist.checks[i].Index = i;

            if (checklist.checks[i].IsConditional)
            {
                // instantiate conditional check
                var conditionalCheckObject = Instantiate(conditionalCheckPrefab, checkContainer.transform);
                var conditionalCheckRenderer = conditionalCheckObject.GetComponent<ConditionalCheckRenderer>();
                conditionalCheckRenderer.check = checklist.checks[i];
                _checkObjects.Add(conditionalCheckObject);
            }
            else
            {
                // instantiate normal check TODO ne treba se prikazuju 6 checka nego 7 ali da smanjuje ako ne mogu da stanu na ekran
                // TODO da se scaluju checkovi sa kolicinom teksta
                
                if (checklist.checks[i].name == "PageBreak")
                {
                    _pagesCount++;
                }
                var checkObject = Instantiate(checkPrefab, checkContainer.transform);
                var checkRenderer = checkObject.GetComponent<CheckRenderer>();
                checkRenderer.check = checklist.checks[i];
                checkRenderer.SetTextSize(characterCount, splitNameLimit);
                var indent = indentCount.ContainsKey(checklist.checks[i].name)? indentCount[checklist.checks[i].name] : 0;
                checkRenderer.indentation = indent;
                checkObject.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta =
                    new Vector2(checkObject.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x, (checklist.checks[i].expectedValue.Length / characterCount + 1) * 60);
                checkObject.GetComponent<RectTransform>().sizeDelta = new Vector2(checkObject.GetComponent<RectTransform>().sizeDelta.x, (checklist.checks[i].expectedValue.Length / characterCount + 1) * 60);
                _checkObjects.Add(checkObject);
            }
        }

        _currentPage = 1;
        if (_pagesCount > 1)
        {
            SetPageButtons();
        }
        else
        {
            RemovePageButtons();
        }

        LoadPage();
        if (_currentChecklist.IsDone())
            ChecklistDone();
        else
            ChecklistNotDone();
    }

    public void UnloadCurrentChecklist()
    {
        if (_currentChecklist == null) return;
        checklistStatus.SetActive(false);
        _currentChecklist.OnCheckChecked -= OnCheckboxCheck;
        foreach (var checkObject in _checkObjects)
        {
            Destroy(checkObject);
        }

        if (!_currentChecklist.IsDone()) _currentChecklist.Reset();

        _checkObjects.Clear();
        _currentChecklist = null;
    }

    public void OnCheckboxCheck(int index, bool value)
    {
        if (_currentChecklist?.IsDone() == true)
            ChecklistDone();
        if (_pagesCount == 1) return;
        var flag = true;
        for (var i = (_currentPage - 1) * checksPerPage;
             i < _currentPage * checksPerPage && i < _currentChecklist?.checks.Count;
             i++)
        {
            if (!_currentChecklist.checks[i].Checked)
            {
                SetPageNotComplete();
                flag = false;
                break;
            }
        }
        if (flag) SetPageComplete();
    }

    public void OverrideCheck()
    {
        _currentChecklist?.OverrideCheck();
        if (_currentChecklist?.IsDone() == true)
        {
            ChecklistDone();
        }
    }

    public void OverrideChecklist()
    {
        _currentChecklist?.OverrideChecklist();
        ChecklistDone();
    }

    public void ResetChecklist()
    {
        _currentChecklist?.Reset();
        ChecklistNotDone();
    }

    private void ChecklistDone()
    {
        menuManager.GetComponent<MenuManager>().ShowButtons(new[]
        {
            "NORMAL", "CHKLOVRD", "CHKLRESET"
        });

        checklistStatus.SetActive(true);
        var isOverridden = _currentChecklist!.IsOverridden;
        checklistStatus.GetComponentInChildren<TMP_Text>().text =
            isOverridden ? "CHECKLIST OVERRIDDEN" : "CHECKLIST COMPLETE";
        checklistStatus.GetComponent<Image>().color = isOverridden ? new Color(.23f, .64f, .76f, 1) : Color.green;

        // TODO vidi jel treba HideButtons da se napravi
    }

    private void ChecklistNotDone()
    {
        menuManager.GetComponent<MenuManager>().ShowButtons(new[]
        {
            "NORMAL", "ITEMOVRD", "CHKLOVRD", "CHKLRESET"
        });
        checklistStatus.SetActive(false);
    }

    public void SetPageButtons()
    {
        RemovePageButtons();
        RectTransform pageButtonsRect = pageButtons.GetComponent<RectTransform>();

        pageButtons.SetActive(true);
        pageButtons.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(HandlePreviousPage);
        pageButtons.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(HandleNextPage);

        for (int i = 0; i < _pagesCount; i++)
        {
            var pageButton = Instantiate(pageNumberPrefab, pageButtons.transform);
            int iCopy = i;
            pageButton.transform.SetSiblingIndex(1 + i);
           
            pageButton.transform.GetChild(0).GetComponent<TMP_Text>().text = (i + 1).ToString();
            pageButton.GetComponent<Button>().onClick.AddListener((() => { HandlePageButtonPress(iCopy + 1); }));
            
            RectTransform pageButtonRect = pageButton.GetComponent<RectTransform>();

            pageButtonRect.localScale = Vector3.one;
            pageButtonRect.sizeDelta =
                new Vector2(pageButtonRect.sizeDelta.x, (pageButtonsRect.sizeDelta.y - 200) / _pagesCount);
            _highestPage = i + 1;
        }
    }

    public void RemovePageButtons()
    {
        int count = pageButtons.transform.childCount;
        if (count > 2)
        {
            for (int i = count - 2; i > 0; i--)
            {
                Destroy(pageButtons.transform.GetChild(i).gameObject);
            }
        }

        pageButtons.SetActive(false);
    }

    public void HandlePreviousPage()
    {
        if (_currentPage < 2)
            return;
        _currentPage--;
        LoadPage();
    }

    public void HandleNextPage()
    {
        if (_currentPage >= _highestPage)
            return;
        _currentPage++;
        LoadPage();
    }

    public void HandlePageButtonPress(int pageNumber)
    {
        _currentPage = pageNumber;
        LoadPage();
    }

    private void LoadPage()
    {
        int nPageBreak = 1;
        for (var i = 0; i < _checkObjects.Count; i++)
        {
            //Debug.Log(_checkObjects[i].GetComponentInChildren<TMP_Text>().text);
            if (_currentChecklist.checks[i].name.Contains("PageBreak"))
            {
                nPageBreak++;
                _checkObjects[i].SetActive(false);
            }
            else if (nPageBreak == _currentPage)
            {
                _checkObjects[i].SetActive(true);
            }
            else
            {
                _checkObjects[i].SetActive(false);
            }
            
            //_checkObjects[i].SetActive(i >= (_currentPage - 1) * checksPerPage && i < _currentPage * checksPerPage);

        }
    }

    public void SetPageComplete()
    {
        StringBuilder pageNumber = new StringBuilder();
        pageNumber.Append("<color=green>").Append(_currentPage).Append("</color>");

        pageButtons.transform.GetChild(_currentPage).GetChild(0).GetComponent<TMP_Text>().text = pageNumber.ToString();
    }

    public void SetPageNotComplete()
    {
        StringBuilder pageNumber = new StringBuilder();
        pageNumber.Append("<color=white>").Append(_currentPage).Append("</color>");

        pageButtons.transform.GetChild(_currentPage).GetChild(0).GetComponent<TMP_Text>().text = pageNumber.ToString();
    }
}