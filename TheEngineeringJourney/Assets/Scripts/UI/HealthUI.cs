using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    public List<GameObject> healthHeartsList = new();

    private void OnEnable()
    {
        GameManager.Instance.Player.HealthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.Player.HealthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        SetHealthBar(healthEventArgs);
    }

    private void ClearHealthBar()
    {
        foreach (var heartIcon in healthHeartsList)
        {
            Destroy(heartIcon);
        }

        healthHeartsList.Clear();
    }

    private void SetHealthBar(HealthEventArgs healthEventArgs)
    {
        ClearHealthBar();

        // Instantiate heart image prefabs
        var healthHearts = Mathf.CeilToInt(healthEventArgs.HealthPercent * 100f / 20f);

        for (var i = 0; i < healthHearts; i++)
        {
            var heart = Instantiate(GameResources.Instance.HeartPrefab, transform);
            
            heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(32f * i, 0f);

            healthHeartsList.Add(heart);
        }

    }
}
