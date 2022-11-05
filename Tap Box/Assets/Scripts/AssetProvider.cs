using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class AssetProvider
{
    public static async Task<T> LoadAssetAsync<T>(string key)
    {
        var handle = Addressables.LoadAssetAsync<T>(key);
        return await handle.Task;
    }
}