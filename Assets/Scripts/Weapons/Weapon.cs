using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSO _weaponData;
    [SerializeField] private Transform _player;
    
    private float _cooldownTimer;
    private int _instanceCount;

    private void Awake()
    {
        _cooldownTimer = 0.0f;
        _instanceCount = 0;
    }

    private void Update()
    {
        _cooldownTimer += Time.deltaTime;
        
        while (_cooldownTimer >= _weaponData.Cooldown)
        {
            for (int i = 0; i < _weaponData.InstanceCountPerUse; i++)
            {
                if (_instanceCount < _weaponData.MaxInstanceCount)
                {
                    Transform weaponGameObject = Instantiate(_weaponData.Prefab, _player.position, Quaternion.identity);
                    IWeapon weapon = weaponGameObject.GetComponent<IWeapon>();
                    weapon.Setup(_weaponData, this, _player);
                    _instanceCount++;    
                }
            }

            _cooldownTimer -= _weaponData.Cooldown;
        }
    }

    public void OnWeapownDestroyed()
    {
        _instanceCount--;
    }
}
