using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Check : MonoBehaviour
{
    // data
    private readonly string _name;
    private readonly string _expectedValue;
    private bool _checked;
    private bool _overridden;
    private readonly bool _isAutomatic;
    // gameobjects & components
    private GameObject _checkObject;
    private Image _checkmarkImage;
    private Image _checkmarkBackgroundImage;
    private TMP_Text _checkTextComponent;
    private Outline _outline;
    private int _characterCount;
    private int _splitNameLimit;

    public Check(string name, string expectedValue, bool isAutomatic)
    {
        _name = name;
        _expectedValue = expectedValue;
        _isAutomatic = isAutomatic;
        _checked = false;
        _overridden = false;
    }

    public bool IsDone => _checked || _overridden;
    private string Text()
    {
        var stringBuilder = new StringBuilder();
        if (_checked) stringBuilder.Append("<color=green>");
        else if (_overridden) stringBuilder = new StringBuilder().Append("<color=#3ba4c2>");
        var count = 0;
        var names = _name.Split(' ');
        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            stringBuilder.Append(name);
            count += name.Length;
            if (i != names.Length - 1 && count >= _splitNameLimit)
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

        count += _expectedValue.Length;
        stringBuilder.Append(new string('.', _characterCount - count));
        stringBuilder.Append(_expectedValue);
        if (_checked || _overridden) stringBuilder.Append("</color>");
        return stringBuilder.ToString();
    }

    public GameObject GetObj(GameObject checkPrefabManual, GameObject checkListParent, int characterCount, int splitNameLimit, int i)
    {
        _characterCount = characterCount;
        _splitNameLimit = splitNameLimit;
        _checkObject = Instantiate(checkPrefabManual, checkListParent.transform, false);
        _checkTextComponent = _checkObject.GetComponentInChildren<TMP_Text>();
        _checkmarkImage = _checkObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>();
        _outline = _checkObject.GetComponent<Outline>();
        _checkmarkBackgroundImage = _checkObject.transform.GetChild(0).GetComponent<Image>();
        
        _checkTextComponent.text = Text();
        _checkTextComponent.rectTransform.offsetMin = new Vector2(-700, _checkTextComponent.rectTransform.offsetMin.y);
        _checkTextComponent.rectTransform.offsetMax = new Vector2(700, _checkTextComponent.rectTransform.offsetMax.y);

        var textButton = _checkObject.GetComponent<Button>();
        textButton.onClick.AddListener(() =>
        {
            checkListParent.GetComponent<ChecklistRenderer>().OnCheckSelect(i);
        });
        
        var button = _checkmarkBackgroundImage.GetComponent<Button>();
        if (!_isAutomatic)
        {
            button.onClick.AddListener(() =>
            {
                TriggerCheck(checkListParent, i, characterCount, splitNameLimit);
            });
            
        }
        else
        {
            _checkmarkBackgroundImage.enabled = false;
        }
        
        Debug.Log(button.onClick);
        return _checkObject;
    }

    private void TriggerCheck(GameObject checklistRendererHolder, int i, int characterCount, int splitNameLimit)
    {
        if (_overridden || _isAutomatic) return;
        
        _checked = !_checked;
        checklistRendererHolder.GetComponent<ChecklistRenderer>().OnCheckboxCheck(i, _checked);
        _checkmarkImage.enabled = _checked;
        //_checkObject.transform.GetComponent<Image>().enabled = _checked;
        
        _checkTextComponent.text = Text();
    }

    public void TriggerOverride()
    {
        _checked = false;
        _overridden = true;

        _checkTextComponent.text = Text();
        _checkmarkImage.color = new Color(.23f, .64f, .76f, 1);
    }

    public void TriggerSelect(bool selected)
    {
        _outline.enabled = selected;
    }

    public void TriggerReset()
    {
        // reset data
        _checked = false;
        _overridden = false;
        // reset visuals
        _checkmarkImage.enabled = false;
        _checkmarkImage.color = new Color(0, 1, 0, 1);
        _checkmarkBackgroundImage.enabled = !_isAutomatic;
        _outline.enabled = false;
        _checkTextComponent.text = Text();

    }
    public void DestroyCheck()
    {
        Destroy(_checkObject);
    }

    public bool IsChecked()
    {
        return _checked;
    }
}