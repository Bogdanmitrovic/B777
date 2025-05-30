#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ChecklistRenderer : MonoBehaviour
{
    public int characterCount = 50;
    public int splitNameLimit = 20;
    // public int checksPerPage = 8;

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
        for (int i = titleContainer.transform.childCount - 1; i > 0; i--)
        {
            Destroy(titleContainer.transform.GetChild(i).gameObject);
        }

        titleContainer.transform.GetChild(0).GetComponentInChildren<TMP_Text>().text = checklist.name;
        titleContainer.transform.GetChild(0).GetComponentInChildren<Image>().enabled = false;
        checklist.OnCheckChecked += OnCheckboxCheck;
        var conditionalChecks = checklist.checks.Where(check => check.IsConditional).ToList();


        var indentCount = new Dictionary<string, int>();
        foreach (var check in conditionalChecks)
        {
            var indent = 0;
            if (indentCount.TryGetValue(check.name, out var value))
                indent = value;
            if (check.conditionalChecksNo != null)
                foreach (var child in check.conditionalChecksNo)
                {
                    indentCount[child] = indent + 1;
                }

            if (check.conditionalChecksYes != null)
                foreach (var child in check.conditionalChecksYes)
                {
                    indentCount[child] = indent + 1;
                }
        }


        for (var i = 0; i < checklist.checks.Count; i++)
        {
            checklist.checks[i].Index = i;
            var indent = indentCount.ContainsKey(checklist.checks[i].name) ? indentCount[checklist.checks[i].name] : 0;

            if (checklist.checks[i].IsConditional)
            {
                // instantiate conditional check
                var conditionalCheckObject = Instantiate(conditionalCheckPrefab, checkContainer.transform);
                var conditionalCheckRenderer = conditionalCheckObject.GetComponent<ConditionalCheckRenderer>();
                conditionalCheckRenderer.check = checklist.checks[i];
                conditionalCheckRenderer.indentation = indent;
                _checkObjects.Add(conditionalCheckObject);
            }
            else
            {
                if (checklist.checks[i].name == "PageBreak")
                {
                    _pagesCount++;
                }

                var checkObject = Instantiate(checkPrefab, checkContainer.transform);
                var checkRenderer = checkObject.GetComponent<CheckRenderer>();
                checkRenderer.check = checklist.checks[i];
                checkRenderer.SetTextSize(characterCount, splitNameLimit);
                checkRenderer.indentation = indent;
                //checkObject.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta =
                //  new Vector2(checkObject.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x, (checklist.checks[i].expectedValue.Length / characterCount + 1) * 60);
                //checkObject.GetComponent<RectTransform>().sizeDelta = new Vector2(checkObject.GetComponent<RectTransform>().sizeDelta.x, (checklist.checks[i].expectedValue.Length / characterCount + 1) * 60);
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

    public void OnCheckboxCheck()
    {
        if (_currentChecklist?.IsDone() == true)
            ChecklistDone();
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
        LoadPage();
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
        var isCompleteExceptDeferred = _currentChecklist.IsDoneWithoutDeferred();
        if (isOverridden)
            checklistStatus.GetComponentInChildren<TMP_Text>().text = "CHECKLIST OVERRIDDEN";
        else if (isCompleteExceptDeferred)
            checklistStatus.GetComponentInChildren<TMP_Text>().text = "CHECKLIST COMPLETE EXCEPT DEFERRED";
        else
            checklistStatus.GetComponentInChildren<TMP_Text>().text = "CHECKLIST COMPLETE";

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
            if (i == 0)
            {
                ColorBlock cb = pageButton.GetComponent<Button>().colors;
                cb.normalColor = new Color(.2f, .2f, .3f);
                pageButton.GetComponent<Button>().colors = cb;
            }

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

        pageButtons.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        pageButtons.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();

        pageButtons.SetActive(false);
    }

    public void HandlePreviousPage()
    {
        if (_currentPage < 2)
            return;
        _currentPage--;

        ColorBlock cb;
        for (int i = 1; i < pageButtons.transform.childCount - 1; i++)
        {
            cb = pageButtons.transform.GetChild(i).GetComponent<Button>().colors;
            cb.normalColor = new Color(.3f, .3f, .4f);
            pageButtons.transform.GetChild(i).GetComponent<Button>().colors = cb;
        }

        cb = pageButtons.transform.GetChild(_currentPage).GetComponent<Button>().colors;
        cb.normalColor = new Color(.2f, .2f, .3f);
        pageButtons.transform.GetChild(_currentPage).GetComponent<Button>().colors = cb;

        Debug.Log("TRENUTNA: " + _currentPage);
        LoadPage();
    }

    public void HandleNextPage()
    {
        if (_currentPage >= _highestPage)
            return;
        _currentPage++;

        ColorBlock cb;
        for (int i = 1; i < pageButtons.transform.childCount - 1; i++)
        {
            cb = pageButtons.transform.GetChild(i).GetComponent<Button>().colors;
            cb.normalColor = new Color(.3f, .3f, .4f);
            pageButtons.transform.GetChild(i).GetComponent<Button>().colors = cb;
        }

        cb = pageButtons.transform.GetChild(_currentPage).GetComponent<Button>().colors;
        cb.normalColor = new Color(.2f, .2f, .3f);
        pageButtons.transform.GetChild(_currentPage).GetComponent<Button>().colors = cb;

        Debug.Log("TRENUTNA: " + _currentPage);
        LoadPage();
    }

    public void HandlePageButtonPress(int pageNumber)
    {
        _currentPage = pageNumber;

        ColorBlock cb;
        for (int i = 1; i < pageButtons.transform.childCount - 1; i++)
        {
            cb = pageButtons.transform.GetChild(i).GetComponent<Button>().colors;
            cb.normalColor = new Color(.3f, .3f, .4f);
            pageButtons.transform.GetChild(i).GetComponent<Button>().colors = cb;
        }

        cb = pageButtons.transform.GetChild(_currentPage).GetComponent<Button>().colors;
        cb.normalColor = new Color(.2f, .2f, .3f);
        pageButtons.transform.GetChild(_currentPage).GetComponent<Button>().colors = cb;

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
                _checkObjects[i].GetComponent<CheckRenderer>()?.SetCheckRendered();
                _checkObjects[i].GetComponent<ConditionalCheckRenderer>()?.SetCheckRendered();
            }
            else
            {
                _checkObjects[i].SetActive(false);
            }
        }

        if (_currentChecklist != null && _currentChecklist.IsDone())
            ChecklistDone();
        else
            ChecklistNotDone();
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