using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private Trail _trailPrefab;
    [SerializeField] private Transform _trailParent;
    [SerializeField] private float _trailSpawnRate = 0.1f;
    [SerializeField] private float _trailLifeTime = 0.2f;
    private float _tailSpawnTimer = 0.0f;
    private Vector2 _movementInput;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private void Awake()
    {
        _movementInput = Vector2.zero;
        
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); 
        _movementInput += input;

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

        bool isMoving = input.x != 0.0f || input.y != 0.0f;
        
        if (isMoving)
        {
            _tailSpawnTimer += Time.deltaTime;
            while (_tailSpawnTimer >= _trailSpawnRate)
            {
                _tailSpawnTimer -= _trailSpawnRate;
                
                Trail trail = Instantiate(_trailPrefab, transform.position, transform.rotation);
                trail.Setup(_spriteRenderer.sprite, _trailLifeTime);
                trail.transform.parent = _trailParent;
            }
        }

        _animator.SetBool("IsMoving", isMoving);
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _movementInput.normalized * _moveSpeed * Time.fixedDeltaTime);
        _movementInput = Vector2.zero;
    }
}