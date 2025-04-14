using System;
using System.Text;
using TMPro;
using Unity.VisualScripting;
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
        checkmarkImage.enabled = (check.Checked || check.Overridden ) && !check.IsNote && !check.expectedValue.Contains(":");
        checkmarkBackgroundImage.enabled = !check.isAutomatic && !check.IsNote && !check.expectedValue.Contains(":");
        outline.enabled = check.IsSelected;
    }

    private void SetColors()
    {
        if (check.IsNote)
            return;
        var color = check.Overridden ? new Color(.23f, .64f, .76f, 1) : check.Checked ? Color.green : Color.white;
        if (check.isAutomatic && (check.expectedValue.Contains(':') || check.expectedValue.Contains("--") || check.expectedValue.Contains("==>")) && !check.Overridden) color = Color.white;
        checkmarkImage.color = color;
        checkTextComponent.color = color;
        checkmarkBackgroundImage.color = check.Overridden ? new Color(0f, 0f, 0f, 1f) : new Color(1f, 1f, 1f, .5f);
        checkmarkBackgroundImage.GetComponent<Outline>().enabled = check.Overridden;
    }

private string Text(int characterCount, int splitNameLimit)
{
    if (check.IsNote)
        return "NOTE: " + check.expectedValue;
    if (check.IsPlainText)
        return check.expectedValue;
        
    var stringBuilder = new StringBuilder();
    var indentString = new string(' ', indentation * 3);
    var count = indentString.Length;
    stringBuilder.Append(indentString);
    
    // Format the name part with proper wrapping
    var words = check.name.Split(' ');
    for (int i = 0; i < words.Length; i++)
    {
        var word = words[i];
        if (word.Contains('\n'))
        {
            var split = word.Split('\n');
            stringBuilder.Append('\n');
            stringBuilder.Append(indentString);
            stringBuilder.Append(split[1]);
            count = indentString.Length + split[1].Length;
        }
        else
        {
            // Check if adding this word would exceed splitNameLimit
            if (count + word.Length >= splitNameLimit && count > indentString.Length)
            {
                stringBuilder.Append('\n');
                stringBuilder.Append(indentString);
                count = indentString.Length;
            }
            
            stringBuilder.Append(word);
            count += word.Length;
        }
        
        if (i != words.Length - 1)
        {
            if (count + 1 >= splitNameLimit) // +1 for space
            {
                stringBuilder.Append('\n');
                stringBuilder.Append(indentString);
                count = indentString.Length;
            }
            else
            {
                stringBuilder.Append(' ');
                count++;
            }
        }
    }
    
    // Add expected value, right aligned
    var remainingSpace = characterCount - count;
    
    if (remainingSpace >= check.expectedValue.Length + 3) // At least a few dots
    {
        // Case 1: Everything fits on one line with dots
        stringBuilder.Append(new string('.', remainingSpace - check.expectedValue.Length));
        stringBuilder.Append(check.expectedValue);
    }
    else
    {
        // Case 2: Not enough space for expected value on current line
        
        // Add dots to fill the current line
        if (remainingSpace > 0)
        {
            stringBuilder.Append(new string('.', remainingSpace));
        }
        
        // Start a new line for the expected value
        stringBuilder.Append('\n');
        stringBuilder.Append(indentString);
        
        // Calculate how many characters fit per line after indentation
        int charsPerLine = characterCount - indentString.Length;
        
        // Right align the expected value on the new line(s)
        for (int i = 0; i < check.expectedValue.Length; i += charsPerLine)
        {
            if (i > 0)
            {
                stringBuilder.Append('\n');
                stringBuilder.Append(indentString);
            }
            
            int chunkLength = Math.Min(charsPerLine, check.expectedValue.Length - i);
            string chunk = check.expectedValue.Substring(i, chunkLength);
            
            // Right align the chunk on the line
            stringBuilder.Append(chunk.PadLeft(charsPerLine));
        }
    }
    
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