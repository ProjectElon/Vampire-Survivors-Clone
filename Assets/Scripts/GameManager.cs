using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Arrow _arrowPrefab;

    [SerializeField] private int _enemyCount = 100;
    [SerializeField] private Enemy _enemyPrefab;

    public static GameManager Instance { get; private set; }
    public IObjectPool<Arrow> ArrowPool { get; private set; }
    public IObjectPool<Enemy> EnemyPool { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            ArrowPool = new ObjectPool<Arrow>(CreateArrow, OnGetFromPool, GetReleaseFromPool, OnDestroyPoolObject, true, 100, 1000);
            EnemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGetFromPool, GetReleaseFromPool, OnDestroyPoolObject, true, 100, 1000);
        }
        
        for (int i = 0; i < _enemyCount; i++)
        {
            Camera cam = Camera.main;
            
            float camWidth = 2.0f * cam.orthographicSize;
            float camHeight = cam.aspect * camWidth;

            float range = 1.0f;
            Vector2 randomPosition = new Vector2(Random.Range(0, range), Random.Range(0, range)) + new Vector2(camWidth * 0.5f, camHeight * 0.5f);
            Enemy enemy = EnemyPool.Get();
            enemy.transform.position = randomPosition + (Vector2)cam.transform.position;
            enemy.Target = _player;
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
