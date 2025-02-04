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
            checkmarkBackgroundImage.GetComponent<Button>().onClick.AddListener(() =>
            {
                Check.TriggerCheck();
            });
            
        }
        else
        {
            checkmarkBackgroundImage.enabled = false;
        }
        Check.OnCheckDataChanged += UpdateUI;
    }

    private void UpdateUI()
    {
        if (checkmarkImage == null)
        {
            Check.OnCheckDataChanged -= UpdateUI;
            return;
        }
        checkTextComponent.text = Check.Text(CharacterCount, SplitNameLimit);
        checkmarkImage.color = Check.Overridden ? new Color(.23f, .64f, .76f, 1) : Color.green;
        checkmarkImage.enabled = Check.Checked || Check.Overridden;
        outline.enabled = Check.IsSelected;
    }
    
}
