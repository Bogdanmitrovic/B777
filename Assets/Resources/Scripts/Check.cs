using System;
using System.Text;
using UnityEngine.Events;

[Serializable]
public class Check
{
    public string name;
    public string expectedValue;
    public bool isAutomatic;
    [NonSerialized]
    public bool Checked;
    [NonSerialized]
    public bool Overridden;
    [NonSerialized]
    public int Index;
    [NonSerialized]
    public bool IsSelected;
    
    public UnityAction OnCheckDataChanged;
    public UnityAction<int, bool> OnCheckChecked;

    public Check(string name, string expectedValue, bool isAutomatic, int i)
    {
        this.name = name;
        this.expectedValue = expectedValue;
        this.isAutomatic = isAutomatic;
        Checked = isAutomatic;
        Overridden = false;
        Index = i;
        IsSelected = false;
    }

    public bool IsDone => Checked || Overridden || IsNote;
    public bool IsNote => name == "NOTE";
    public string Text(int characterCount, int splitNameLimit)
    {
        if (IsNote) return name + " " + expectedValue;
        var stringBuilder = new StringBuilder();
        var count = 0;
        var names = name.Split(' ');
        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            stringBuilder.Append(name);
            count += name.Length;
            if (i != names.Length - 1 && count >= splitNameLimit)
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

        count += expectedValue.Length;
        stringBuilder.Append(new string('.', characterCount - count));
        stringBuilder.Append(expectedValue);
        return stringBuilder.ToString();
        // TODO boja da ide u CheckRenderer preko .color ili sta vec, ne kroz tekst sa <color> i </color>
    }

    public void TriggerOverride()
    {
        if (IsNote) return;
        Checked = false;
        Overridden = true;
        OnCheckDataChanged?.Invoke();
    }

    public void TriggerSelect(bool selected)
    {
        if (IsNote) return;
        IsSelected = selected;
        OnCheckDataChanged?.Invoke();
    }

    public void TriggerReset()
    {
        Checked = isAutomatic;
        Overridden = false;
        IsSelected = false;
        OnCheckDataChanged?.Invoke();
    }

    public void TriggerCheck()
    {
        if (Overridden || isAutomatic) return;
        
        Checked = !Checked;
        OnCheckDataChanged?.Invoke();
        OnCheckChecked?.Invoke(Index, Checked);
    }
}