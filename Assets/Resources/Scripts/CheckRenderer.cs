using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckRenderer : MonoBehaviour
{
    public Check check;
    public Image checkmarkImage;
    public Image checkmarkBackgroundImage;
    public TMP_Text checkTextComponent;
    public Outline outline;
    public int indentation;
    private int _splitNameLimit;
    private int _characterCount;

    //TODO da se stekuju odozgo a ne na sredinu
    //TODO kad predje u novi red samostalno ne radi indent
    //TODO 
    void Start()
    {
        SetListeners();
        check.OnCheckDataChanged += UpdateUI;
        UpdateUI();
    }

    private void SetListeners()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => { check.TriggerSelect(true); });

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
        checkmarkImage.enabled = (check.Checked || check.Overridden) && !check.IsNote;
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
        if (check.IsNote) return "NOTE: " + check.expectedValue;

        var stringBuilder = new StringBuilder();
        var count = 0;
        // append indentation*"   "
        var indentString= new string(' ', indentation * 3);
        stringBuilder.Append(indentString);
        count += indentation * 3;
        var words = check.name.Split(' ');
        if (check.IsNote || check.IsPlainText) words = check.expectedValue.Split(' ');
        for (var i = 0; i < words.Length; i++)
        {
            var word = words[i];
            if (word.Contains('\n'))
            {
                count = 0;
                var split = word.Split('\n');
                stringBuilder.Append('\n');
                stringBuilder.Append(indentString);
                stringBuilder.Append(split[1]);
                count += indentString.Length + split[1].Length;
            }
            else
            {
                stringBuilder.Append(word);
                count += word.Length;
            }
            if (i != words.Length - 1 && count >= splitNameLimit)
            {
                if ((check.IsPlainText || check.IsNote) && count >= _characterCount || !check.IsPlainText )
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
            }
            else
            {
                stringBuilder.Append(" ");
                count++;
            }
        }
        if(check.IsPlainText) return stringBuilder.ToString();
        count += check.expectedValue.Length;
        if (characterCount - count > 0)
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
    }
}