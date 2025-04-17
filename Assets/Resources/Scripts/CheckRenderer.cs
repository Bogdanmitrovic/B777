using System;
using System.Collections.Generic;
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
        checkmarkImage.enabled = (check.Checked || check.Overridden ) && !check.IsNote && !check.expectedValue.Contains(":") && !check.expectedValue.Contains("--") && !check.expectedValue.Contains("==>");
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
    if (check.IsNote) return $"NOTE: {check.expectedValue}";
    if (check.IsPlainText)
    {
        return WrapPlainText(check.expectedValue, characterCount);
    }    
    // Split name using constant limit
    List<string> nameLines = SplitTextIntoLines(check.name, splitNameLimit);
    
    // Split expected value with progressively increasing limits
    List<string> valueLines = SplitExpectedValueWithProgressiveLimits(check.expectedValue, splitNameLimit);
    
    StringBuilder checkText = new StringBuilder();
    
    // First line: name + dots + value
    string firstLineName = nameLines.Count > 0 ? nameLines[0] : "";
    string firstLineValue = valueLines.Count > 0 ? valueLines[0] : "";
    string dots = GenerateDots(characterCount, firstLineName, firstLineValue);
    checkText.Append(firstLineName + dots + firstLineValue);
    
    // Handle remaining lines by combining them or right-aligning as needed
    int maxLines = Math.Max(nameLines.Count, valueLines.Count);
    for (int i = 1; i < maxLines; i++)
    {
        checkText.Append("\n");
        
        string nameLine = i < nameLines.Count ? nameLines[i] : "";
        string valueLine = i < valueLines.Count ? valueLines[i] : "";
        
        if (!string.IsNullOrEmpty(nameLine) && !string.IsNullOrEmpty(valueLine))
        {
            // Both name and value have content on this line - combine them with dots
            string lineDots = GenerateSpaces(characterCount, nameLine, valueLine);
            checkText.Append(nameLine + lineDots + valueLine);
        }
        else if (!string.IsNullOrEmpty(nameLine))
        {
            // Only name has content
            checkText.Append(nameLine);
        }
        else if (!string.IsNullOrEmpty(valueLine))
        {
            // Only value has content - right align it
            int spacesNeeded = characterCount - valueLine.Length;
            checkText.Append(new string(' ', spacesNeeded) + valueLine);
        }
    }
    
    return checkText.ToString();
}

private List<string> SplitTextIntoLines(string text, int splitLimit)
{
    List<string> lines = new List<string>();
    if (string.IsNullOrEmpty(text)) return lines;
    
    StringBuilder currentLine = new StringBuilder();
    int currentLineLength = 0;
    string[] words = text.Split(' ');
    
    foreach (string word in words)
    {
        // Check if adding this word would exceed the line limit
        if (currentLineLength + word.Length + (currentLineLength > 0 ? 1 : 0) > splitLimit && currentLineLength > 0)
        {
            // Add current line to results and start a new line
            lines.Add(currentLine.ToString());
            currentLine.Clear();
            currentLineLength = 0;
        }
        
        // Add space if not at the beginning of a line
        if (currentLineLength > 0)
        {
            currentLine.Append(' ');
            currentLineLength++;
        }
        
        currentLine.Append(word);
        currentLineLength += word.Length;
    }
    
    // Add the last line if there's anything in it
    if (currentLine.Length > 0)
    {
        lines.Add(currentLine.ToString());
    }
    
    return lines;
}

private List<string> SplitExpectedValueWithProgressiveLimits(string text, int baseLimit)
{
    List<string> lines = new List<string>();
    if (string.IsNullOrEmpty(text)) return lines;
    
    StringBuilder currentLine = new StringBuilder();
    int currentLineLength = 0;
    string[] words = text.Split(' ');
    int currentLineIndex = 0;
    int currentLineLimit = baseLimit;
    
    foreach (string word in words)
    {
        // Check if adding this word would exceed the current line's limit
        if (currentLineLength + word.Length + (currentLineLength > 0 ? 1 : 0) > currentLineLimit && currentLineLength > 0)
        {
            // Add current line to results and start a new line
            lines.Add(currentLine.ToString());
            currentLine.Clear();
            currentLineLength = 0;
            
            // Double the limit for the next line
            currentLineIndex++;
            currentLineLimit = baseLimit * (int)Math.Pow(2, currentLineIndex);
        }
        
        // Add space if not at the beginning of a line
        if (currentLineLength > 0)
        {
            currentLine.Append(' ');
            currentLineLength++;
        }
        
        currentLine.Append(word);
        currentLineLength += word.Length;
    }
    
    // Add the last line if there's anything in it
    if (currentLine.Length > 0)
    {
        lines.Add(currentLine.ToString());
    }
    
    return lines;
}

private string GenerateDots(int totalLength, string leftText, string rightText)
{
    int leftLength = leftText.Length;
    int rightLength = rightText.Length;
    int dotsNeeded = totalLength - leftLength - rightLength;
    
    // Ensure we have at least one dot
    dotsNeeded = Math.Max(1, dotsNeeded);
    
    return new string('.', dotsNeeded);
}
private string GenerateSpaces(int totalLength, string leftText, string rightText)
{
    int leftLength = leftText.Length;
    int rightLength = rightText.Length;
    int dotsNeeded = totalLength - leftLength - rightLength;
    
    // Ensure we have at least one dot
    dotsNeeded = Math.Max(1, dotsNeeded);
    
    return new string(' ', dotsNeeded);
}
private string WrapPlainText(string text, int lineLength)
{
    if (string.IsNullOrEmpty(text)) return string.Empty;
    
    StringBuilder result = new StringBuilder();
    int currentLineLength = 0;
    string[] words = text.Split(' ');
    
    foreach (string word in words)
    {
        // If adding this word would exceed the line length
        if (currentLineLength + word.Length + (currentLineLength > 0 ? 1 : 0) > lineLength && currentLineLength > 0)
        {
            // Start a new line
            result.Append('\n');
            currentLineLength = 0;
        }
        
        // Add space if not at beginning of line
        if (currentLineLength > 0)
        {
            result.Append(' ');
            currentLineLength++;
        }
        
        result.Append(word);
        currentLineLength += word.Length;
    }
    
    return result.ToString();
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