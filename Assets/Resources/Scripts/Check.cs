using System;
using System.Text;
using UnityEngine;

public class Check
{
    public string Name;
    public string ExpectedValue;
    public bool Checked;
    public bool Overridden;

    public void MarkOverridden()
    {
        Checked = false;
        Overridden = true;
    }

    public void MarkChecked()
    {
        Checked = true;
        Overridden = false;
    }

    public string Text(int characterCount, int splitNameLimit)
    {
        var stringBuilder = new StringBuilder();
        if (Checked) stringBuilder.Append("<color=green>");
        else if (Overridden) stringBuilder.Append("<color=#3ba4c2>");
        var count = 0;
        var names = Name.Split(' ');
        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            stringBuilder.Append(name);
            count += name.Length;
            if (i != names.Length - 1 && name.Length >= splitNameLimit)
            {
                stringBuilder.Append("\n");
                count = 0;
            }
            else
            {
                stringBuilder.Append(" ");
                count++;
            }
        }

        count += ExpectedValue.Length;
        stringBuilder.Append(new string('.', characterCount - count));
        stringBuilder.Append(ExpectedValue);
        if (Checked || Overridden) stringBuilder.Append("</color>");
        return stringBuilder.ToString();
    }
}