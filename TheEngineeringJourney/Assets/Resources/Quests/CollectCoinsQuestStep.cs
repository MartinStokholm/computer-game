using System;
using UnityEngine;

public class CollectCoinsQuestStep : QuestStep
{
    private int _coinsCollected = 0;
    private const int CoinsToComplete = 5;

    private void OnEnable()
    {
        GameManager.Instance.MiscEvents.OnCoinCollected += CoinsCollected;
    }
    
    private void OnDisable()
    {
        GameManager.Instance.MiscEvents.OnCoinCollected -= CoinsCollected;
    }

    private void CoinsCollected()
    {
        if (_coinsCollected < CoinsToComplete)
        {
            _coinsCollected++;
            UpdateState();
        }
        
        if (_coinsCollected >= CoinsToComplete)
        {
            FinishQuestStep();
        }
    }
    
    private void UpdateState()
    {
        ChangeState(_coinsCollected.ToString());
    }

    protected override void SetQuestStepState(string state)
    {
        try
        {
            _coinsCollected = int.Parse(state);
            UpdateState();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to insert state CollectCoinsQuestStep : " + e);
        }
    }
}
