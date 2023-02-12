using UnityEngine;

namespace Zenject.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private Camera mainCam;
        
        public override void InstallBindings()
        {
            Container.Bind<Camera>().FromInstance(mainCam).AsSingle();
        }
    }
}