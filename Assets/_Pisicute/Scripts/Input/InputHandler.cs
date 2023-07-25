using UnityEngine;

public class InputHandler : ScriptableObject
{
    protected static InputControlSchemes inputControlSchemes;

    protected virtual void OnEnable()
    {
        if (inputControlSchemes == null)
        {
            inputControlSchemes = new InputControlSchemes();
            inputControlSchemes.Enable();
        }
    }
}
