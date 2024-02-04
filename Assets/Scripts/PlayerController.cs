using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private Arrow _arrowPrefab;
    [SerializeField] private float _weapownCooldown = 0.5f;
    [SerializeField] private int _maxHealth;
    [SerializeField] private float _damageTime = 0.1667f;
    [SerializeField] private bool _canFire = false;

    private float _damageTimer;
    private int _health;
    
    private Vector2 _movementInput;
    private Vector2 _lastValidInput;
    private float _weapownTimer;
    
    private Rigidbody2D _rb;
    private Animator _animator;
    private GhostTrailEffect _ghostTrailEffect;

    private void Awake()
    {
        _health = _maxHealth;
        _movementInput = Vector2.zero;
        _weapownTimer = 0;
        _damageTimer = _damageTime;
        
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _ghostTrailEffect = GetComponent<GhostTrailEffect>();
    }

    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); 
        _movementInput += input;
        
        // Handle Facing Direction
        {
            Vector3 rotation = transform.localEulerAngles;

            if (input.x > 0.0f)
            {
                rotation.y = 0.0f;
            }
            else if (input.x < 0.0f)
            {
                rotation.y = 180.0f;
            }
            
            transform.localEulerAngles = rotation;    
        }

        bool isMoving = input.x != 0.0f || input.y != 0.0f;
        if (isMoving)
        {
            _lastValidInput = input;
        }

        _weapownTimer += Time.deltaTime;
        
        while (_weapownTimer > _weapownCooldown)
        {
            _weapownTimer -= _weapownCooldown;
            
            Vector2 arrowDirection = _lastValidInput.normalized; 
            float arrowAngle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;
            if (_canFire)
            {
                for (int i = -2; i <= 2; i++)
                {
                    Arrow arrow = GameManager.Instance.ArrowPool.Get();
                    arrow.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0.0f, 0.0f, arrowAngle + 5.0f * i));
                    arrow.SetUp();
                }
            }
        }

        if (_damageTimer < _damageTime)
        {
            _damageTimer += Time.deltaTime;
        }

        _ghostTrailEffect.enabled = isMoving;
        _animator.SetBool("IsMoving", isMoving);
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _movementInput.normalized * _moveSpeed * Time.fixedDeltaTime);   
        _movementInput = Vector2.zero;
    }

    public void TakeDamage(int amount, Vector2 direction)
    {
        if (_damageTimer < _damageTime) return;
        _damageTimer = 0.0f;

        _health = Mathf.Max(_health - amount, 0);

        if (_health == 0)
        {
            Die();
        }
        else
        {
            _animator.SetTrigger("TakeDamage");
        }
    }
    
    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}