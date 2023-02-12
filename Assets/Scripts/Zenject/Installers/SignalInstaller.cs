using Zenject.Signals;

namespace Zenject.Installers
{
    public class SignalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<RemoveCardSignal>();
            Container.DeclareSignal<SetNewParentForCardSignal>();
            Container.DeclareSignal<UpdateCardListSignal>();
            Container.DeclareSignal<SelectCardSignal>();
            Container.DeclareSignal<RemoveCardFromHandSignal>();
        }
    }
}
