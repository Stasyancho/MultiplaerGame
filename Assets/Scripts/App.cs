using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class App
{
    public static IUnityService Unity;
    
    public static void Initialize(IUnityService service)
    {
        Unity = service;
    }

    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Unity.Instantiate(prefab, position, rotation);
    }
}
