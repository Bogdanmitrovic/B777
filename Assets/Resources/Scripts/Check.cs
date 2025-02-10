using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Check
{
    public string name;
    public string expectedValue;
    public List<Check> expectedYes;
    public List<Check> expectedNo;
    public bool isAutomatic;
    [CanBeNull] public List<string> conditionalChecksYes;
    [CanBeNull] public List<string> conditionalChecksNo;
    [NonSerialized] public bool Checked;
    [NonSerialized] public bool Overridden;
    [NonSerialized] public int Index;
    [NonSerialized] public bool IsSelected;

    public UnityAction OnCheckDataChanged;
    public UnityAction<int, bool> OnCheckChecked;
    public UnityAction<int> OnCheckSelected;
    public UnityAction<int, ConditionalState> OnConditionalCheck;

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
    public bool IsConditional => conditionalChecksYes != null || conditionalChecksNo != null;

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
        if (selected) OnCheckSelected?.Invoke(Index);
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