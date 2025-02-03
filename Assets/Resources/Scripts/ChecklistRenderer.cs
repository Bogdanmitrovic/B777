#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistRenderer : MonoBehaviour
{
    public int characterCount = 50;
    public int splitNameLimit = 20;
    public int checksPerPage = 8;

    public GameObject checkPrefab;
    public GameObject pageNumberPrefab;
    public GameObject topButtons;
    public GameObject bottomButtons;
    public GameObject pageButtons;
    public GameObject checklistDone;
    public GameObject title;
    public GameObject menuContainer;
    public GameObject checkContainer;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public TextAsset jsonFile;

    private int _checklistIndex = 0;
    private List<Checklist> _normalChecklists = new();
    private Checklist? _currentChecklist;
    private int _currentMenu = -1;
    private int _currentPage = 1;
    private int _pagesCount = 0;
    private int _highestPage = -1;
    private int _leftChildCount = 0;
    private ListMenu menus;
    private List<GameObject> _checkObjects = new();

    void Start()
    {
        menus = JsonUtility.FromJson<ListMenu>(jsonFile.text);
        LoadFromJson();
    }

    private void LoadFromJson()
    {
        if (jsonFile == null) return;
        var menu = menus.Menus[0];
        foreach (var list in menu.Lists)
        {
            var checklist = new Checklist
            {
                Name = list.ListName
            };
            for (var i = 0; i < list.List.Count; i++)
            {
                var item = list.List[i];
                checklist.AddCheck(new Check(item.name, item.expectedValue, item.isAutomatic, i));
            }

            _normalChecklists.Add(checklist);
        }
    }

    public void LoadNormalChecklist(int index = -1)
    {
        if (index != -1)
            _checklistIndex = index;
        if (_checklistIndex >= _normalChecklists.Count() || _checklistIndex < 0)
        {
            _checklistIndex = 0;
        }

        LoadChecklist(_normalChecklists[_checklistIndex]);
        _checklistIndex++;
    }

    private void LoadChecklist(Checklist checklist)
    {
        UnloadCurrentChecklist();
        _currentChecklist = checklist;
        _checkObjects.Clear();
        checklist.OnCheckChecked += OnCheckboxCheck;
        foreach (var check in checklist.Checks)
        {
            var checkObject = Instantiate(checkPrefab, checkContainer.transform);
            checkObject.GetComponent<CheckRenderer>().Check = check;
            _checkObjects.Add(checkObject);
        }

        _pagesCount = (_currentChecklist.Checks.Count - 1) / checksPerPage + 1;
        _currentPage = 1;
        if (_pagesCount > 1)
        {
            SetPageButtons();
        }

        // povecaj right ako ima paging TODO pogledaj jel treba i dalje ovo
        if (pageButtons.activeSelf)
        {
            bottomButtons.GetComponent<RectTransform>().offsetMax = new
                Vector2(-325f, bottomButtons.GetComponent<RectTransform>().offsetMax.y);
        }
        else
        {
            bottomButtons.GetComponent<RectTransform>().offsetMax = new
                Vector2(-165f, bottomButtons.GetComponent<RectTransform>().offsetMax.y);
        }

        LoadPage();
        bottomButtons.SetActive(true);
        ChecklistNotDone();
    }

    public void UnloadCurrentChecklist()
    {
        if (_currentChecklist == null) return;
        checklistDone.SetActive(false);
        _currentChecklist.OnCheckChecked -= OnCheckboxCheck;
        foreach (var checkObject in _checkObjects)
        {
            Destroy(checkObject);
        }
    }

    public void OnCheckboxCheck(int index, bool value)
    {
        for (var i = (_currentPage - 1) * checksPerPage;
             i < _currentPage * checksPerPage && i < _currentChecklist?.Checks.Count;
             i++)
        {
            if (!_currentChecklist.Checks[i].Checked)
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
        checklistDone.SetActive(true);
        bottomButtons.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void ChecklistNotDone()
    {
        checklistDone.SetActive(false);
        bottomButtons.transform.GetChild(1).gameObject.SetActive(true);
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