using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Weapon", menuName="ScriptableObjects/Weapons")]
public class WeaponSO : ScriptableObject
{
    public Transform Prefab;
    public int InstanceCountPerUse;
    public int MaxInstanceCount;
    public float Cooldown;
    public float Speed;
    public int Damage;
    public float Range;
    public int BounceCount;
}
