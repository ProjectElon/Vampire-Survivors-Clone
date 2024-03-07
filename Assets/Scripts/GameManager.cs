using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private PlayerController _playerController;
    
    [SerializeField] private int _enemyCount;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private LayerMask _enemyLayerMask;
    
    private List<Enemy> _enemies;
    
    public static GameManager Instance { get; private set; }
    public IObjectPool<Enemy> EnemyPool { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _playerController = _player.GetComponent<PlayerController>();

        EnemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGetFromPool, GetReleaseFromPool, OnDestroyPoolObject, true, 500, 1000);
        _enemies = new List<Enemy>();

        Camera cam = Camera.main;
            
        float camHeight = 2.0f * cam.orthographicSize;
        float camWidth = cam.aspect * camHeight;
        float r = Mathf.Max(camWidth, camHeight);

        for (int i = 0; i < _enemyCount; i++)
        {
            float range = 2.0f;
            Vector2 randomOffset = Random.insideUnitCircle.normalized * (r + Random.Range(0, range));
            randomOffset += new Vector2(Random.Range(-range, range), Random.Range(-range, range));
            
            Enemy enemy = EnemyPool.Get();
            enemy.transform.position = randomOffset + (Vector2)cam.transform.position;
        }
    }

    private void FixedUpdate()
    {
        float moveSpeed = 1.0f;
        float radiusBias = 0.1f;
        
        for (int i = 0; i < _enemies.Count; i++)
        {
            Enemy enemy = _enemies[i];
            Rigidbody2D rb = enemy.Rigidbody;
            CircleCollider2D collider = enemy.Collider;

            Vector2 pos = rb.transform.position;
            Vector2 posToPlayer = (Vector2)_player.position - pos;

            Vector2 lookDir = Vector2.zero;

            if (posToPlayer.sqrMagnitude > 0.0f)
            {
                lookDir = posToPlayer.normalized;
                float lookAngle = Mathf.Clamp01(Mathf.Sign(lookDir.x) * -1.0f) * 180.0f;
                Quaternion newRotation = Quaternion.Euler(0.0f, lookAngle, 0.0f);
                rb.transform.rotation = newRotation;
            }

            float teleportRadius = 20.0f;
            if (posToPlayer.sqrMagnitude >= teleportRadius * teleportRadius)
            {
                rb.transform.position = _player.transform.position + (Vector3)lookDir * Mathf.Sign(Random.Range(-1f, 1.0f)) * teleportRadius;
                continue;
            }
            
            float radius = collider.radius + radiusBias;

            Vector2 separation = Vector2.zero;
            int separationCount = 0;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(rb.position, radius * 2.0f, _enemyLayerMask);
            foreach (CircleCollider2D neighbourCollider in colliders)
            {
                if (collider != neighbourCollider)
                {   
                    float neighbourRadius = neighbourCollider.radius + radiusBias;
                    Vector2 offset = rb.position - (Vector2)neighbourCollider.transform.position;
                    if (offset.sqrMagnitude < (radius + neighbourRadius) * (radius + neighbourRadius))
                    {
                        float pushDist = (radius + neighbourRadius) - offset.magnitude;
                        separation += offset.normalized * pushDist;
                        separationCount++;
                    }
                }
            }
            
            float dist = moveSpeed * Time.fixedDeltaTime;
            float separationDist = 0.0f;
            float moveDist = 0.0f;    
            float separationFactor = 10.0f;
            
            if (separationCount > 0)
            {
                separationDist = separation.magnitude * separationFactor * Time.fixedDeltaTime;
            }

            if (separationDist < dist)
            {
                moveDist = dist - separationDist; 
            }
            
            if (posToPlayer.sqrMagnitude < radius * radius)
            {
                if (_playerController.IsMoving)
                {
                    moveDist = -_playerController.MoveSpeed * Time.fixedDeltaTime;
                }
                _playerController.TakeDamage(enemy.Damge, -lookDir);
            }
            
            Vector2 newPos = rb.position + separation * separationFactor * Time.fixedDeltaTime + lookDir * moveDist;
            rb.MovePosition(newPos);
        }
    }

    private Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(_enemyPrefab);
        enemy.gameObject.SetActive(false);
        enemy.ObjectPool = EnemyPool;
        return enemy;
    }

    private void OnGetFromPool(Enemy enemy)
    {
        _enemies.Add(enemy);
        enemy.gameObject.SetActive(true);
    }

    private void GetReleaseFromPool(Enemy enemy)
    {
        _enemies.Remove(enemy);
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}
