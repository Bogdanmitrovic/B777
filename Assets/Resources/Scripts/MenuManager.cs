using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    // prefabs
    public GameObject buttonPrefab;

    // containers
    public GameObject topButtonContainer;
    public GameObject bottomButtonContainer;
    public GameObject[] bottomButtonSlots;
    public GameObject screenContainer;

    // content of ScreenContainer
    public GameObject[] checksContent;
    public GameObject[] menuContent;

    // other
    public GameObject title;
    public TextAsset jsonFile;

    // private variables
    private readonly Dictionary<string, GameObject> _buttons = new();
    private ChecklistRenderer _checklistRenderer;
    private int _currentMenu = -1;
    private int _leftChildCount = 0;
    private List<Menu> _menus;

    void Start()
    {
        bottomButtonSlots = new GameObject[6];
        for (var i = 0; i < 6; i++)
        {
            bottomButtonSlots[i] = bottomButtonContainer.transform.GetChild(i).gameObject;
        }

        _checklistRenderer = screenContainer.GetComponent<ChecklistRenderer>();

        CreateBottomButton("NORMAL", "NORMAL", LoadNormalChecklist, 0);
        CreateBottomButton("ITEM OVRD", "ITEMOVRD", _checklistRenderer.OverrideCheck, 1);
        CreateBottomButton("CHKL OVRD", "CHKLOVRD", _checklistRenderer.OverrideChecklist, 3);
        CreateBottomButton("CHKL RESET", "CHKLRESET", _checklistRenderer.ResetChecklist, 4);
        //TODO listener za exit menu
        CreateBottomButton("EXIT MENU", "EXITMENU", null, 5, false);
        bottomButtonContainer.SetActive(true);

        LoadMenusFromJson();
        ShowMenu(0);
        for (var i = 0; i < 3; i++)
            ShowMenuSetListener(i);
    }

    public void CreateBottomButton(string buttonText, string buttonKey, UnityAction onClickListener, int slotIndex,
        bool isActive = true)
    {
        var button = Instantiate(buttonPrefab, bottomButtonSlots[slotIndex].transform);
        button.GetComponentInChildren<TMP_Text>().text = buttonText;
        button.GetComponent<Button>().onClick.AddListener(onClickListener);
        button.SetActive(isActive);
        _buttons.Add(buttonKey, button);
    }

    public void ShowMenuSetListener(int index)
    {
        topButtonContainer.transform.GetChild(index).GetComponent<Button>().onClick
            .AddListener(() => { ShowMenu(index); });
    }

    private void LoadMenusFromJson()
    {
        _menus = JsonConvert.DeserializeObject<List<Menu>>(jsonFile.text);
        _checklistRenderer.SetChecklists(_menus[0].checklists);
    }

    void LoadNormalChecklist()
    {
        var loaded = _checklistRenderer.LoadNextNormalChecklist();
        if (!loaded)
            ShowMenu(0);
    }

    public void ShowMenu(int menuNumber)
    {
        SetMenuContentActive();
        ClearMenu();
        _currentMenu = menuNumber;
        _checklistRenderer.SetChecklists(_menus[menuNumber].checklists);

        var verticalLayoutGroup1 = menuContent[0].transform.GetChild(0);
        var verticalLayoutGroup2 = menuContent[0].transform.GetChild(1);

        const float buttonHeight = 50f;

        var menu = _menus[menuNumber];

        title.SetActive(true);
        title.GetComponent<TMP_Text>().text = menu.menuName.ToUpper();

        for (var i = 0; i < menu.checklists.Count; i++)
        {
            var list = menu.checklists[i];
            GameObject button;
            CreateButton(verticalLayoutGroup1, verticalLayoutGroup2, out button);
            button.GetComponentInChildren<TMP_Text>().text = list.name;

            RectTransform buttonRect = button.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, buttonHeight);

            if (menuNumber == 1)
            {
                for (var j = 0; j < 3; j++)
                {
                    button.transform.GetComponent<Button>().onClick.AddListener(() => ResetButtonFunctions(j));
                }
            }
            else
            {
                var i1 = i;
                button.transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SetChecksContentActive();
                    _checklistRenderer.LoadChecklistByIndex(i1);
                });
            }
            
        }
    }

    public void ResetButtonFunctions(int index)
    {
        switch (index)
        {
            case < 0:
                return;
            case > 3:
                return;
            case 0:
                ResetNormal();
                break;
            case 1:
                ResetNonNormal();
                break;
            default:
                LoadMenusFromJson();
                break;
        }
    }

    public void ResetNormal()
    {
        foreach (var checklist in _menus[0].checklists)
        {
            checklist.Reset();
        }
    }

    public void ResetNonNormal()
    {
        foreach (var checklist in _menus[2].checklists)
        {
            checklist.Reset();
        }
    }

    public void ClearMenu()
    {
        _currentMenu = -1;
        var verticalLayoutGroup1 = menuContent[0].transform.GetChild(0);
        var verticalLayoutGroup2 = menuContent[0].transform.GetChild(1);

        for (var i = verticalLayoutGroup1.childCount - 1; i >= 0; i--)
        {
            Destroy(verticalLayoutGroup1.GetChild(i).gameObject);
            _leftChildCount--;
        }

        for (var i = verticalLayoutGroup2.childCount - 1; i >= 0; i--)
        {
            Destroy(verticalLayoutGroup2.GetChild(i).gameObject);
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

    private void SetMenuContentActive()
    {
        foreach (var content in checksContent)
        {
            content.SetActive(false);
            _checklistRenderer.UnloadCurrentChecklist();
        }

        foreach (var content in menuContent)
        {
            content.SetActive(true);
        }

        ShowButtons(new[]
        {
            "EXITMENU"
        });
    }

    private void SetChecksContentActive()
    {
        foreach (var content in checksContent)
        {
            content.SetActive(true);
        }

        foreach (var content in menuContent)
        {
            content.SetActive(false);
        }

        ShowButtons(new[]
        {
            "NORMAL", "ITEMOVRD", "CHKLOVRD", "CHKLRESET"
        });
    }

    public void ShowButtons(string[] buttons)
    {
        foreach (var button in _buttons)
        {
            button.Value.SetActive(false);
        }

        foreach (var button in buttons)
        {
            _buttons[button].SetActive(true);
        }
    }
}