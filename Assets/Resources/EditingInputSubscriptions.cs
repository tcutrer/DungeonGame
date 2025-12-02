using UnityEngine;
using UnityEngine.InputSystem;

public class EditingInputSubscriptions : MonoBehaviour
{
    public bool ActionInput {get; private set;} = false;
    InputActions _Input = null;

    private void OnEnable()
    {
        _Input = new InputActions();
        _Input.EditingActionMap.Enable();
        _Input.EditingActionMap.PlayerAction.started += setAction;
        _Input.EditingActionMap.PlayerAction.canceled += setAction;
    }
    private void OnDisable()
    {
        _Input.EditingActionMap.PlayerAction.started -= setAction;
        _Input.EditingActionMap.PlayerAction.canceled -= setAction;
        _Input.EditingActionMap.Disable();
    }

    void setAction(InputAction.CallbackContext ctx)
    {
        ActionInput = ctx.started;
    }
}
