using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    [SerializeField] private GameObject afterImagePrefab;
    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    GameObject pool;

    private void Start()
    {
        pool = new GameObject();
        pool.name = "AfterImages";
        for (int i = 0; i < 10; i++)
        {
            GrowPool();
        }
    }

    private void GrowPool()
    {
        var instanceToAdd = Instantiate(afterImagePrefab);
        instanceToAdd.GetComponent<PlayerAfterImage>().imagePool = this;
        instanceToAdd.transform.SetParent(pool.transform);
        AddToPool(instanceToAdd);
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            GrowPool();
        }
        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
