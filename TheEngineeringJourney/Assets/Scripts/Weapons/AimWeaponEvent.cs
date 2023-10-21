using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 WeaponAimDirectionVector)
    {
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs()
        {
            AimDirection = aimDirection,
            AimAngle = aimAngle,
            WeaponAimAngle = weaponAimAngle,
            WeaponAimDirectionVector = WeaponAimDirectionVector
        });
    }
}

public class AimWeaponEventArgs : EventArgs
{
    public AimDirection AimDirection;
    public float AimAngle;
    public float WeaponAimAngle;
    public Vector3 WeaponAimDirectionVector;
}