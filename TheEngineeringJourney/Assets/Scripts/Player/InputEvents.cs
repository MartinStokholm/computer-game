using System;
using System.Collections;
using System.Collections.Generic;
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
