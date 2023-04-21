using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AssetProvider
{
    public static async Task<T> LoadAssetAsync<T>(string key)
    {
        try
        {
 
            var handle = Addressables.LoadAssetAsync<T>(key);
            return await handle.Task;
        }
        catch (Exception e)
        {
            return await Task.FromResult<T>(default);
        }
    }
    
    public static bool AddressableResourceExists(object key, Type type)
    {
        foreach (var l in Addressables.ResourceLocators) {
            IList<IResourceLocation> locs;
            if (l.Locate(key, type, out locs))
                return true;
        }
        return false;
    }

    // public static bool AddressableResourceExists<T>(object key)
    // {
    //     var x =  Addressables.ResourceLocators.OfType<ResourceLocationMap>()
    //         .SelectMany(locationMap =>
    //             locationMap.Locations.Keys.Select(key => key.ToString())
    //         );
    //     
    //     return string.IsNullOrEmpty(x.t)
    // }
}