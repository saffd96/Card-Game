using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    public class CardData
    {
        private readonly CardStats baseCardStats;
        private readonly CardStats currentCardStats;
        
        public CardData(List<CardValue> values)
        {
            currentCardStats = baseCardStats = new CardStats(values);
        }

        public void InitCardStats()
        {
            currentCardStats.UpdateStatByType(CardValueType.Attack, Random.Range(1, 10));
            currentCardStats.UpdateStatByType(CardValueType.Health, Random.Range(1, 10));
            currentCardStats.UpdateStatByType(CardValueType.Mana, Random.Range(1, 10));
        }
        
        public void UpdateCardStatByType(CardValueType type, int amount)
        {
            currentCardStats.UpdateStatByType(type, amount);
        }

        public int GetStatByType(CardValueType type)
        {
            return currentCardStats.GetStatByType(type);
        }
        
        public CardStats GetCurrentCardStats()
        {
            return currentCardStats;
        }
        
        public CardStats GetBaseCardStats()
        {
            return baseCardStats;
        }
    }
}