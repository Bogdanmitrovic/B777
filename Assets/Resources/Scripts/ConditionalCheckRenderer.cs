using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionalCheckRenderer : MonoBehaviour
{
    public Check check;
    public Outline outline;
    
    public GameObject conditionalCheckContainer;

    public Image checkmarkImageLeft;
    public Image checkmarkBackgroundImageLeft;
    public TMP_Text checkTextComponentLeft;
    
    public Image checkmarkImageRight;
    public Image checkmarkBackgroundImageRight;
    public TMP_Text checkTextComponentRight;
    
    private int _characterCount;
    private int _splitNameLimit;
/*
    void Start()
    {
        SetListeners();
        check.OnCheckDataChanged += UpdateUI;
        UpdateUI();
    }

    private void SetListeners()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            transform.parent.parent.GetComponent<ChecklistRenderer>().OnCheckSelect(check.Index);
        });

        if (!check.isAutomatic)
        {
            checkmarkBackgroundImage.GetComponent<Button>().onClick.AddListener(() => { check.TriggerCheck(); });
        }
        else
        {
            checkmarkBackgroundImage.enabled = false;
        }
    }
    private void UpdateUI()
    {
        checkTextComponent.text = Text(_characterCount, _splitNameLimit);
        SetColors();
        checkmarkImage.enabled = check.Checked;
        outline.enabled = check.IsSelected;
    }

    private void SetColors()
    {
        if (check.IsNote)
            return;
        var color = check.Overridden ? new Color(.23f, .64f, .76f, 1) : check.Checked ? Color.green : Color.white;
        checkmarkImage.color = color;
        checkTextComponent.color = color;
        checkmarkBackgroundImage.color = check.Overridden ? new Color(0f, 0f, 0f, 1f) : new Color(1f, 1f, 1f, .5f);
        checkmarkBackgroundImage.GetComponent<Outline>().enabled = check.Overridden;
    }
    private string Text(int characterCount, int splitNameLimit)
    {
        if (check.IsNote) return check.name + " " + check.expectedValue;
        var stringBuilder = new StringBuilder();
        var count = 0;
        if (hasIndentation)
        {
            stringBuilder.Append("   ");
            count = 3;
        }
        var words = check.name.Split(' ');
        for (var i = 0; i < words.Length; i++)
        {
            var word = words[i];
            stringBuilder.Append(word);
            count += word.Length;
            if (i != words.Length - 1 && count >= splitNameLimit)
            {
                stringBuilder.Append("\n");
                count = 0;
                if (!hasIndentation) continue;
                stringBuilder.Append("   ");
                count = 3;
            }
            else
            {
                stringBuilder.Append(" ");
                count++;
            }
        }
        count += check.expectedValue.Length;
        stringBuilder.Append(new string('.', characterCount - count));
        stringBuilder.Append(check.expectedValue);
        return stringBuilder.ToString();
    }
    public void SetTextSize(int characterCount, int splitNameLimit)
    {
        _characterCount = characterCount;
        _splitNameLimit = splitNameLimit;
    }
    private void OnDestroy()
    {
        check.OnCheckDataChanged -= UpdateUI;
    }*/
}
