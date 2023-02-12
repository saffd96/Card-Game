using UnityEngine;

namespace Card
{
    public class CardValue : MonoBehaviour
    {
        [SerializeField] private CardValueType cardValueType;
        [SerializeField] private CounterText amountText;

        public CounterText AmountTextCounter => amountText;
        public CardValueType CardValueType => cardValueType;
        public int Amount => amount;
        
        private int amount;

        private void OnValidate()
        {
            amountText = GetComponentInChildren<CounterText>();
        }

        public void SetAmount(int newAmount)
        {
            amount = newAmount;
            amountText.SetTargetValue(amount, true);
        }
    }

    public enum CardValueType
    {
        Attack = 0,
        Health = 1,
        Mana = 2
    }}