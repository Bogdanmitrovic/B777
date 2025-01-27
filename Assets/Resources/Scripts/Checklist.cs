using System.Collections.Generic;
using UnityEngine;

public class Checklist
{
    public readonly List<Check> Checks = new List<Check>();
    public string Name;
    //private List<GameObject> _checkObjects;
    private int _checkSelectedIndex = -1;

    public bool IsDone()
    {
        return Checks.TrueForAll(check => check.IsDone);
    }
    public void OverrideChecklist()
    {
        Checks.ForEach(check => check.TriggerOverride());
    }
    public void Load(GameObject checkPrefab, GameObject checkListParent, int characterCount, int splitNameLimit)
    {
        //_checkObjects = new List<GameObject>();
        _checkSelectedIndex = -1;
        for (var i = 0; i < Checks.Count; i++)
        {
            var check = Checks[i];
            var newObj = check.GetObj(checkPrefab, checkListParent, characterCount, splitNameLimit, i);
            //_checkObjects.Add(newObj);
        }
    }
    public void Unload()
    {
        foreach (var check in Checks)
        {
            check.DestroyCheck();
        }
    }

    public void OnCheckSelect(int index)
    {
        if (_checkSelectedIndex >= 0 && _checkSelectedIndex < Checks.Count)
        {
            Checks[_checkSelectedIndex].TriggerSelect(false);
        }
        _checkSelectedIndex = index;
        if(index >= 0 && index<Checks.Count)
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
}
