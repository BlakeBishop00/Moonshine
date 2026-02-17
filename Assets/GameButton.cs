using UnityEngine;
using UnityEngine.InputSystem;

public class GameButton : MonoBehaviour // Renamed some assets had script with same name
{
    public bool triggeredByEnter = false;

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
            if (action != null && action.IsPressed())
            {
                triggered = true;
                if (resets)
                {
                    Invoke("ResetTrigger", Mathf.Max(resetTimer, 0.02f)); // I don't think invoke is the best? I don't really care it should be fine
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == triggerTag)
        {
            if (!triggeredByEnter)
            {
                inside = true;
                if (playerUIManager != null)
                {
                    playerUIManager.ShowUseButtonTooltip();
                }
                else
                {
                    other.gameObject.GetComponent<PlayerUIManage>().ShowUseButtonTooltip();
                }
            }
            else
            {
                triggered = true;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {

        if (other.tag == triggerTag)
        {
            if (!triggeredByEnter)
            {
                inside = false;
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
            else
            {
                triggered = false; // reset by default
            }
        }

    }

    void ResetTrigger()
    {
        triggered = false;
    }
}
