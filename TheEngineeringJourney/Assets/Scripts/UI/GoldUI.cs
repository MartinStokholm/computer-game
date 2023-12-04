using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI goldText;

    private void OnEnable() 
    {
        GameManager.Instance.GoldEvents.OnGoldChange += GoldChange;
    }

    private void OnDisable() 
    {
        GameManager.Instance.GoldEvents.OnGoldChange -= GoldChange;
    }

    private void GoldChange(int gold) 
    {
        goldText.text = gold.ToString();
    }
}
