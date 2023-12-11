using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEvent : MonoBehaviour
{
    public event Action<HealthEvent, HealthEventArgs> OnHealthChanged;

    public void CallHealthChangedEvent(float healthPercent, int healthAmount, int damageAmount)
    {
        OnHealthChanged?.Invoke(this, new HealthEventArgs() { HealthPercent = healthPercent, HealthAmount = healthAmount, DamageAmount = damageAmount });
    }
}

public class HealthEventArgs : EventArgs
{
    public float HealthPercent;
    public int HealthAmount;
    public int DamageAmount;
}
