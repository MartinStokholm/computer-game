using UnityEngine;


public class KillEnemiesQuest : QuestStep
{
    private int _enemiesKilled = 0;
    private const int EnemiesToKill = 5;
    
    private void OnEnable()
    {
        GameManager.Instance.MiscEvents.OnEnemyKilled += EnemyKilled;
    }

    private void OnDisable()
    {
        GameManager.Instance.MiscEvents.OnEnemyKilled -= EnemyKilled;
    }

    private void EnemyKilled()
    {
        if (_enemiesKilled < EnemiesToKill)
        {
            _enemiesKilled++;
            UpdateState();
        }
        
        if (_enemiesKilled >= EnemiesToKill)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        ChangeState(_enemiesKilled.ToString());
    }

    protected override void SetQuestStepState(string state)
    {
        try
        {
            _enemiesKilled = int.Parse(state);
            UpdateState();
        }
        catch (System.Exception e)
        {
         Debug.LogError("Failed to insert state KillEnemiesQuest : " + e);
        }
    }
}
