using System;
using Card;
using UnityEngine;
using Zenject;
using Zenject.Signals;

namespace Gameplay
{
    public class DragController : MonoBehaviour
    {
        [Inject] private SignalBus signalBus;

        private CardController selectedCard;
        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void OnEnable()
        {
            signalBus.Subscribe<SelectCardSignal>(SelectCard);
        }

        private void Update()
        {
            if (selectedCard != null && selectedCard.IsEnabled)
            {
                var pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                selectedCard.transform.position = new Vector3(pos.x, pos.y, 0);
            }
        }

        private void OnDisable()
        {
            signalBus.Unsubscribe<SelectCardSignal>(SelectCard);
        }

        private void SelectCard(SelectCardSignal signal)
        {
            selectedCard = signal.Card;
        }
    }
}