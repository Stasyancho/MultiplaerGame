using UnityEngine;
using Zenject;

public class GameBootstrapper : MonoBehaviour
{
    [Inject] private IUnityService _unityService;
    void Awake()
    {
        App.Initialize(_unityService);
    }
}