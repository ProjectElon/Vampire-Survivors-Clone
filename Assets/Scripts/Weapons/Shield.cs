using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Shield : MonoBehaviour, IWeapon
{
    private Rigidbody2D _rb;
    private CircleCollider2D _collider;
    private Vector2 _moveDirection;
    private Collider2D _closestEnemyCollider;
    private int _bounceCount = 1;
    private float _bounceTimer = 0.0f;
    private float _bounceTime = 0.5f;
    
    private WeaponSO _weaponData;
    private Weapon _weapon;
    private Transform _player;
    private CircleCollider2D _playerCollier;
    
    public void Setup(WeaponSO weaponData, Weapon weapon, Transform player)
    {
        _weaponData = weaponData;
        _weapon = weapon;
        _player = player;
        _playerCollier = _player.GetComponent<CircleCollider2D>();
        _bounceCount = weaponData.BounceCount;
        SetMoveDirectionToClosestEnemy(0, 0);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        _bounceCount = 1;
        _bounceTimer = _bounceTime;
        _closestEnemyCollider = null;
    }

    private void Update()
    {
        Camera cam = Camera.main;
        float halfCamHeight = cam.orthographicSize;
        float halfCamWidth = cam.aspect * halfCamHeight;
        float xBounds = halfCamWidth;
        float yBounds = halfCamHeight;

        bool shouldBounce = false;
        int xdir = 0;
        int ydir = 0;

        if (transform.position.x - _collider.radius < cam.transform.position.x - xBounds)
        {
            shouldBounce = true;
            xdir = -1;
        }

        if (transform.position.x + _collider.radius > cam.transform.position.x + xBounds)
        {
            shouldBounce = true;
            xdir = 1;
        }

        if (transform.position.y - _collider.radius < cam.transform.position.y - yBounds)
        {
            shouldBounce = true;
            ydir = -1;
        }

        if (transform.position.y + _collider.radius > cam.transform.position.y + yBounds)
        {
            shouldBounce = true;
            ydir = 1;
        }
        
        if (_bounceTimer < _bounceTime)
        { 
            _bounceTimer += Time.deltaTime;
        }
        
        bool canBounce = _bounceTimer >= _bounceTime;
        if (shouldBounce && canBounce)
        {
            Bounce(xdir, ydir);
            _bounceTimer = 0.0f;
        }

        bool shouldReturnToPlayer = _bounceCount == 0;
        if (shouldReturnToPlayer)
        {
            _moveDirection = (_player.transform.position - transform.position).normalized;
            float playerRadius = _playerCollier.radius;
            float distSq = (transform.position - _player.transform.position).sqrMagnitude;
            if (distSq <= playerRadius * playerRadius)
            {
                _weapon.OnWeapownDestroyed();
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        float speed = _bounceCount == 0 ? _weaponData.Speed * 2.0f : _weaponData.Speed;
        _rb.MovePosition(_rb.position + _moveDirection * speed * Time.fixedDeltaTime);
        float rotationSpeed = 360.0f;
        _rb.MoveRotation(_rb.rotation + rotationSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(_weaponData.Damage, _moveDirection);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            _closestEnemyCollider = other;
            Bounce();
        }
    }

    private void SetMoveDirectionToClosestEnemy(int xdir, int ydir)
    {
        Collider2D closestCollider = null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _weaponData.Range, LayerMask.GetMask("Enemy"));
        float rangeSq = float.MaxValue;
        
        foreach (Collider2D collider in colliders)
        {
            Vector2 offset = collider.transform.position - transform.position;
            if (offset.sqrMagnitude < rangeSq && collider != _closestEnemyCollider)
            {
                closestCollider = collider;
                rangeSq = offset.sqrMagnitude;
            }
        }
        
        if (closestCollider)
        {
            _moveDirection = (closestCollider.transform.position - transform.position).normalized;
        }
        else
        {
            float minAngle = 0;
            float maxAngle = 360.0f;
            
            if (xdir == -1)
            {
                if (ydir == -1)
                {
                    minAngle = 0.0f;
                    maxAngle = 90.0f;
                }
                else if (ydir == 1)
                {
                    minAngle = 270.0f;
                    maxAngle = 360.0f;
                }
                else
                {
                    minAngle = -90.0f;
                    maxAngle = 90.0f;
                }
            }
            else if (xdir == 1)
            {
                if (ydir == -1)
                {
                    minAngle = 90.0f;
                    maxAngle = 180.0f;
                }
                else if (ydir == 1)
                {
                    minAngle = 180.0f;
                    maxAngle = 270.0f;
                }
                else
                {
                    minAngle = 90.0f;
                    maxAngle = 270.0f;
                }
            }
            else
            {
                if (ydir == -1)
                {
                    minAngle = 0.0f;
                    maxAngle = 180.0f;
                }
                else if (ydir == 1)
                {
                    minAngle = 180.0f;
                    maxAngle = 360.0f;
                }
            }

            float offset = 10.0f;
            float randomAngle = Random.Range(minAngle + offset, maxAngle - offset) * Mathf.Deg2Rad;
            _moveDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;
        }

        _closestEnemyCollider = closestCollider;
    }

    private void Bounce(int xdir = 0, int ydir = 0)
    {
        if (_bounceCount > 0)
        {
            SetMoveDirectionToClosestEnemy(xdir, ydir);
            _bounceCount--;
        }
    }
}