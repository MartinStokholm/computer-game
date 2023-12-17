using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : SingletonMonobehaviour<InputManager>
{
    private Vector2 _moveDirection = Vector2.zero;
    private bool _interactPressed = false;
    private bool _submitPressed = false;
    
    private static InputManager instance;
    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _submitPressed = true;
        }
        else if (context.canceled)
        {
            _submitPressed = false;
        }
    }
    
    // private void Awake()
    // {
    //     if (instance != null)
    //     {
    //         Debug.LogError("Found more than one Input Manager in the scene.");
    //     }
    //     instance = this;
    // }
    
    
    // public void SubmitPressed(InputAction.CallbackContext context)
    // {
    //     if (context.started)
    //     {
    //         GameManager.Instance.InputEvents.SubmitPressed();
    //     }
    // }

    public void InventoryTogglePressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.InputEvents.InventoryTogglePressed();
        }
    }
    
    public void QuestLogTogglePressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.InputEvents.QuestLogTogglePressed();
        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.InputEvents.SubmitPressed();
        }
    }
    
    public void ExitDialogButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            DialogueManager.GetInstance().ExitDialog();
        }
    }
    
    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            _moveDirection = context.ReadValue<Vector2>();
        } 
    }
    
    public void PauseMenuTogglePressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.InputEvents.QuestLogTogglePressed();
        }
    }

    
    public Vector2 GetMoveDirection() 
    {
        return _moveDirection;
    }
    
    public bool GetInteractPressed() 
    {
        var result = _interactPressed;
        _interactPressed = false;
        return result;
    }

    public bool GetSubmitPressed()
    {
        var result = _submitPressed;
        _submitPressed = false;
        return result;
    }
    
    public void RegisterSubmitPressed() 
    {
        _submitPressed = false;
    }
}
