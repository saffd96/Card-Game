using Card;
using UnityEngine;

namespace Zenject
{
    [CreateAssetMenu(fileName = "Prefabs")]

    public class Prefabs : ScriptableObject
    {
        public CardController cardController;
    }
}