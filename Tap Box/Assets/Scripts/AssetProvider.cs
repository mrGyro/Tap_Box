using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class AssetProvider
{
    public static async Task<T> LoadAssetAsync<T>(string key)
    {
        var handle = Addressables.LoadAssetAsync<T>(key);
        return await handle.Task;
    }

    public static bool AddressableResourceExists(object key, Type type)
    {
        foreach (var l in Addressables.ResourceLocators)
        {
            if (l.Locate(key, type, out _))
            {
                return true;
            }
        }

        return false;
    }
}