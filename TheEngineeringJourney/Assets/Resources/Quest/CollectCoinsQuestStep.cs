public class CollectCoinsQuestStep : QuestStep
{
    private int _coinsCollected = 0;
    private const int _coinsToComplete = 5;

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
        if (_coinsCollected < _coinsToComplete)
        {
            _coinsCollected++;
        }

        if (_coinsCollected >= _coinsToComplete)
        {
            FinishQuestStep();
        }
    }
}
