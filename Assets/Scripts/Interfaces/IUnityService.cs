using UnityEngine;

public interface IUnityService
{
    GameObject Instantiate(GameObject prefab, Transform parent = null);
    GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation);
}
