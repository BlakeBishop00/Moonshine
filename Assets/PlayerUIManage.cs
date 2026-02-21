using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIManage : MonoBehaviour
{
    public TextMeshProUGUI useButtonTooltip;
    InputAction useButtonAction;
    public InputActionReference useButtonActionReference;
    string useButtonKey; // cache these for less lag

    public float elementDisplayTime = 1;


    private void Awake()
    {
        if (useButtonActionReference != null)
            useButtonAction = useButtonActionReference.action;
        useButtonKey = useButtonAction.GetBindingDisplayString();
    }

    public void UpdateKeys()
    {
        useButtonKey = useButtonAction.GetBindingDisplayString();
    }

    public void ShowUseButtonTooltip()
    {
        if (!useButtonTooltip.gameObject.activeSelf)
        {
            useButtonTooltip.gameObject.SetActive(true);
            useButtonTooltip.text = $"Press '{useButtonKey}' to use.";
            StartCoroutine(DisableElement(useButtonTooltip.gameObject));
        }
    }

    public void HideUseButtonTooltip()
    {
        if (useButtonTooltip.gameObject.activeSelf)
        {
            useButtonTooltip.gameObject.SetActive(false);
        }
    }

    public IEnumerator DisableElement(GameObject element, float multiplier = 1)
    {
        if (element.activeSelf)
            yield return new WaitForSeconds(elementDisplayTime * multiplier);
        else
            yield break;

        element.SetActive(false);
    }
}
