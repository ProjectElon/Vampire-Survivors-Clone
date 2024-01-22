using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private int _maxHealth = 100;
    private int _health;
    private Transform _target;
    public Transform Target { set => _target = value; } 
    private Rigidbody2D _rb;
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
        _animator = GetComponent<Animator>();
    }
    
    private void FixedUpdate()
    {
        _knockBackTimer += Time.fixedDeltaTime;
        if (_knockBackTimer >= _knockBackTime) 
        {
            Vector2 lookDirection = (_target.position - transform.position).normalized;
            float lookAngle = Mathf.Clamp01(Mathf.Sign(lookDirection.x) * -1.0f) * 180.0f;
            Quaternion newRotation = Quaternion.Euler(0.0f, lookAngle, 0.0f);
            transform.rotation = newRotation;
            _rb.MovePosition(Vector2.MoveTowards(_rb.position, (Vector2)_target.position, _moveSpeed * Time.fixedDeltaTime));    
        }
    }

    public void TakeDamage(int amount, Vector2 direction)
    {
        _knockBackTimer = 0.0f;
        _rb.AddForce(direction * 2.0f, ForceMode2D.Impulse);

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
        _objectPool.Release(this);
    }
}
