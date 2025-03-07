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
    public List<Checklist> subChecklists = new();
    [NonSerialized] public UnityAction<int, bool> OnCheckChecked;
    [NonSerialized] private int _checkSelectedIndex = -1;
    public List<string> deferredChecks = new();

    public bool IsMenu => subChecklists.Count > 0;
    public bool IsChecklist => !IsMenu;
    public bool IsOverridden => checks.TrueForAll(check => check.Overridden);

    public bool IsDone()
    {
        if (deferredChecks == null || deferredChecks.Count == 0)
            return checks.TrueForAll(check => check.IsDone);
        var notDone = checks.FindAll(check => !check.IsDone);
        return notDone.All(check => deferredChecks.Contains(check.name));
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
        var conditionalChecksYes = checks[index].conditionalChecksYes;
        var conditionalChecksNo = checks[index].conditionalChecksNo;
        if (conditionalChecksYes != null)
            foreach (var check in conditionalChecksYes)
            {
                if (state == ConditionalState.No)
                    checks.First(ch => ch.name == check).TriggerOverride();
                else
                    checks.First(ch => ch.name == check).TriggerReset();
            }

        if (conditionalChecksNo != null)
            foreach (var check in conditionalChecksNo)
            {
                if (state == ConditionalState.Yes)
                    checks.First(ch => ch.name == check).TriggerOverride();
                else
                    checks.First(ch => ch.name == check).TriggerReset();
            }
    }
}