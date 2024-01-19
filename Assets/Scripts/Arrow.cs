using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float _impluseForce = 15.0f;
    [SerializeField] private float _lifeTime = 5.0f;

    private IObjectPool<Arrow> _objectPool; 
    public IObjectPool<Arrow> ObjectPool { set => _objectPool = value; }

    private Rigidbody2D _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void SetUp()
    {
        _rb.AddForce(transform.right * _impluseForce, ForceMode2D.Impulse);
        StartCoroutine(ReturnToPool());
    }

    private void Reset()
    {
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0.0f;
    }

    private IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(_lifeTime);
        
        Reset();
        _objectPool.Release(this);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Reset();
    }
}