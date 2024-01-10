using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] private float _lifeTime;
    private float _aliveTimer;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _aliveTimer = 0.0f;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Sprite sprite, float lifeTime = 1.0f)
    {
        _lifeTime = lifeTime;
        _spriteRenderer.sprite = sprite;
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        float alpha = Mathf.Clamp01(1.0f - _aliveTimer / _lifeTime);
        Color color = _spriteRenderer.color;
        color.a = alpha;
        _spriteRenderer.color = color;
        _aliveTimer += Time.deltaTime;
    }
}
