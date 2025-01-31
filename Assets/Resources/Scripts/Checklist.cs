using System.Collections.Generic;
using UnityEngine.Events;

public class Checklist
{
    public readonly List<Check> Checks = new();
    public string Name;
    public UnityAction<int, bool> OnCheckChecked;
    private int _checkSelectedIndex = -1;

    public bool IsDone()
    {
        return Checks.TrueForAll(check => check.IsDone);
    }

    public void OverrideChecklist()
    {
        Checks.ForEach(check => check.TriggerOverride());
    }

    public void OnCheckSelect(int index)
    {
        if (_checkSelectedIndex >= 0 && _checkSelectedIndex < Checks.Count)
        {
            Checks[_checkSelectedIndex].TriggerSelect(false);
        }

        _checkSelectedIndex = index;
        if (index >= 0 && index < Checks.Count)
        {
            Checks[index].TriggerSelect(true);
        }
    }

    public void OverrideCheck()
    {
        if (_checkSelectedIndex >= 0 && _checkSelectedIndex < Checks.Count)
        {
            Checks[_checkSelectedIndex].TriggerOverride();
        }
    }

    public void Reset()
    {
        foreach (var check in Checks)
        {
            check.TriggerReset();
        }
    }

    private void CheckChecked(int index, bool checkedValue)
    {
        OnCheckChecked?.Invoke(index, checkedValue);
    }

    public void AddCheck(Check check)
    {
        check.OnCheckChecked += CheckChecked;
        Checks.Add(check);
    }
}