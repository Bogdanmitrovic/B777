using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class Checklist
{
    public List<Check> checks = new();
    public string name;
    [NonSerialized]
    public UnityAction<int, bool> OnCheckChecked;
    [NonSerialized]
    private int _checkSelectedIndex = -1;

    public bool IsDone()
    {
        return checks.TrueForAll(check => check.IsDone);
    }

    public void OverrideChecklist()
    {
        checks.ForEach(check => check.TriggerOverride());
    }

    public void OnCheckSelect(int index)
    {
        if (_checkSelectedIndex >= 0 && _checkSelectedIndex < checks.Count)
        {
            checks[_checkSelectedIndex].TriggerSelect(false);
        }

        _checkSelectedIndex = index;
        if (index >= 0 && index < checks.Count)
        {
            checks[index].TriggerSelect(true);
        }
    }

    public void OverrideCheck()
    {
        if (_checkSelectedIndex >= 0 && _checkSelectedIndex < checks.Count)
        {
            checks[_checkSelectedIndex].TriggerOverride();
        }
    }

    public void Reset()
    {
        foreach (var check in checks)
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
        checks.Add(check);
    }

    public void RemoveListeners()
    {
        foreach (var check in checks)
        {
            check.OnCheckChecked -= CheckChecked;
        }
    }
}