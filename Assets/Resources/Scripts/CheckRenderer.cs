using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckRenderer : MonoBehaviour
{
    public Check Check;
    public Image checkmarkImage;
    public Image checkmarkBackgroundImage;
    public TMP_Text checkTextComponent;
    public Outline outline;
    private const int SplitNameLimit = 10;
    private const int CharacterCount = 48;

    void Start()
    {
        checkTextComponent.text = Check.Text(CharacterCount, SplitNameLimit);

        var textButton = gameObject.GetComponent<Button>();
        textButton.onClick.AddListener(() =>
        {
            Debug.Log("Check selected");
            transform.parent.parent.GetComponent<ChecklistRenderer>().OnCheckSelect(Check.Index);
        });

        if (!Check.isAutomatic)
        {
            checkmarkBackgroundImage.GetComponent<Button>().onClick.AddListener(() => { Check.TriggerCheck(); });
        }
        else
        {
            checkmarkBackgroundImage.enabled = false;
        }

        Check.OnCheckDataChanged += UpdateUI;
        UpdateUI();
    }

    private void UpdateUI()
    {
        checkTextComponent.text = Check.Text(CharacterCount, SplitNameLimit);
        SetColors();
        checkmarkImage.enabled = Check.Checked;
        outline.enabled = Check.IsSelected;
    }

    private void SetColors()
    {
        // checkmarkImage.color = color; ne postoje plave kvacice?
        if (Check.IsNote)
            return;
        checkTextComponent.color = Check.Overridden ? new Color(.23f, .64f, .76f, 1) : Check.Checked ? Color.green : Color.white;
        checkmarkBackgroundImage.color = Check.Overridden ? new Color(0f, 0f, 0f, 1f) : new Color(1f, 1f, 1f, .5f);
        checkmarkBackgroundImage.GetComponent<Outline>().enabled = Check.Overridden;
    }

    private void OnDestroy()
    {
        Check.OnCheckDataChanged -= UpdateUI;
    }
}