using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5.0f;
    private Vector2 _movementInput;

    private Rigidbody2D _rb;
    private Animator _animator;
    private GhostTrailEffect _ghostTrailEffect;

    private void Awake()
    {
        _movementInput = Vector2.zero;
        
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

        _ghostTrailEffect.enabled = isMoving;

        _animator.SetBool("IsMoving", isMoving);
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _movementInput.normalized * _moveSpeed * Time.fixedDeltaTime);
        _movementInput = Vector2.zero;
    }
}