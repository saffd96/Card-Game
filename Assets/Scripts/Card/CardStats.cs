using System.Collections.Generic;
using System.Linq;

namespace Card
{
    public class CardStats
    {
        private readonly List<CardValue> cardValues;

        public CardStats(List<CardValue> values)
        {
            cardValues = values;
        }

        public void UpdateStatByType(CardValueType type, int newAmount)
        {
            cardValues.FirstOrDefault(x => x.CardValueType == type)?.SetAmount(newAmount);
        }

        public int GetStatByType(CardValueType type)
        {
            return cardValues.FirstOrDefault(x => x.CardValueType == type).Amount;
        }
    }
}