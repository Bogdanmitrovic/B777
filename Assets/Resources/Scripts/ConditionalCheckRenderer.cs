using System.Text;
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

    public int indentation;

    public int _characterCount;
    public int _splitNameLimit;

    void Start()
    {
        SetListeners();
        check.OnCheckDataChanged += UpdateUI;
        ButtonsEnabled(true);
        UpdateUI();
    }

    private void SetListeners()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => { check.TriggerSelect(true); });
        checkmarkBackgroundImageLeft.GetComponent<Button>().onClick
            .AddListener(() => { CheckConditional(ConditionalState.Yes); });
        checkmarkBackgroundImageRight.GetComponent<Button>().onClick
            .AddListener(() => { CheckConditional(ConditionalState.No); });
    }

    private void CheckConditional(ConditionalState state)
    {
        check.TriggerConditionCheck(state);
    }

    private void ButtonsEnabled(bool state)
    {
        checkmarkBackgroundImageLeft.GetComponent<Button>().enabled = state;
        checkmarkBackgroundImageRight.GetComponent<Button>().enabled = state;
    }


    private void UpdateUI()
    {
        checkTextComponent.text = Text();
        checkTextComponentLeft.text = "Yes";
        checkTextComponentRight.text = "No";
        checkmarkImageLeft.enabled = check.ConditionalState == ConditionalState.Yes;
        checkmarkImageRight.enabled = check.ConditionalState == ConditionalState.No;
        // ButtonsEnabled(check.ConditionalState == ConditionalState.None);
        //ButtonsEnabled(!check.IsDone);
        SetColors();
        outline.enabled = check.IsSelected;
    }

    private string Text()
    {
        var stringBuilder = new StringBuilder();
        // append indentation*"   "
        int count = 0;
        var indentString = new string(' ', indentation * 3);
        stringBuilder.Append(indentString);
        stringBuilder.Append(check.name);
        /*var words = check.name.Split(' ');
        for (var i = 0; i < words.Length; i++)
        {
            var word = words[i];
            stringBuilder.Append(word);
            count += word.Length;
            if (i != words.Length - 1 && count >= _characterCount)
            {
                stringBuilder.Append("\n");
                stringBuilder.Append(indentString);
                count = indentation * 3;
            }
            else
            {
                stringBuilder.Append(" ");
                count++;
            }
        }*/
        return stringBuilder.ToString();
    }

    private void SetColors()
    {
        var color = check.Overridden ? new Color(.23f, .64f, .76f, 1) : check.Checked ? Color.green : Color.white;
        checkTextComponent.color = color;
    }

    private void OnDestroy()
    {
        check.OnCheckDataChanged -= UpdateUI;
    }

    public void SetCheckRendered()
    {
        check.Rendered = true;
    }
}