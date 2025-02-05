#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
    public GameObject pageNumberPrefab;
    public GameObject pageButtons;
    public GameObject checklistStatus;
    public GameObject title;
    public GameObject checkContainer;
    public GameObject menuManager;
    public HorizontalLayoutGroup horizontalLayoutGroup;

    private int _checklistIndex = 0;
    private List<Checklist> _checklists = new();
    private Checklist? _currentChecklist;
    private int _currentPage = 1;
    private int _pagesCount = 0;
    private int _highestPage = -1;
    private List<GameObject> _checkObjects = new();

    public void SetChecklists(List<Checklist> checklists)
    {
        _checklists = checklists;
    }

    public void LoadNextNormalChecklist()
    {
        var checklist = _checklists.FirstOrDefault(checklist => !checklist.IsDone()) ?? _checklists.Last();
        LoadChecklist(checklist);
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
        _currentChecklist.SetListeners();
        title.GetComponent<TMP_Text>().text = checklist.name;
        checklist.OnCheckChecked += OnCheckboxCheck;
        for (var i = 0; i < checklist.checks.Count; i++)
        {
            var checkObject = Instantiate(checkPrefab, checkContainer.transform);
            checkObject.GetComponent<CheckRenderer>().Check = checklist.checks[i];
            if (checklist.checks[i].name == "NOTE")
            {
                checkObject.transform.GetChild(0).gameObject.SetActive(false);
                checkObject.transform.GetChild(1).GetComponent<TMP_Text>().text =
                    checklist.checks[i].name + " " + checklist.checks[i].expectedValue;
            }

            checklist.checks[i].Index = i;
            _checkObjects.Add(checkObject);
        }

        _pagesCount = (_currentChecklist.checks.Count - 1) / checksPerPage + 1;
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
        for (var i = (_currentPage - 1) * checksPerPage;
             i < _currentPage * checksPerPage && i < _currentChecklist?.checks.Count;
             i++)
        {
            if (!_currentChecklist.checks[i].Checked)
            {
                SetPageNotComplete();
                return;
            }
        }

        SetPageComplete();
        if (_currentChecklist?.IsDone() != true) return;
        ChecklistDone();
    }

    public void OnCheckSelect(int index)
    {
        Debug.Log("Check selected " + index);
        _currentChecklist?.OnCheckSelect(index);
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
            isOverridden ? "CHECKLIST OVERRIDDEN" : "CHECKLIST DONE";
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
        for (var i = 0; i < _checkObjects.Count; i++)
        {
            _checkObjects[i].SetActive(i >= (_currentPage - 1) * checksPerPage && i < _currentPage * checksPerPage);
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