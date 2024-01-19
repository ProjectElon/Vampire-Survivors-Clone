using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1.0f;
    private Transform _target;
    public Transform Target { set => _target = value; } 

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate()
    {
        Vector2 lookDirection = (_target.position - transform.position).normalized;
        float lookAngle = Mathf.Clamp01(Mathf.Sign(lookDirection.x) * -1.0f) * 180.0f;
        Quaternion newRotation = Quaternion.Euler(0.0f, lookAngle, 0.0f);
        transform.rotation = newRotation;
        _rb.MovePosition(Vector2.MoveTowards(_rb.position, (Vector2)_target.position, _moveSpeed * Time.fixedDeltaTime));
    }
}
