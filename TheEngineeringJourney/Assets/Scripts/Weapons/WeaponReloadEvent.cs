using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponReloadEvent : MonoBehaviour
{
    public event Action<WeaponReloadEvent, ReloadWeaponEventArgs> OnReloadWeapon;

    /// <summary>
    /// Specify the weapon to have it's clip reloaded.  If the total ammo is also to be increased then specify the topUpAmmoPercent.
    /// </summary>
    public void CallReloadWeaponEvent(Weapon weapon, int topUpAmmoPercent)
    {
        OnReloadWeapon?.Invoke(this, new ReloadWeaponEventArgs() { Weapon = weapon, TopUpAmmoPercent = topUpAmmoPercent });
    }
}


public class ReloadWeaponEventArgs : EventArgs
{
    public Weapon Weapon;
    public int TopUpAmmoPercent;
}