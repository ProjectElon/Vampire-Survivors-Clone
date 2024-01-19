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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            ArrowPool = new ObjectPool<Arrow>(CreateArrow, OnGetFromPool, GetReleaseFromPool, OnDestroyPoolObject, true, 100, 1000);
        }
        
        for (int i = 0; i < _enemyCount; i++)
        {
            float range = 10.0f;
            Vector3 position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0.0f);
            Enemy enemy = Instantiate(_enemyPrefab, position + _player.position, Quaternion.identity);
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
}
