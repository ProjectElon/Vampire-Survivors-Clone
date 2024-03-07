using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void Setup(WeaponSO weaponData, Weapon weapon, Transform player);
}