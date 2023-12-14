using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponReloadedEvent : MonoBehaviour
{
    public event Action<WeaponReloadedEvent, WeaponReloadedEventArgs> OnWeaponReloaded;

    public void CallWeaponReloadedEvent(Weapon weapon)
    {
        OnWeaponReloaded?.Invoke(this, new WeaponReloadedEventArgs() { Weapon = weapon });
    }
}

public class WeaponReloadedEventArgs : EventArgs
{
    public Weapon Weapon;
}