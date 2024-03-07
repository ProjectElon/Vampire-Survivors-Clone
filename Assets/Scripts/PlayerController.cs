using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private float _moveSpeed = 5.0f;
    public float MoveSpeed => _moveSpeed;

    [SerializeField] private int _maxHealth = 100;
    private int _health;

    [SerializeField] private float _takeDamageTime = 0.5f;
    private bool _isTakingDamage;
    
    private Vector2 _movementInput;
    private Vector2 _lookDirection;
    public Vector2 LookDirection => _lookDirection;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private GhostTrailEffect _ghostTrailEffect;
    public bool IsMoving => _rb.velocity.sqrMagnitude >= 0;

    private void Awake()
    {
        _health = _maxHealth;
        _isTakingDamage = false;
        _movementInput = Vector2.zero;
        
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _ghostTrailEffect = GetComponent<GhostTrailEffect>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
            _lookDirection = input.normalized;
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
        if (_isTakingDamage)
        {
            return;
        }
        
        _health = Mathf.Max(_health - amount, 0);
        if (_health == 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(TakeDamage_CR());
        }
    }

    IEnumerator TakeDamage_CR()
    {
        _isTakingDamage = true;
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(_takeDamageTime);
        _isTakingDamage = false;
        _spriteRenderer.color = Color.white;
    }
    
    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}