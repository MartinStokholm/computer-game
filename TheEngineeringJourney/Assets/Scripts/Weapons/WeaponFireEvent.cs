using System;
using UnityEngine;

[DisallowMultipleComponent]
public class FireWeaponEvent : MonoBehaviour
{
    public event Action<FireWeaponEvent, FireWeaponEventArgs> OnFireWeapon;

    public void CallFireWeaponEvent(bool fire, bool firePreviousFrame, AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnFireWeapon?.Invoke(this, new FireWeaponEventArgs() { Fire = fire, FirePreviousFrame = firePreviousFrame, AimDirection = aimDirection, AimAngle = aimAngle, WeaponAimAngle = weaponAimAngle, WeaponAimDirectionVector = weaponAimDirectionVector });
    }
}

public class FireWeaponEventArgs : EventArgs
{
    public bool Fire;
    public bool FirePreviousFrame;
    public AimDirection AimDirection;
    public float AimAngle;
    public float WeaponAimAngle;
    public Vector3 WeaponAimDirectionVector;
}