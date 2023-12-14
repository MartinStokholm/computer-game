using System;
using UnityEngine;

public class InputEvents : MonoBehaviour
{
    public event Action OnSubmitPressed;
    public void SubmitPressed()
    {
        OnSubmitPressed?.Invoke();
    }

    public event Action OnQuestLogTogglePressed;
    public void QuestLogTogglePressed()
    {
        OnQuestLogTogglePressed?.Invoke();
    }
    
    public event Action OnPauseMenuTogglePressed;
    public void PauseMenuTogglePressed()
    {
        OnPauseMenuTogglePressed?.Invoke();
    }
    
    public event Action OnInventoryTogglePressed;
    public void InventoryTogglePressed()
    {
        Debug.Log("Inventory");
        OnInventoryTogglePressed?.Invoke();
    }
}
