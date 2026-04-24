using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    [SerializeField] private UICanvasJoystick leftJoystick;
    [SerializeField] private UICanvasJoystick rightJoystick;
    override public void InstallBindings()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE)
        Container.BindInterfacesTo<DesktopInput>().AsSingle();
#elif UNITY_ANDROID || UNITY_IOS
        Container.BindInstance(leftJoystick).WithId("MoveJoystick");
        Container.BindInstance(rightJoystick).WithId("AttackJoystick");
        Container.Bind<IInput>().To<MobileInput>().AsSingle();
#endif
        Container.BindInterfacesTo<UnityService>().AsSingle();
    }
}
