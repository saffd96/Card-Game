using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Card;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zenject.Signals;

namespace Gameplay
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Button button;

        [Inject] private SignalBus signalBus;

        private List<CardController> cards = new List<CardController>();
        private bool isCardsCreated;

        private void OnEnable()
        {
            button.onClick.AddListener(OnClickAction);
            signalBus.Subscribe<UpdateCardListSignal>(UpdateCards);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClickAction);
            signalBus.Unsubscribe<UpdateCardListSignal>(UpdateCards);
        }

        private void OnClickAction()
        {
            if (!isCardsCreated) return;
            StartCoroutine(ChangeStatRoutine());
        }

        private void UpdateCards(UpdateCardListSignal signal)
        {
            if (!isCardsCreated)
                isCardsCreated = true;

            cards = signal.Cards;
        }

        private IEnumerator ChangeStatRoutine()
        {
            while (cards.Count > 0)
            {
                var cardsList = cards.ToList();
                foreach (var card in cardsList)
                {
                    var stats = Enum.GetValues(typeof(CardValueType));
                    var randomStat = (CardValueType) stats.GetValue(new System.Random().Next(stats.Length));
                    var amount = UnityEngine.Random.Range(-2, 10);

                    yield return card.UpdateCardStatRoutine(randomStat, amount);
                    if (card.CardData.GetStatByType(CardValueType.Health) < 1)
                    {
                        signalBus.Fire(new RemoveCardSignal(card));
                    }
                }

                yield return null;
            }
        }
    }
}