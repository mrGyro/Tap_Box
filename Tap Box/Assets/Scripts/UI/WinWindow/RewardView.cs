using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI.WinWindow
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
            _getSprite = await AssetProvider.LoadAssetAsync<Sprite>(setting.RewardType.ToString());
            _notGetSprite = await AssetProvider.LoadAssetAsync<Sprite>(setting.RewardType + "_notGet");
            rewardProgressImage.sprite = await AssetProvider.LoadAssetAsync<Sprite>(setting.RewardType.ToString());
            SetActiveVFX(false);
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
            rewardPercents.text = $"{text}%";
        }
    }
}