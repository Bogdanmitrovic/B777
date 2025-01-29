#nullable enable
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class ChecklistRenderer : MonoBehaviour
{
    public int characterCount = 50;
    public int splitNameLimit = 20;
    public int checksPerPage = 8;
    public GameObject checkPrefab;
    public GameObject buttonPrefab;
    public GameObject pageNumberPrefab;
    public GameObject topButtons;
    public GameObject bottomButtons;
    public GameObject pageButtons;
    public GameObject checklistDone;
    public GameObject title;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public TextAsset jsonFile;
    
    private int _checklistIndex = 0;
    private List<Checklist> _normalChecklists;
    private Checklist? _currentChecklist;
    private int _currentMenu = -1;
    private int _currentPage = 1;
    private int _pagesCount = 0;
    private int _highestPage = -1;
    private int _leftChildCount = 0;
    private ListMenu menus;
    
    void Start()
    {
        // normal
        // itemovrd
        // chklovrd
        // chklreset
        Checklist checklist = new Checklist();
        checklist.Checks.Add(new Check ("Oxygen", "Tested 100%" ,  false));
        checklist.Checks.Add(new Check ("Flight instruments", "Heading ___, Altimeter ___" ,  false));
        checklist.Checks.Add(new Check ("Parking brake", "Set", true));
        checklist.Checks.Add(new Check ("Fuel Control Switches", "CUTOFF", true));
        // checkListParent.GetComponent<VerticalLayoutGroup>();
        _normalChecklists = new List<Checklist>
        {
            checklist
        };
        bottomButtons.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(LoadNormalChecklist);
        bottomButtons.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(OverrideCheck);
        bottomButtons.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(OverrideChecklist);
        bottomButtons.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(ResetChecklist);
        topButtons.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(()=>{ShowMenu(0);});
        topButtons.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(()=>{ShowMenu(1);});
        topButtons.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(()=>{ShowMenu(2);});

        menus = JsonUtility.FromJson<ListMenu>(jsonFile.text);
        
        // LoadNormalChecklist();
        // OverrideChecklist();
        // OnCheckSelect(0);
        // OverrideCheck();
        // ResetChecklist();
    }
    
    private void LoadNormalChecklist()
    {
        if(_checklistIndex is >= 11 or < 0)
        {
            _checklistIndex = 0;
        }
        LoadChecklist(_normalChecklists[_checklistIndex]);
        _checklistIndex++;
    }

    private void LoadChecklist(Checklist checklist)
    {
        _currentChecklist?.Unload();
        _currentChecklist = checklist;
        checklist.Load(checkPrefab, gameObject, characterCount, splitNameLimit, _currentPage, checksPerPage);
        ChecklistNotDone();

        _pagesCount = (_currentChecklist.Checks.Count - 1) / checksPerPage + 1;
        if (_pagesCount > 1)
        {
            SetPageButtons();
        }
    }

    public void OnCheckboxCheck(int index, bool value)
    {
        if (_currentChecklist?.IsDone() != true) return;
        ChecklistDone();
    }

    public void OnCheckSelect(int index)
    {
        _currentChecklist?.OnCheckSelect(index);
    }

    private void OverrideCheck()
    {
        _currentChecklist?.OverrideCheck();
        if (_currentChecklist?.IsDone() == true)
        {
            ChecklistDone();
        }
    }

    private void OverrideChecklist()
    {
        _currentChecklist?.OverrideChecklist();
        ChecklistDone();
    }

    private void ResetChecklist()
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

    public void ShowMenu(int menuNumber)
    {
        if (menuNumber == _currentMenu)
            return;

        RemovePageButtons();
        ClearMenu();
        _currentMenu = menuNumber;
        
        _currentChecklist?.Unload();
        bottomButtons.SetActive(false);
        checklistDone.SetActive(false);
        
        var verticalLayoutGroup1 = horizontalLayoutGroup.transform.GetChild(0);
        var verticalLayoutGroup2 = horizontalLayoutGroup.transform.GetChild(1);
        
        if (jsonFile != null)
        {
            MenuItem menu = menus.Menus[menuNumber];
            const float buttonHeight = 50f;
            
            title.SetActive(true);
            title.GetComponent<TMP_Text>().text = menu.MenuName.ToUpper();
            
            foreach (var list in menu.Lists)
            {
                GameObject button;
                CreateButton(verticalLayoutGroup1, verticalLayoutGroup2, out button);
                button.GetComponentInChildren<TMP_Text>().text = list.ListName;
                
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, buttonHeight);
                
                Checklist checklist = new Checklist();
                foreach (var item in list.List)
                {
                    checklist.Checks.Add(new Check(item.name, item.expectedValue, item.isAutomatic));
                }

                button.transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ClearMenu();
                    title.GetComponent<TMP_Text>().text = list.ListName.ToUpper();
                    _currentPage = 1;
                    LoadChecklist(checklist);
                    bottomButtons.SetActive(true);
                });

                
            }
        }
    }
    
    private void CreateButton(Transform verticalLayoutGroup1, Transform verticalLayoutGroup2, out GameObject button)
    {
        if (_leftChildCount < 10)
        {
            button = Instantiate(buttonPrefab, verticalLayoutGroup1);
            _leftChildCount++;
        }
        else
        {
            button = Instantiate(buttonPrefab, verticalLayoutGroup2);
        }
        button.AddComponent<LayoutElement>().flexibleHeight = 0;
        button.GetComponent<LayoutElement>().preferredHeight = 75;
        button.transform.localScale = Vector3.one;
        button.GetComponentInChildren<TMP_Text>().fontSize = 32;
        button.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.Left;
        button.GetComponentInChildren<TMP_Text>().margin = new Vector4(20, 0, 0, 0);
    }
    
    public void ClearMenu()
    {
        _currentMenu = -1;
        var verticalLayoutGroup1 = horizontalLayoutGroup.transform.GetChild(0);
        var verticalLayoutGroup2 = horizontalLayoutGroup.transform.GetChild(1);
        
        for (int i = verticalLayoutGroup1.childCount-1; i >= 0; i--)
        {
            Destroy(verticalLayoutGroup1.GetChild(i).gameObject);
            _leftChildCount--;
        }
        for (int i = verticalLayoutGroup2.childCount-1; i >= 0; i--)
        {
            Destroy(verticalLayoutGroup2.GetChild(i).gameObject);
        }
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
            pageButton.GetComponent<Button>().onClick.AddListener((() =>
            {
                HandlePageButtonPress(iCopy + 1);
            }));
            
            RectTransform pageButtonRect = pageButton.GetComponent<RectTransform>();
            
            pageButtonRect.localScale = Vector3.one;
            pageButtonRect.sizeDelta = new Vector2(pageButtonRect.sizeDelta.x, (pageButtonsRect.sizeDelta.y - 200) / _pagesCount);
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
        LoadChecklist(_currentChecklist);
    }

    public void HandleNextPage()
    {
        if(_currentPage >= _highestPage)
            return;
        _currentPage++; 
        LoadChecklist(_currentChecklist);
    }

    public void HandlePageButtonPress(int pageNumber)
    {
        _currentPage = pageNumber; 
        LoadChecklist(_currentChecklist);
    }
}
