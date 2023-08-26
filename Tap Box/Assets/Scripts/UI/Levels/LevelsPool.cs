using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Levels
{
    public class LevelsPool : MonoBehaviour
    {
        private const string LevelButtonAddressable = "LevelButton";
        [SerializeField] private List<UILevelItem> availableLevels;

        public async UniTask<UILevelItem> Get()
        {
            var result = availableLevels.Count == 0 ? null : availableLevels[0];

            if (result == null) 
                return await CreateLevelButton();
            
            availableLevels.Remove(result);
            result.SetActive(true);
            return result;
        }

        public void Back(UILevelItem item)
        {
            availableLevels.Add(item);
            item.SetActive(false);
            
        }

        private async UniTask<UILevelItem> CreateLevelButton()
        {
            var instantiateAssetAsync = await InstantiateAssetAsync(LevelButtonAddressable);
            return instantiateAssetAsync.GetComponent<UILevelItem>();
        }

        private async UniTask<GameObject> InstantiateAssetAsync(string assetName)
        {
            var x = await AssetProvider.LoadAssetAsync<GameObject>(assetName);
            return x == null ? null : Instantiate(x, null);
        }
    }
}