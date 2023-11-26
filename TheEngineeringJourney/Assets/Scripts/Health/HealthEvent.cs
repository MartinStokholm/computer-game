using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEvent : MonoBehaviour
{
    public event Action<HealthEvent, HealthEventArgs> OnHealthChanged;

    public void CallHealthChangedEvent(int healthAmount, int damageAmount)
    {
        OnHealthChanged?.Invoke(this, new HealthEventArgs() { HealthAmount = healthAmount, DamageAmount = damageAmount });
    }
}

public class HealthEventArgs : EventArgs
{
    public int HealthAmount;
    public int DamageAmount;
}
