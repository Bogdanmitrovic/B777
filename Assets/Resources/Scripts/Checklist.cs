using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Checklist
{
    public List<Check> checks = new();
    public string name;
    public List<Checklist> subChecklists = new();
    [NonSerialized] public UnityAction OnCheckChecked;
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
    
    public bool IsDoneWithoutDeferred()
    {
        if (deferredChecks == null || deferredChecks.Count == 0)
            return false;
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
        //Debug.Log("Checklist: " + name + " Check: " + checks[index].name + " Checked: " + checkedValue);
        OnCheckChecked?.Invoke();
        checks.FirstOrDefault(check => check.name == checks[index].name && check.expectedValue == checks[index].expectedValue &&
                              check.Checked != checkedValue)?.TriggerCheck();    

        //checks.FindAll(check => check.name == checks[index    ].name && check.Checked != checkedValue)
        //.ForEach(check => check.TriggerCheck());
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
        OnCheckChecked?.Invoke();
        if (conditionalChecksYes != null)
            foreach (var check in conditionalChecksYes)
            {
                Check first = null;
                for (var i = index + 1; i < checks.Count && first == null; i++)
                    if(checks[i].name == check)
                        first = checks[i];
                if (first == null) continue;
                if (state == ConditionalState.No)
                    foreach (var ch in checks.Where(ch =>
                                 ch.name == first.name && ch.expectedValue == first.expectedValue))
                        ch.TriggerOverride();
                else
                    foreach (var ch in checks.Where(ch =>
                                 ch.name == first.name && ch.expectedValue == first.expectedValue))
                        ch.TriggerReset();
            }

        if (conditionalChecksNo != null)
            foreach (var check in conditionalChecksNo)
            {
                Check first = null;
                for (var i = index + 1; i < checks.Count && first == null; i++)
                    if(checks[i].name == check)
                        first = checks[i];
                if (first == null) continue;
                if (state == ConditionalState.Yes)
                    foreach (var ch in checks.Where(ch =>
                                 ch.name == first.name && ch.expectedValue == first.expectedValue))
                        ch.TriggerOverride();
                else
                    foreach (var ch in checks.Where(ch =>
                                 ch.name == first.name && ch.expectedValue == first.expectedValue))
                        ch.TriggerReset();
            }
        var sameNameChecks = checks.FindAll(check => check.name == checks[index].name && check.expectedValue == checks[index].expectedValue);
        var sameNameDifferentChecked = sameNameChecks.FindAll(check => check.ConditionalState != state);
        sameNameDifferentChecked.ForEach(check => check.TriggerConditionCheck(state));
    }
}