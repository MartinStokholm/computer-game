using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Coin : MonoBehaviour
{
    [Header("Config")] 
    [SerializeField] private float _respawnTimeSeconds = 8;
    [SerializeField] private int _goldGained;

    private CircleCollider2D _circleCollider;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    private void OnTriggerEnter2D(Collider2D otherCollider) 
    {
        if (otherCollider.CompareTag(Settings.PlayerTag))
        {
            Debug.Log("Collected coin");
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        _circleCollider.enabled = false;
        _spriteRenderer.gameObject.SetActive(false);
        GameManager.Instance.GoldEvents.GoldGained(_goldGained);
        GameManager.Instance.MiscEvents.CoinCollected();
        StopAllCoroutines();
    }
    
    private IEnumerator RespawnAfterTime()
    {
        yield return new WaitForSeconds(_respawnTimeSeconds);
        _circleCollider.enabled = true;
        _spriteRenderer.gameObject.SetActive(true);
    }


}
