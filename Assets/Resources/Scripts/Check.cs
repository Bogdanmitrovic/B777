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
    public GameObject CheckObject;
    public Image CheckImageComponent;
    public TMP_Text CheckTextComponent;

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
        if (Overridden) stringBuilder = new StringBuilder().Append("<color=#3ba4c2>");
        var count = 0;
        var names = Name.Split(' ');
        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            stringBuilder.Append(name);
            count += name.Length;
            if (i != names.Length - 1 && name.Length >= splitNameLimit)
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
        CheckObject = Instantiate(checkPrefabManual, checkListParent.transform, false);
        CheckTextComponent = CheckObject.GetComponentInChildren<TMP_Text>();
        CheckImageComponent = CheckObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>();
        
        CheckTextComponent.text = Text(characterCount, splitNameLimit);
        CheckTextComponent.rectTransform.offsetMin = new Vector2(-700, CheckTextComponent.rectTransform.offsetMin.y);
        CheckTextComponent.rectTransform.offsetMax = new Vector2(700, CheckTextComponent.rectTransform.offsetMax.y);
        
        var button = CheckObject.GetComponentInChildren<Button>();
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
            button = CheckObject.transform.GetChild(0).GetChild(0).AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                TriggerOverride(characterCount, splitNameLimit);
                
            });
            TriggerCheck(checkListParent, i, characterCount, splitNameLimit);
            CheckObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
        
        Debug.Log(button.onClick);
        return CheckObject;
    }

    private void TriggerCheck(GameObject checklistRendererHolder, int i, int characterCount, int splitNameLimit)
    {
        Checked = !Checked;
        checklistRendererHolder.GetComponent<ChecklistRenderer>().OnCheckboxClick(i, Checked);
        CheckImageComponent.enabled = Checked;
        CheckObject.transform.GetComponent<Image>().enabled = Checked;
        
        CheckTextComponent.text = Text(characterCount, splitNameLimit);
    }

    private void TriggerOverride( int characterCount, int splitNameLimit)
    {
        Overridden = true;

        CheckTextComponent.text = Text(characterCount, splitNameLimit);
        CheckImageComponent.color = new Color(.23f, .64f, .76f, 1);
    }

    private void TriggerSelect()
    {
        
    }
}