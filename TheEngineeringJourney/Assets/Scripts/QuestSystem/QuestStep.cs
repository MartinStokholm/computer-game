using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool _isFinished = false;
    private string _questId;
    private int _stepIndex;

    public void InitializeQuestStep(string questId, int stepIndex, string questStepState)
    {
        _questId = questId;
        _stepIndex = stepIndex;
        if (!string.IsNullOrEmpty(questStepState))
        {
            SetQuestStepState(questStepState);
        }
    }

    protected void FinishQuestStep()
    {
        Debug.Log($"FinishQuestStep{_isFinished}");
        if (_isFinished) return;
        
        GameManager.Instance.QuestEvents.AdvanceQuest(_questId);
        _isFinished = true;
        //Destroy(gameObject);
    }
    
    protected void ChangeState(string newState)
    {
        GameManager.Instance.QuestEvents.QuestStepStateChange(_questId, _stepIndex, new QuestStepState(newState));
    }

    protected abstract void SetQuestStepState(string state);
}
