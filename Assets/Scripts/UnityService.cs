using UnityEngine;
using Zenject;

public class UnityService : IUnityService
{
    private readonly DiContainer _container; 
    
    public UnityService(DiContainer container)
    {
        _container = container;
    }
    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        Debug.Log($"Instantiate {prefab.name}");
        GameObject prefabGO = _container.InstantiatePrefab(prefab, parent);
        return prefabGO;
    }

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Debug.Log($"Instantiate {prefab.name}");
        GameObject prefabGO = _container.InstantiatePrefab(prefab, position, rotation, null);
        return prefabGO;
    }
}
