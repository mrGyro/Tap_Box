using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LevelCreator;
using UnityEngine;

namespace UI.Levels
{
    public class LevelsPool : MonoBehaviour
    {
        private const string AddressablePassedLevelItem = "PassedLevelItem";
        private const string AddressableUnlockLevelItem = "UnlockableLevel";
        private const string AddressableLockLevelItem = "LockedLevelItem";

        [SerializeField] private List<UILevelItem> availableLevels;

        public async UniTask<UILevelItem> Get(Status status)
        {
            var result = availableLevels.FirstOrDefault(x => x.ButtonType == status);

            if (result == null) 
                return await CreateLevelButton(status);
            
            availableLevels.Remove(result);
            result.SetActive(true);
            return result;
        }

        public void Back(UILevelItem item)
        {
            availableLevels.Add(item);
            item.SetActive(false);
            
        }

        private async UniTask<UILevelItem> CreateLevelButton(Status status)
        {
            string asset = string.Empty;

            switch (status)
            {
                case Status.None:
                    break;
                case Status.Open:
                    asset = AddressableUnlockLevelItem;
                    break;
                case Status.Passed:
                    asset = AddressablePassedLevelItem;
                    break;
                case Status.Close:
                    asset = AddressableLockLevelItem;
                    break;
            }

            var g = await InstantiateAssetAsync(asset);
            return g.GetComponent<UILevelItem>();
        }

        private async UniTask<GameObject> InstantiateAssetAsync(string assetName)
        {
            var x = await AssetProvider.LoadAssetAsync<GameObject>(assetName);
            return x == null ? null : Instantiate(x, null);
        }
    }
}