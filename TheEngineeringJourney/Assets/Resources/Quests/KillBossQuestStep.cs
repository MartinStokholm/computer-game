using System;
using UnityEngine;
    
public class KillBossQuestStep : QuestStep
{
    
    private int _bossKilled = 0;
    private const int BossToKill = 1;
    
    private void OnEnable()
    {
        GameManager.Instance.MiscEvents.OnBossKilled += BossKilled;
    }

    private void OnDisable()
    {
        GameManager.Instance.MiscEvents.OnBossKilled -= BossKilled;
    }

    private void BossKilled()
    {
        if(_bossKilled < BossToKill)
        {
            _bossKilled++;
            UpdateState();
        }
        
        if (_bossKilled >= BossToKill)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        ChangeState(_bossKilled.ToString());
    }
    
    protected override void SetQuestStepState(string state)
    {
        throw new System.NotImplementedException();
    }
}
