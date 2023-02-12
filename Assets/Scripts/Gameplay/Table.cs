using UnityEngine;

namespace Gameplay
{
    public class Table : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Transform container;

        public Transform Container => container;
        
        private void Awake()
        {
            boxCollider.size = rectTransform.sizeDelta;
        }
    }
}