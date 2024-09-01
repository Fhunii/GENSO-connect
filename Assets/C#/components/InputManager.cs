using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public EventSystem eventSystem;

    public void DisableInput()
    {
        if (eventSystem != null)
        {
            eventSystem.enabled = false;
        }
    }

    public void EnableInput()
    {
        if (eventSystem != null)
        {
            eventSystem.enabled = true;
        }
    }
}
