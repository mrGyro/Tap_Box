﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

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
            IList<IResourceLocation> locs;
            if (l.Locate(key, type, out locs))
                return true;
        }

        return false;
    }
}