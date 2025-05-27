using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Events;

[Serializable]
public class Check
{
    public string name;
    public string expectedValue;
    public bool isAutomatic;
    [CanBeNull] public List<string> conditionalChecksYes;
    [CanBeNull] public List<string> conditionalChecksNo;
    [NonSerialized] public bool Checked;
    [NonSerialized] public bool Overridden;
    [NonSerialized] public int Index;
    [NonSerialized] public bool IsSelected;
    [NonSerialized] public bool Rendered = false;
    [NonSerialized] public ConditionalState ConditionalState;

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
        ConditionalState = ConditionalState.None;
    }

    public bool IsDone => (IsPageBreak || Rendered) &&
                          (Checked || Overridden || IsNote || IsPageBreak ||
                           (IsConditional && ConditionalState != ConditionalState.None) ||
                           (IsPlainText && (expectedValue.Contains("--") || expectedValue.Contains("Inhibited") || expectedValue.Contains("Objective") || expectedValue.Contains("Condition") || expectedValue.Contains("==>"))));

    public bool IsNote => name.Contains("NOTE");
    public bool IsPlainText => name.Contains("PLAINTEXT");
    public bool IsPageBreak => name.Contains("PageBreak");
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
        Rendered = false;
        Checked = isAutomatic;
        Overridden = false;
        IsSelected = false;
        ConditionalState = ConditionalState.None;
        OnCheckDataChanged?.Invoke();
    }

    public void TriggerCheck()
    {
        if (Overridden || isAutomatic) return;

        Checked = !Checked;
        OnCheckDataChanged?.Invoke();
        OnCheckChecked?.Invoke(Index, Checked);
    }

    public void TriggerConditionCheck(ConditionalState conditionalState)
    {
        ConditionalState = conditionalState;
        OnConditionalCheck?.Invoke(Index, conditionalState);
        OnCheckDataChanged?.Invoke();
    }
}