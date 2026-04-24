using UnityEngine;
using Zenject;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject mobileInputCanvas;
    [SerializeField] private GameObject desktopInputCanvas;
    
    [Inject] private IUnityService _unityService;
    void Awake()
    {
        App.Initialize(_unityService);
        ActivateBattleUI();
    }

    public void ActivateBattleUI()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE)
        desktopInputCanvas.SetActive(true);
#elif UNITY_ANDROID || UNITY_IOS
        mobileInputCanvas.SetActive(true);
#endif
    }
}