using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    override public void InstallBindings()
    {
        Container.BindInterfacesTo<DesktopInput>().AsSingle();
        Container.BindInterfacesTo<UnityService>().AsSingle();
    }
}
