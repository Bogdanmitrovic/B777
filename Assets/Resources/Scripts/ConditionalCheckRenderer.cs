using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionalCheckRenderer : MonoBehaviour
{
    public Check check;
    public Outline outline;
    public TMP_Text checkTextComponent;
    
    public GameObject conditionalCheckContainer;

    public Image checkmarkImageLeft;
    public Image checkmarkBackgroundImageLeft;
    public TMP_Text checkTextComponentLeft;
    
    public Image checkmarkImageRight;
    public Image checkmarkBackgroundImageRight;
    public TMP_Text checkTextComponentRight;
    
    private int _characterCount;
    private int _splitNameLimit;

    void Start()
    {
        SetListeners();
        check.OnCheckDataChanged += UpdateUI;
        SetConditionalCheckState(true);
        UpdateUI();
    }
    private void SetListeners()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            check.TriggerSelect(true);
        });
        checkmarkBackgroundImageLeft.GetComponent<Button>().onClick.AddListener(() => { CheckConditional(ConditionalState.Yes); });
        checkmarkBackgroundImageRight.GetComponent<Button>().onClick.AddListener(() => { CheckConditional(ConditionalState.No); });
        
    }

    private void CheckConditional(ConditionalState state)
    {
        check.TriggerConditionCheck(state);
        switch (state)
        {
            case ConditionalState.No:
                { 
                    check.TriggerOverride();
                    SetConditionalCheckState(false);
                    break;
                }
            case ConditionalState.Yes:
                {
                    check.TriggerCheck();
                    SetConditionalCheckState(false);
                    break;
                }
        }
    }

    private void SetConditionalCheckState(bool state)
    {
        checkmarkBackgroundImageLeft.GetComponent<Button>().enabled = state;
        checkmarkBackgroundImageRight.GetComponent<Button>().enabled = state;
    }
       

    private void UpdateUI()
    {
        checkTextComponent.text = check.name;
        checkTextComponentLeft.text = "Yes";
        checkTextComponentRight.text = "No";
        checkmarkImageLeft.enabled = check.ConditionalState == ConditionalState.Yes;
        checkmarkImageRight.enabled = check.ConditionalState == ConditionalState.No;
        if(check.ConditionalState == ConditionalState.None) SetConditionalCheckState(true);
        SetColors();
        outline.enabled = check.IsSelected;
    }
    private void SetColors()
    {
        // TODO set conditional check colors
        var color = check.Overridden ? new Color(.23f, .64f, .76f, 1) : check.Checked ? Color.green : Color.white;
        checkTextComponent.color = color;
    }
    private void OnDestroy()
    {
        check.OnCheckDataChanged -= UpdateUI;
    }
}
