using System.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

namespace Card
{
    public class CardView
    {
        private readonly Image artImage;
        private readonly TMP_Text nameText;
        private readonly TMP_Text descriptionText;
        
        [Inject] private CardsContainer cardsContainer;

        public CardView(Image art, TMP_Text name, TMP_Text description)
        {
            artImage = art;
            nameText = name;
            descriptionText = description;
        }

        public void InitCardView()
        {
            CreatePicture();
            SetTexts();
        }

        private void SetTexts()
        {
            var cardInfo = cardsContainer.CardInfos[Random.Range(0, cardsContainer.CardInfos.Count)];
            nameText.SetText(cardInfo.name);
            descriptionText.SetText(cardInfo.description);
        }

        private void CreatePicture()
        {
            var rectSize = artImage.rectTransform.rect.size;
            var pictureSize = new Vector2(Mathf.RoundToInt(rectSize.x), Mathf.RoundToInt(rectSize.y));
            var sb = new StringBuilder();

            sb.Append("https://picsum.photos/");
            sb.Append(pictureSize.x);
            sb.Append("/");
            sb.Append(pictureSize.y);

            UniTask.Create(() => DownloadImage(sb.ToString()));
        }

        private async UniTask DownloadImage(string url)
        {
            using var request = UnityWebRequestTexture.GetTexture(url);

            await UniTask.Create(() => GetTextureAsync(request));

            var texture2D = DownloadHandlerTexture.GetContent(request);
            var sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
                Vector2.one * 0.5f);

            artImage.sprite = sprite;
        }

        private async UniTask<Texture2D> GetTextureAsync(UnityWebRequest request)
        {
            await request.SendWebRequest();
            return request.result != UnityWebRequest.Result.Success ? DownloadHandlerTexture.GetContent(request) : null;
        }
    }
}