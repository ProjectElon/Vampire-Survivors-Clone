using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Arrow _arrowPrefab;

    [SerializeField] private int _enemyCount = 100;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private LayerMask _enemyLayerMask;
    
    private Rigidbody2D[] _rbs;
    private CircleCollider2D[] _colliders;

    public static GameManager Instance { get; private set; }
    public IObjectPool<Arrow> ArrowPool { get; private set; }
    public IObjectPool<Enemy> EnemyPool { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        ArrowPool = new ObjectPool<Arrow>(CreateArrow, OnGetFromPool, GetReleaseFromPool, OnDestroyPoolObject, true, 100, 1000);
        EnemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGetFromPool, GetReleaseFromPool, OnDestroyPoolObject, true, 100, 1000);
        
        _rbs = new Rigidbody2D[_enemyCount];
        _colliders = new CircleCollider2D[_enemyCount];

        Camera cam = Camera.main;
            
        float camWidth = 2.0f * cam.orthographicSize;
        float camHeight = cam.aspect * camWidth;
        float r = Mathf.Max(camWidth, camHeight);

        for (int i = 0; i < _enemyCount; i++)
        {
            float range = 2.0f;
            Vector2 randomOffset = Random.insideUnitCircle.normalized * (r + Random.Range(0, range));
            // randomOffset += new Vector2(Random.Range(-range, range), Random.Range(-range, range));
            
            Enemy enemy = EnemyPool.Get();
            enemy.transform.position = randomOffset + (Vector2)cam.transform.position;
            enemy.Target = _player;

            _rbs[i] = enemy.GetComponent<Rigidbody2D>();
            _colliders[i] = enemy.GetComponent<CircleCollider2D>();
        }
    }

    private void FixedUpdate()
    {
        float moveSpeed = 1.0f;
        float radiusBias = 0.05f;
        
        for (int i = 0; i < _enemyCount; i++)
        {
            Rigidbody2D rbi = _rbs[i];
            CircleCollider2D collideri = _colliders[i];

            Vector2 pos = rbi.transform.position;
            Vector2 posToPlayer = (Vector2)_player.position - pos;
            
            Vector2 lookDir = posToPlayer.normalized;
            float lookAngle = Mathf.Clamp01(Mathf.Sign(lookDir.x) * -1.0f) * 180.0f;
            Quaternion newRotation = Quaternion.Euler(0.0f, lookAngle, 0.0f);
            rbi.transform.rotation = newRotation;
            
            float ri = collideri.radius + radiusBias;

            Vector2 separation = Vector2.zero;
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(rbi.position, ri * 2.0f);
            foreach (CircleCollider2D collider in colliders)
            {
                if (collider != collideri)
                {   
                    float rj = collider.radius + radiusBias;
                    Vector2 offset = rbi.position - (Vector2)collider.transform.position;
                    if (offset.sqrMagnitude < (ri + rj) * (ri + rj))
                    {
                        float pushDist = (ri + rj) - offset.magnitude;
                        separation += offset.normalized * pushDist;
                    }
                }
            }
            
            float totalMoveDist = moveSpeed * Time.fixedDeltaTime;
            float separationDist = 0.0f;
            float moveDist = 0.0f;

            float rr = collideri.radius * 0.5f;

            if (separation.sqrMagnitude >= totalMoveDist * totalMoveDist || posToPlayer.sqrMagnitude <= rr * rr)
            {
                separationDist = totalMoveDist;
            }
            else
            {
                separationDist = separation.magnitude;
                moveDist = totalMoveDist - separationDist;
            }

            Vector2 newPos = rbi.position + separation.normalized * separationDist + lookDir * moveDist;
            rbi.MovePosition(newPos);
        }
    }

    private Arrow CreateArrow()
    {
        Arrow arrow = Instantiate(_arrowPrefab);
        arrow.gameObject.SetActive(false);
        arrow.ObjectPool = ArrowPool;
        return arrow;
    }

    private void OnGetFromPool(Arrow arrow)
    {
        arrow.gameObject.SetActive(true);
    }

    private void GetReleaseFromPool(Arrow arrow)
    {
        arrow.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(Arrow arrow)
    {
        Destroy(arrow.gameObject);
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
        enemy.gameObject.SetActive(true);
    }

    private void GetReleaseFromPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}
