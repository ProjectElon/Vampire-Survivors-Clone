using UnityEngine;
using UnityEngine.Pool;

public class GhostTrailEffect : MonoBehaviour
{
    [SerializeField] private GhostTrail _ghostTrailPrefab;
    [SerializeField] private Color _color;
    [SerializeField] private float _spawnRate = 0.1f;
    [SerializeField] private float _lifeTime = 0.2f;

    private float _spwanTimer;
    private SpriteRenderer _spriteRenderer;
    private IObjectPool<GhostTrail> _ghostTrailPool;

    private void Awake()
    {
        _spwanTimer = 0.0f;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ghostTrailPool = new ObjectPool<GhostTrail>(CreateGhostTrail, OnGetFromPool, GetReleaseFromPool, OnDestroyPoolObject, true, 100, 10000);
    }

    private GhostTrail CreateGhostTrail()
    {
        GhostTrail ghostTrail = Instantiate(_ghostTrailPrefab);
        ghostTrail.ObjectPool = _ghostTrailPool;
        return ghostTrail;
    }

    private void OnGetFromPool(GhostTrail ghostTrail)
    {
        ghostTrail.gameObject.SetActive(true);
    }

    private void GetReleaseFromPool(GhostTrail ghostTrail)
    {
        ghostTrail.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(GhostTrail ghostTrail)
    {
        Destroy(ghostTrail.gameObject);
    }

    private void Update()
    {
        _spwanTimer += Time.deltaTime;
        while (_spwanTimer >= _spawnRate)
        {
            _spwanTimer -= _spawnRate;
            
            GhostTrail ghostTrail = _ghostTrailPool.Get(); 
            ghostTrail.transform.SetPositionAndRotation(transform.position, transform.rotation);
            ghostTrail.Setup(_spriteRenderer.sprite, _color, _lifeTime);
        }
    }
}