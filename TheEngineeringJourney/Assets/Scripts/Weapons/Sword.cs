using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private PlayerControls _playerControls;
    private Animator _animator;
    private Player _player;
    private ActiveWeapon _activeWeapon;
    private static readonly int Attack1 = Animator.StringToHash("Attack");

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _activeWeapon = GetComponentInParent<ActiveWeapon>();
        _animator = GetComponent<Animator>();
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }
    
    private  void Start()
    {
        _playerControls.Combat.Attack.started += _ => Attack();
    }

    private void Update()
    {
        MouseFollowWithOffset();
    }

    private void Attack()
    {
        _animator.SetTrigger(Attack1);
    }

    private void MouseFollowWithOffset()
    {
        var mousePos = Input.mousePosition;
        var playerScreenPoint = Camera.main.WorldToScreenPoint(_player.transform.position);

        var angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        
        if (mousePos.x < playerScreenPoint.x)
        {
            var pos = _activeWeapon.transform;
            _activeWeapon.transform.rotation = Quaternion.Euler(0,-180, angle);
            
        }
        else
        {
            _activeWeapon.transform.rotation = Quaternion.Euler(0,0, angle);

        }
    }
    
}
