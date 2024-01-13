using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _objectToPool;
    [SerializeField] private int _objectCount;

    private void Start()
    {
        for (int i = 0; i < _objectCount; i++)
        {
            GameObject gameObject = Instantiate(_objectToPool);
            gameObject.transform.parent = transform;
            gameObject.SetActive(false);
        }
    }

    public GameObject Instantiate()
    {
        if (transform.childCount > 0)
        {
            Transform pooledObjectTransform = transform.GetChild(0);
            if (!pooledObjectTransform.gameObject.activeInHierarchy)
            {
                pooledObjectTransform.parent = null;
                return pooledObjectTransform.gameObject;
            }
        }
        return null;
    }

    public void Destroy(GameObject gameObject)
    {
        gameObject.transform.parent = transform;
        gameObject.SetActive(false);
    }
}
