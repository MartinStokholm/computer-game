using UnityEngine;

public class GoldManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] public int startingGold = 0;

    public int currentGold { get; private set; }

    private void Awake()
    {
        currentGold = startingGold;
    }

    private void OnEnable() 
    {
        GameManager.Instance.GoldEvents.OnGoldGained += GoldGained;
    }

    private void OnDisable() 
    {
        GameManager.Instance.GoldEvents.OnGoldGained  -= GoldGained;
    }

    private void Start()
    {
        GameManager.Instance.GoldEvents.GoldChange(currentGold);
    }

    private void GoldGained(int gold) 
    {
        currentGold += gold;
        GameManager.Instance.GoldEvents.GoldChange(currentGold);
    }
}
