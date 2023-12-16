using System;

public class MiscEvents 
{
    public event Action OnCoinCollected;
    
    public event Action OnEnemyKilled;
    
    public event Action OnBossKilled;
    public void CoinCollected()
    {
        OnCoinCollected?.Invoke();
    }
    
    public void EnemyKilled()
    {
        OnEnemyKilled?.Invoke();
    }
    
    public void BossKilled()
    {
        OnBossKilled?.Invoke();
    }

    // public event Action OnGemCollected;
    // public void GemCollected()
    // {
    //     OnGemCollected?.Invoke();
    // }
}
