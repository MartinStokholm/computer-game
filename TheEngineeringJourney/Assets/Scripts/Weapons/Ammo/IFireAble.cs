
using UnityEngine;

public interface IFireAble
{
    void InitialiseAmmo(
        AmmoDetailsSO ammoDetails, 
        float aimAngle, 
        float weaponAimAngle, 
        float ammoSpeed, 
        Vector3 weaponAimDirectionVector, 
        bool overrideAmmoMovement = false);

    GameObject GetGameObject();
}
