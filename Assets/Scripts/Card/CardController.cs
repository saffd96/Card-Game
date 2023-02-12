using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Zenject.Signals;

namespace Card
{
    [RequireComponent(typeof(ZenAutoInjecter))]
    public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
        [SerializeField] private RectTransform rectTransform;

        [SerializeField] private Image artImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;

        [SerializeField] private List<CardValue> cardValues;

        [SerializeField] private GameObject cardBack;
        [SerializeField] private GameObject cardFront;
        [SerializeField] private GameObject cardGlow;
        
        private CardData cardData;
        private CardView cardView;

        private bool cardState;
        private bool isEnabled;
        private bool isInteractable;

        private float baseYPos;
        private int canvasOrder;

        private Sequence baseSequence;

        private Vector3 baseScale;
        private Vector3 inHandPosition;
        private Vector3 inHandRotation;

        [Inject] private DiContainer container;
        [Inject] private SignalBus signalBus;
        [Inject] private Camera mainCam;

        public float WightOffset => rectTransform.sizeDelta.y * 0.5f;
        public CardData CardData => cardData;
        public bool IsInteractable => isInteractable;
        public bool IsEnabled => isEnabled;

        private void OnValidate()
        {
            rectTransform = GetComponent<RectTransform>();
            cardValues = GetComponentsInChildren<CardValue>().ToList();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)),
                Vector3.forward * 1000);
        }

        private void Awake()
        {
            baseScale = transform.localScale;
            ChangeState(false, true, isNeedToCheckState: false);
            CardGlowState(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isInteractable || !isEnabled) return;

            baseSequence?.Kill();
            CardGlowState(true);
            SetIsInteractable(false);
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DORotate(Vector3.zero, 0.15f));
            signalBus.Fire(new SelectCardSignal(this));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isEnabled) return;

            signalBus.Fire(new SelectCardSignal(null));
            CardGlowState(false);
            SetIsInteractable(false);
            var pos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            if (!Physics.Raycast(pos, Vector3.forward * 1000, out var hit))
            {
                var sequence = DOTween.Sequence();
                ReturnToHandPosition(sequence);
                sequence.OnComplete(() => SetIsInteractable(true));
                return;
            }

            if (hit.collider.TryGetComponent(out Table table))
            {
                SetIsEnabled(false);
                transform.SetParent(table.Container.transform);
                signalBus.Fire(new RemoveCardFromHandSignal(this));
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isInteractable || !isEnabled) return;

            baseSequence?.Kill();
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalMoveY(baseYPos + WightOffset * 0.5f, 0.5f));
            baseSequence = sequence;
            signalBus.Fire(new SetNewParentForCardSignal(this, true));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isInteractable || !isEnabled) return;

            baseSequence?.Kill();
            var sequence = DOTween.Sequence();
            ReturnToHandPosition(sequence);
            baseSequence = sequence;
            signalBus.Fire(new SetNewParentForCardSignal(this, false));
        }

        public void Init(float parentY)
        {
            baseYPos = parentY + WightOffset;

            cardData = new CardData(cardValues);
            cardView = new CardView(artImage, nameText, descriptionText);

            container.Inject(cardView);

            cardData.InitCardStats();
            cardView.InitCardView();
        }

        public IEnumerator UpdateCardStatRoutine(CardValueType type, int amount)
        {
            cardData.UpdateCardStatByType(type, amount);

            yield return new WaitForSeconds((float) cardValues.First(x => x.CardValueType == type).AmountTextCounter
                .Duration);
        }

        private void CardGlowState(bool state)
        {
            cardGlow.SetActive(state);
        }

        public void ChangeState(bool state, bool isInstant = false, bool isNeedToMove = false,
            bool isNeedToCheckState = true, Action onCompleteAction = null)
        {
            if (isNeedToCheckState)
                if (cardState == state)
                    return;

            var sequence = DOTween.Sequence();

            sequence.Append(transform.DOScale(new Vector3(0f, 1f, 1f), isInstant ? 0 : 0.25f)
                .OnComplete(() =>
                {
                    cardState = state;
                    cardBack.SetActive(!state);
                    cardFront.SetActive(state);
                }));
            sequence.Append(transform.DOScale(baseScale, isInstant ? 0 : 0.25f));
            if (isNeedToMove)
            {
                ReturnToHandPosition(sequence, isInstant);
                sequence.Append(transform.DOLocalMoveY(baseYPos - WightOffset * 3, isInstant ? 0 : 0.5f)
                    .SetEase(Ease.InBack));
            }

            sequence.OnComplete(() => onCompleteAction?.Invoke());
        }

        public void SavePositionAndRotation(Vector3 newPosition, Vector3 rotation)
        {
            inHandPosition = newPosition;
            inHandRotation = rotation;
        }

        public void SetIsInteractable(bool state)
        {
            isInteractable = state;
        }

        public void SetIsEnabled(bool state)
        {
            isEnabled = state;
        }

        private void ReturnToHandPosition(Sequence sequence, bool isInstant = false)
        {
            sequence.Append(transform.DOLocalMove(inHandPosition, isInstant ? 0 : 0.5f).SetEase(Ease.Linear));
            sequence.Join(transform.DOLocalRotate(inHandRotation, 0.25f).SetEase(Ease.Linear));
            signalBus.Fire(new SetNewParentForCardSignal(this, false));
        }
    }
}