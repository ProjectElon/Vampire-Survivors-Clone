using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GhostTrail : MonoBehaviour
{
    [SerializeField] private float _lifeTime;
    
    private float _aliveTimer;
    private SpriteRenderer _spriteRenderer;

    private IObjectPool<GhostTrail> _objectPool;
    public IObjectPool<GhostTrail> ObjectPool { set => _objectPool = value; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float alpha = Mathf.Clamp01(1.0f - _aliveTimer / _lifeTime);
        Color color = _spriteRenderer.color;
        color.a = alpha;
        _spriteRenderer.color = color;
        _aliveTimer += Time.deltaTime;
    }

    public void Setup(Sprite sprite, Color color, float lifeTime = 1.0f)
    {
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.color = color;
        _lifeTime = lifeTime;
        _aliveTimer = 0.0f;
        StartCoroutine(ReleaseToObjectPool());
    }

    private IEnumerator ReleaseToObjectPool()
    {
        yield return new WaitForSeconds(_lifeTime);
        _objectPool.Release(this);
    }
}
