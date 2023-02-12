using UnityEngine;

namespace Zenject.Installers
{
    [CreateAssetMenu(fileName = "SOInstaller", menuName = "Installers/SOInstaller")]
    public class SoInstaller : ScriptableObjectInstaller<SoInstaller>
    {
        [SerializeField] private CardsContainer cardsContainer;
        [SerializeField] private Prefabs prefabs;

        public override void InstallBindings()
        {
            Container.BindInstance(cardsContainer).AsSingle().IfNotBound();
            Container.Bind<Prefabs>().FromInstance(prefabs).AsSingle();
        }
    }
}