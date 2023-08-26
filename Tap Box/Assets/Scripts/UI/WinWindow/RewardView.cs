using System.Linq;
using Currency;
using Cysharp.Threading.Tasks;
using DefaultNamespace.UI.WinWindow;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WinWindow
{
    public class RewardView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Image rewardProgressImage;
        [SerializeField] private GameObject reward;
        [SerializeField] private TMP_Text rewardPercents;
        [SerializeField] private GameObject VFX;
        private Sprite _notGetSprite;
        private Sprite _getSprite;

        public async UniTask Setup(RewardViewSetting setting)
        {
            var randomSkin = GameManager.Instance.Progress.SkinDatas.FirstOrDefault(skin => skin.WayToGet == CurrencyController.Type.RandomSkin && !skin.IsOpen);

            var rewardType = randomSkin == null ? CurrencyController.Type.Coin : setting.RewardType;
            _getSprite = await AssetProvider.LoadAssetAsync<Sprite>(rewardType + "_icon");
            _notGetSprite = await AssetProvider.LoadAssetAsync<Sprite>(rewardType + "_notGet");
            rewardProgressImage.sprite = _getSprite;
            SetActiveVFX(false);
        }

        public Transform GetCurrencyRoot()
        {
            return rewardProgressImage.transform;
        }

        public void SetRewardSprite(Sprite sprite)
        {
            rewardProgressImage.sprite = sprite;
        }

        public void SetActiveObject(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetActiveVFX(bool value)
        {
            VFX.SetActive(value);
        }

        public void SetActiveReward(bool value)
        {
            reward.SetActive(value);
        }

        public void SetTokState(bool value)
        {
            image.sprite = value ? _getSprite : _notGetSprite;
        }

        public void UpdateRewardPercentText(string text)
        {
            rewardPercents.text = $"{text}";
        }
    }
}