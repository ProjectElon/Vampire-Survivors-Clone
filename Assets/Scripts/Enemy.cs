using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _damage = 10;
    [SerializeField] private LayerMask _whatIsEnemey;

    private int _health;
    private Transform _target;
    public Transform Target { set => _target = value; } 
    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;
    private Animator _animator;
    private IObjectPool<Enemy> _objectPool;
    public IObjectPool<Enemy> ObjectPool { set => _objectPool = value; }
    private float _knockBackTime = 0.1f;
    private float _knockBackTimer = 0.0f;
    
    private void Awake()
    {
        _health = _maxHealth;
        _knockBackTimer = _knockBackTime;
        
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // _knockBackTimer += Time.fixedDeltaTime;
        // if (_knockBackTimer >= _knockBackTime) 
        // {
        //     Vector2 pos = transform.position;
        //     Vector2 positionToTarget = (Vector2)_target.position - pos;
        //     Vector2 lookDirection = positionToTarget.normalized;

        //     // float radius = 25.0f;
        //     // if (positionToTarget.sqrMagnitude >= radius * radius)
        //     // {
        //     //     transform.position = _target.transform.position + (Vector3)(Mathf.Sign(Random.Range(-1.0f, 1.0f)) * lookDirection * radius);
        //     //     return;
        //     // }
            
        //     float saparationRadius = _circleCollider.radius;
            
        //     Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, saparationRadius, _whatIsEnemey);
        //     Vector2 saperation = Vector2.zero;
            
        //     foreach (Collider2D collider in colliders)
        //     {
        //         if (collider == _circleCollider || collider.transform == _target)
        //         {
        //             continue;
        //         }
                
        //         Vector2 neighbourPos = collider.transform.position;
        //         saperation += pos - neighbourPos;
        //     }

        //     _rb.MovePosition(_rb.position + saperation * 10.0f * Time.fixedDeltaTime + lookDirection * _moveSpeed * Time.fixedDeltaTime);
        // }
    }

    public void TakeDamage(int amount, Vector2 direction)
    {
        _knockBackTimer = 0.0f;
        _rb.AddForce(direction * 2.0f, ForceMode2D.Impulse);

        _health = Mathf.Max(_health - amount, 0);
        if (_health == 0 && gameObject.activeSelf)
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
        _objectPool.Release(this);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.transform == _target && other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            Vector2 lookDirection = (_target.position - transform.position).normalized;
            damageable.TakeDamage(_damage, lookDirection);
        }
    }
}
