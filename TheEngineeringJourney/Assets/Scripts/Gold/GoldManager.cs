using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int startingGold = 5;

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
