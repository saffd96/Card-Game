using System.Collections.Generic;
using Card;

namespace Zenject.Signals
{
    public readonly struct RemoveCardSignal
    {
        public readonly CardController CardToRemove;

        public RemoveCardSignal(CardController cardToRemove)
        {
            CardToRemove = cardToRemove;
        }
    }
    
    public readonly struct RemoveCardFromHandSignal
    {
        public readonly CardController CardToRemove;

        public RemoveCardFromHandSignal(CardController cardToRemove)
        {
            CardToRemove = cardToRemove;
        }
    }

    public readonly struct SetNewParentForCardSignal
    {
        public readonly CardController Card;
        public readonly bool IsTopParentNeed;

        public SetNewParentForCardSignal(CardController card, bool isTopParentNeed)
        {
            Card = card;
            IsTopParentNeed = isTopParentNeed;
        }
    }
    
    public readonly struct SelectCardSignal
    {
        public readonly CardController Card;

        public SelectCardSignal(CardController card)
        {
            Card = card;
        }
    }
    
    public readonly struct UpdateCardListSignal
    {
        public readonly List<CardController> Cards;

        public UpdateCardListSignal(List<CardController> cards)
        {
            Cards = cards;
        }
    }
}