using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string _itemName;

    [SerializeField] private int _quantity;
    
    [TextArea][SerializeField] private string _description;


    private CircleCollider2D _circleCollider;
    private SpriteRenderer _spriteRenderer;
    private bool _collected;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag(Settings.PlayerTag))
        {
            var leftOverItems = InventoryManager.Instance.AddItem(_itemName, _quantity, _description, _spriteRenderer.sprite);
            if (leftOverItems <= 0)
                Destroy(gameObject);
            else
                _quantity = leftOverItems;
        }
    }
}
