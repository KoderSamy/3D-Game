using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Dictionary<GameObject, List<GameObject>> pools = new Dictionary<GameObject, List<GameObject>>();

    public void Initialize(GameObject prefab, int size)
    {
        if (!pools.ContainsKey(prefab))
        {
            pools.Add(prefab, new List<GameObject>());
        }

        List<GameObject> pool = pools[prefab];
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
            obj.transform.SetParent(transform); //  <-- Добавлено для организации в иерархии
        }
    }

    public GameObject GetPooledObject(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
        {
            Debug.LogWarning("Pool for prefab " + prefab.name + " doesn't exist. Creating a new one.");
            Initialize(prefab, 1); 
        }

        List<GameObject> pool = pools[prefab];

        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] != null && !pool[i].activeInHierarchy) 
            {
                return pool[i];
            }
        }

        // Если все объекты заняты, создаем новый (с ограничением)
        if (pool.Count < pool[0]?.GetComponent<PoolSize>()?.poolSize * 2) 
        {
            GameObject obj = Instantiate(prefab);
            pool.Add(obj);
            obj.transform.SetParent(transform); //  <-- Добавлено для организации в иерархии
            return obj;
        }

        Debug.LogWarning("Pool for prefab " + prefab.name + " is full. Returning null.");
        return null;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}