using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

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
    private int _currentPage = 1;

    void Start()
    {
        bottomButtonSlots = new GameObject[6];
        for (var i = 0; i < 6; i++)
        {
            bottomButtonSlots[i] = bottomButtonContainer.transform.GetChild(i).gameObject;
        }

        _checklistRenderer = screenContainer.GetComponent<ChecklistRenderer>();

        var button = Instantiate(buttonPrefab, bottomButtonSlots[0].transform);
        button.GetComponentInChildren<TMP_Text>().text = "NORMAL";
        button.GetComponent<Button>().onClick.AddListener(LoadNormalChecklist);
        _buttons.Add("NORMAL", button);

        button = Instantiate(buttonPrefab, bottomButtonSlots[1].transform);
        button.GetComponentInChildren<TMP_Text>().text = "ITEM OVRD";
        button.GetComponent<Button>().onClick.AddListener(_checklistRenderer.OverrideCheck);
        _buttons.Add("ITEMOVRD", button);

        button = Instantiate(buttonPrefab, bottomButtonSlots[3].transform);
        button.GetComponentInChildren<TMP_Text>().text = "CHKL OVRD";
        button.GetComponent<Button>().onClick.AddListener(_checklistRenderer.OverrideChecklist);
        _buttons.Add("CHKLOVRD", button);

        button = Instantiate(buttonPrefab, bottomButtonSlots[4].transform);
        button.GetComponentInChildren<TMP_Text>().text = "CHKL RESET";
        button.GetComponent<Button>().onClick.AddListener(_checklistRenderer.ResetChecklist);
        _buttons.Add("CHKLRESET", button);

        button = Instantiate(buttonPrefab, bottomButtonSlots[5].transform);
        button.GetComponentInChildren<TMP_Text>().text = "EXIT MENU";
        // TODO button.GetComponent<Button>().onClick.AddListener(checklistRenderer.ExitMenu);
        button.SetActive(false);
        _buttons.Add("EXITMENU", button);
        bottomButtonContainer.SetActive(true);

        LoadMenusFromJson();
        ShowMenu(0);
        topButtonContainer.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ShowMenu(0); });
        topButtonContainer.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { ShowMenu(1); });
        topButtonContainer.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { ShowMenu(2); });
    }

    private void LoadMenusFromJson()
    {
        _menus = JsonConvert.DeserializeObject<List<Menu>>(jsonFile.text);
        _checklistRenderer.SetChecklists(_menus[0].checklists);
    }

    void LoadNormalChecklist()
    {
        _checklistRenderer.LoadNormalChecklist();
    }

    public void ShowMenu(int menuNumber)
    {
        SetMenuContentActive();
        if (menuNumber == _currentMenu)
            return;

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


            var i1 = i;
            button.transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                _checklistRenderer.LoadNormalChecklist(i1);
                SetChecksContentActive();
            });
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
        
        ShowButtons(new [] {
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