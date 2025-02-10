using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
public class Checklist
{
    public List<Check> checks = new();
    public string name;
    [NonSerialized] public UnityAction<int, bool> OnCheckChecked;
    [NonSerialized] private int _checkSelectedIndex = -1;

    public bool IsOverridden => checks.TrueForAll(check => check.Overridden);

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
        if (index == _checkSelectedIndex)
        {
            return;
        }

        if (_checkSelectedIndex >= 0 && _checkSelectedIndex < checks.Count)
        {
            checks[_checkSelectedIndex].TriggerSelect(false);
        }

        _checkSelectedIndex = index;
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
        Debug.Log("Checklist: " + name + " Check: " + checks[index].name + " Checked: " + checkedValue);
        OnCheckChecked?.Invoke(index, checkedValue);
    }

    public void SetListeners()
    {
        foreach (var check in checks)
        {
            check.OnCheckChecked += CheckChecked;
            check.OnConditionalCheck += CheckConditional;
            check.OnCheckSelected += OnCheckSelect;
        }
    }

    private void CheckConditional(int index, ConditionalState state)
    {
        Debug.Log("Conditional check " + index + " is " + state);
        switch (state)
        {
            case ConditionalState.No:
            {
                var conditionalChecksYes = checks[index].conditionalChecksYes;
                if (conditionalChecksYes != null)
                    foreach (var check in conditionalChecksYes)
                    {
                        checks.First(ch => ch.name==check).TriggerOverride();
                    }
                break;
            }
            case ConditionalState.Yes:
            {
                var conditionalChecksYes = checks[index].conditionalChecksYes;
                if (conditionalChecksYes != null)
                    foreach (var check in conditionalChecksYes)
                    {
                        checks.First(ch => ch.name==check).TriggerReset();
                    }
                break;
            }
        }
    }
}