using UnityEngine;


public class InputManager : MonoBehaviour
{
    public void SubmitPressed()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Debug.Log("Interact");
            GameManager.Instance.InputEvents.SubmitPressed();
        }
    }

    public void QuestLogTogglePressed()
    {
        GameManager.Instance.InputEvents.QuestLogTogglePressed();
    }
}
