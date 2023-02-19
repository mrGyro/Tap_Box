using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI.WinWindow
{
    public class RewardView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private RewardViewSetting setting;
        
        public async void Setup(RewardViewSetting setting)
        {
            image.sprite = await AssetProvider.LoadAssetAsync<Sprite>(setting.RewardType.ToString());
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}