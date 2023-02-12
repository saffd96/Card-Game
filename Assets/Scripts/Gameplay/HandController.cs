using System.Collections.Generic;
using System.Linq;
using Card;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using Zenject;
using Zenject.Signals;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace Gameplay
{
    public class HandController : MonoBehaviour
    {
        [SerializeField] private List<Transform> pathPoints;
        [SerializeField] private int test;

        [SerializeField] private Transform topParent;
        [SerializeField] private Transform normalParent;

        [SerializeField] private int minCards;
        [SerializeField] private int maxCards;
        [SerializeField] private float radius;

        [SerializeField] private float maxCardAngle;

        [Inject] private Prefabs prefabs;
        [Inject] private SignalBus signalBus;

        private List<CardController> cards = new List<CardController>();

        private Sequence baseSequence;
        private Vector2 offsetRadPos;

        private readonly List<Vector3> path = new List<Vector3>();

        public float CardAngle => Mathf.Clamp(1.66f * cards.Count, 0f, maxCardAngle);

        public float Radius => radius * 1000;

        private void Awake()
        {
            foreach (var pathPoint in pathPoints)
            {
                path.Add(pathPoint.position);
            }

            path.Add(transform.position);
        }

        private void OnEnable()
        {
            signalBus.Subscribe<RemoveCardSignal>(RemoveCard);
            signalBus.Subscribe<RemoveCardFromHandSignal>(RemoveCardFromHand);
            signalBus.Subscribe<SetNewParentForCardSignal>(SetParent);
        }

        private void OnDisable()
        {
            signalBus.Unsubscribe<RemoveCardSignal>(RemoveCard);
            signalBus.Unsubscribe<RemoveCardFromHandSignal>(RemoveCardFromHand);
            signalBus.Unsubscribe<SetNewParentForCardSignal>(SetParent);
        }

        private void Start()
        {
            var sequence = DOTween.Sequence().SetEase(Ease.Linear);

            offsetRadPos = new Vector2(0, -Radius + prefabs.cardController.WightOffset);

            var cardsAmount = Random.Range(minCards, maxCards + 1);
            //var cardsAmount = test;

            for (int i = 0; i < cardsAmount; i++)
            {
                CreateCard(sequence);
            }

            cards.Reverse();

            sequence.OnComplete(
                () =>
                {
                    ActivateCards();
                    SortCards();
                });
        }

        private void SetParent(SetNewParentForCardSignal signal)
        {
            var card = signal.Card;

            card.transform.SetParent(signal.IsTopParentNeed ? topParent : normalParent);
        }

        private void ActivateCards()
        {
            foreach (var cardController in cards)
            {
                cardController.SetIsInteractable(true);
                cardController.SetIsEnabled(true);
                cardController.ChangeState(true);
            }
        }

        private void CreateCard(Sequence sequence)
        {
            var card = LeanPool.Spawn(prefabs.cardController, normalParent);

            card.transform.position = path.First();
            card.Init(-transform.position.y);
            cards.Add(card);
            card.SetIsInteractable(false);

            sequence.Append(card.transform.DOPath(path.ToArray(), 0.75f, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.OutQuart));
            sequence.Join(card.transform.DOPunchRotation(new Vector3(0,0,90), 0.75f).SetEase(Ease.Linear));
            sequence.OnComplete(() => card.SetIsInteractable(true));
        }

        private void RemoveCard(RemoveCardSignal signal)
        {
            var card = signal.CardToRemove;
            cards.Remove(card);

            card.SetIsInteractable(false);
            card.SetIsEnabled(false);

            card.ChangeState(false, isNeedToMove: true, onCompleteAction: () =>
            {
                LeanPool.Despawn(card);
                SortCards();
            });
        }
        
        private void RemoveCardFromHand(RemoveCardFromHandSignal signal)
        {
            var card = signal.CardToRemove;
            
            card.SetIsInteractable(false);
            cards.Remove(card);
            SortCards();
        }

        private void SortCards()
        {
            baseSequence?.Kill();

            var sequence = DOTween.Sequence();

            var angleForCards = CardAngle / cards.Count;

            var minAngle = 90 - CardAngle / 2 + angleForCards / 2;

            for (int i = 0; i < cards.Count; i++)
            {
                var angle = minAngle + angleForCards * i;
            
                var x = offsetRadPos.x + Mathf.Cos(Mathf.Deg2Rad * angle) * Radius;
                var y =  offsetRadPos.y + Mathf.Sin(Mathf.Deg2Rad * angle) * Radius;

                var pos = new Vector2(x, y);

                var rotation = new Vector3(0, 0, angle - 90);

                cards[i].SavePositionAndRotation(pos, rotation);
                
                if (!cards[i].IsEnabled || !cards[i].IsInteractable) return;

                sequence.Join(cards[i].transform.DOLocalMove(pos, 0.25f).SetEase(Ease.Linear));
                sequence.Join(cards[i].transform.DOLocalRotate(rotation, 0.25f).SetEase(Ease.Linear));
                sequence.OnComplete(() => cards[i].SetIsInteractable(true));
            }

            signalBus.Fire(new UpdateCardListSignal(cards));

            baseSequence = sequence;
        }
    }
}