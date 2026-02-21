using UnityEngine;
using UnityEngine.InputSystem;

public class GameButton : MonoBehaviour // Renamed some assets had script with same name
{
    public bool triggeredByEnter = false;
    public bool exitTurnOffTrigger = true; // renamed to update all objects using this auto

    public string triggerTag = "Player";
    InputAction action;
    public InputActionReference actionReference;
    
    public bool triggered = false;
    public bool resets = false;
    public float resetTimer = 0;
    public bool removeTextInstantly = true;

    [Header("Because it lags")]
    public PlayerUIManage playerUIManager;

    void OnEnable()
    {
        if (actionReference != null)
        {
            action = actionReference.action;

            action.Enable();
        }
        if(triggered && resets)
        {
            triggered = false;
        }
    }

    void OnDisable()
    {
        if(action != null)
            action.Disable();
    }

    bool inside;

    void Update()
    {
        if (!triggered && inside)
        {
            triggered = true;

            if (resets && !exitTurnOffTrigger && action != null && action.IsPressed())
            { 
                    Invoke("ResetTrigger", Mathf.Max(resetTimer, 0.02f)); // I don't think invoke is the best? I don't really care it should be fine
                
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == triggerTag)
        {

                inside = true;
            if (!triggeredByEnter)
            {
                if (playerUIManager != null)
                {
                    playerUIManager.ShowUseButtonTooltip();
                }
                else
                {
                    other.gameObject.GetComponent<PlayerUIManage>().ShowUseButtonTooltip();
                }
            }

        }
    }


    private void OnTriggerExit(Collider other)
    {

        if (other.tag == triggerTag)
        {
            inside = false;
            if (!triggeredByEnter)
            {
                if (removeTextInstantly)
                {
                    if (playerUIManager != null)
                    {
                        playerUIManager.HideUseButtonTooltip();
                    }
                    else
                    {
                        other.gameObject.GetComponent<PlayerUIManage>().HideUseButtonTooltip();
                    }
                }
            }

            if (exitTurnOffTrigger) // wait time because iosjfguiosdfgsdfguioaweiofsdjfg
                Invoke("ResetTrigger", Mathf.Max(resetTimer, 0.02f)); // I don't think invoke is the best? I don't really care it should be fine

        }

    }

    void ResetTrigger()
    {
        triggered = false;
    }
}
