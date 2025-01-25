using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Check : MonoBehaviour
{
    public string Name;
    public string ExpectedValue;
    public bool Checked;
    public bool Overridden;
    public bool isManual;

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
        else if (Overridden) stringBuilder.Append("<color=#3ba4c2>");
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

    public GameObject GetObj(GameObject checkPrefabManual, GameObject checkListParent, int characterCount, int splitNameLimit,int i)
    {
        var newObj= Instantiate(checkPrefabManual, checkListParent.transform, false);
        newObj.GetComponentInChildren<TMP_Text>().text = Text(characterCount, splitNameLimit);
        newObj.GetComponentInChildren<TMP_Text>().rectTransform.offsetMin = new Vector2(-700, newObj.GetComponentInChildren<TMP_Text>().rectTransform.offsetMin.y);
        newObj.GetComponentInChildren<TMP_Text>().rectTransform.offsetMax = new Vector2(700, newObj.GetComponentInChildren<TMP_Text>().rectTransform.offsetMax.y);
        var button = newObj.GetComponentInChildren<Button>();
        button.onClick.AddListener(() =>
        {
            TriggerCheck(checkListParent, i, newObj, characterCount, splitNameLimit);
        });
        Debug.Log(button.onClick);
        return newObj;
    }

    private void TriggerCheck(GameObject checklistRendererHolder, int i, GameObject checkObject, int characterCount, int splitNameLimit)
    {
        Checked = !Checked;
        checklistRendererHolder.GetComponent<ChecklistRenderer>().OnCheckboxClick(i, Checked);
        checkObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>().enabled = Checked;
        checkObject.GetComponentInChildren<TMP_Text>().text = Text(characterCount, splitNameLimit);
    }
}