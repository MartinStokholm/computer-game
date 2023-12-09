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
}


// public static class StaticInputsEvent
// {
//     public static event Action OnSubmitPressed;
//
//     public static void OnSubmitEvent()
//     {
//         Debug.Log("Interact using static");
//         OnSubmitPressed?.Invoke();
//     }
// }
