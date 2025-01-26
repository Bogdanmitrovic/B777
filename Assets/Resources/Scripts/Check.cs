using System;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Check : MonoBehaviour
{
    public string Name;
    public string ExpectedValue;
    public bool Checked;
    public bool Overridden;
    public bool isAutomatic;
    private GameObject _checkObject;
    private Image _checkImageComponent;
    private TMP_Text _checkTextComponent;

    // public Check(string name, string expectedValue, bool automatic)
    // {
    //     
    // }
    public void MarkOverridden()
    {
        Checked = false;
        Overridden = true;
    }

    public void MarkChecked()
    {
        Checked = true;
        Overridden = false;
    }

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
    }

    public GameObject GetObj(GameObject checkPrefabManual, GameObject checkListParent, int characterCount, int splitNameLimit, int i)
    {
        _checkObject = Instantiate(checkPrefabManual, checkListParent.transform, false);
        _checkTextComponent = _checkObject.GetComponentInChildren<TMP_Text>();
        _checkImageComponent = _checkObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>();
        
        _checkTextComponent.text = Text(characterCount, splitNameLimit);
        _checkTextComponent.rectTransform.offsetMin = new Vector2(-700, _checkTextComponent.rectTransform.offsetMin.y);
        _checkTextComponent.rectTransform.offsetMax = new Vector2(700, _checkTextComponent.rectTransform.offsetMax.y);
        
        var button = _checkObject.GetComponentInChildren<Button>();
        if (!isAutomatic)
        {
            button.onClick.AddListener(() =>
            {
                TriggerCheck(checkListParent, i, characterCount, splitNameLimit);
            });
            
        }
        else
        {
            //za sad je onclick na sliku checkmarka za override!!!
            button = _checkObject.transform.GetChild(0).GetChild(0).AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                TriggerOverride(characterCount, splitNameLimit);
                
            });
            TriggerCheck(checkListParent, i, characterCount, splitNameLimit);
            _checkObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
        
        Debug.Log(button.onClick);
        return _checkObject;
    }

    private void TriggerCheck(GameObject checklistRendererHolder, int i, int characterCount, int splitNameLimit)
    {
        Checked = !Checked;
        checklistRendererHolder.GetComponent<ChecklistRenderer>().OnCheckboxClick(i, Checked);
        _checkImageComponent.enabled = Checked;
        _checkObject.transform.GetComponent<Image>().enabled = Checked;
        
        _checkTextComponent.text = Text(characterCount, splitNameLimit);
    }

    private void TriggerOverride( int characterCount, int splitNameLimit)
    {
        Checked = false;
        Overridden = true;

        _checkTextComponent.text = Text(characterCount, splitNameLimit);
        _checkImageComponent.color = new Color(.23f, .64f, .76f, 1);
    }

    private void TriggerSelect()
    {
        
    }
}