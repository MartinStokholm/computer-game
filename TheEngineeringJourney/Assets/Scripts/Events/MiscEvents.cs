using System;

public class MiscEvents 
{
    public event Action OnCoinCollected;
    public void CoinCollected()
    {
        OnCoinCollected?.Invoke();
    }

    // public event Action OnGemCollected;
    // public void GemCollected()
    // {
    //     OnGemCollected?.Invoke();
    // }
}
