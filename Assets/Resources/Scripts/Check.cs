using System.Text;
using UnityEngine.Events;
public class Check
{
    public readonly string Name;
    public readonly string ExpectedValue;
    public bool Checked;
    public bool Overridden;
    public readonly bool IsAutomatic;
    public readonly int Index;
    public bool IsSelected;
    
    public UnityAction OnCheckDataChanged;
    public UnityAction<int, bool> OnCheckChecked;
    // TODO vidi jel ovo iznad treba ovako da se zove ili ne

    public Check(string name, string expectedValue, bool isAutomatic, int i)
    {
        Name = name;
        ExpectedValue = expectedValue;
        IsAutomatic = isAutomatic;
        Checked = false;
        Overridden = false;
        Index = i;
        IsSelected = false;
    }

    public bool IsDone => Checked || Overridden;
    public string Text(int characterCount, int splitNameLimit)
    {
        var stringBuilder = new StringBuilder();
        if (Checked) stringBuilder.Append("<color=green>");
        else if (Overridden) stringBuilder = new StringBuilder().Append("<color=#3ba4c2>");
        var count = 0;
        var names = Name.Split(' ');
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

        count += ExpectedValue.Length;
        stringBuilder.Append(new string('.', characterCount - count));
        stringBuilder.Append(ExpectedValue);
        if (Checked || Overridden) stringBuilder.Append("</color>");
        return stringBuilder.ToString();
        // TODO boja da ide u CheckRenderer preko .color ili sta vec, ne kroz tekst sa <color> i </color>
    }

    public void TriggerOverride()
    {
        Checked = false;
        Overridden = true;
        OnCheckDataChanged?.Invoke();
    }

    public void TriggerSelect(bool selected)
    {
        IsSelected = selected;
        OnCheckDataChanged?.Invoke();
    }

    public void TriggerReset()
    {
        Checked = false;
        Overridden = false;
        IsSelected = false;
        OnCheckDataChanged?.Invoke();
    }

    public void TriggerCheck()
    {
        if (Overridden || IsAutomatic) return;
        
        Checked = !Checked;
        OnCheckDataChanged?.Invoke();
        OnCheckChecked?.Invoke(Index, Checked);
        //checklistRendererHolder.GetComponent<ChecklistRenderer>().OnCheckboxCheck(i, Check.Checked);
    }
}