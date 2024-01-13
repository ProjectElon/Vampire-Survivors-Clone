using System.Collections;
using UnityEngine;

public class GhostTrailEffect : MonoBehaviour
{
    [SerializeField] private ObjectPool _ghostTrailObjectPool;
    [SerializeField] private Transform _parent;
    [SerializeField] private Color _color;
    [SerializeField] private float _spawnRate = 0.1f;
    [SerializeField] private float _lifeTime = 0.2f;

    private float _spwanTimer;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spwanTimer = 0.0f;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _spwanTimer += Time.deltaTime;
        while (_spwanTimer >= _spawnRate)
        {
            _spwanTimer -= _spawnRate;
            
            GameObject ghostTrailGameObject = _ghostTrailObjectPool.Instantiate();
            ghostTrailGameObject.transform.parent = _parent;
            ghostTrailGameObject.transform.position = transform.position;
            ghostTrailGameObject.transform.rotation = transform.rotation;
            
            GhostTrail ghostTrail = ghostTrailGameObject.GetComponent<GhostTrail>();
            ghostTrail.Setup(_spriteRenderer.sprite, _color, _lifeTime);

            StartCoroutine(DestroyGhostTrail(ghostTrailGameObject, _lifeTime));
            ghostTrailGameObject.SetActive(true);
        }
    }
    
    IEnumerator DestroyGhostTrail(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);
        _ghostTrailObjectPool.Destroy(gameObject);
    }
}