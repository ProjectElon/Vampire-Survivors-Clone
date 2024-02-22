using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 100;
    private int _health;
    
    [SerializeField] private int _damage = 10;
    public int Damge => _damage;

    public CircleCollider2D Collider => _collider;
    public Rigidbody2D Rigidbody => _rb;
    
    private Rigidbody2D _rb;
    private CircleCollider2D _collider;
    
    private Animator _animator;

    private IObjectPool<Enemy> _objectPool;
    public IObjectPool<Enemy> ObjectPool { set => _objectPool = value; }
    
    private void Awake()
    {
        _health = _maxHealth;        
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount, Vector2 direction)
    {
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
}
