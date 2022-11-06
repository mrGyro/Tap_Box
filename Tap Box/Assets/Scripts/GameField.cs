using System;
using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    public static GameField Instance;
    [SerializeField] private Transform _rooTransform;
    [SerializeField] private float size;

    [SerializeField] private List<BaseBox> _boxes;
    [SerializeField] private List<BoxData> _datas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    async void Start()
    {
        _boxes = new List<BaseBox>();
        foreach (var data in _datas)
        {
            var x = await AssetProvider.LoadAssetAsync<GameObject>(data.Type.ToString());
            var y = Instantiate(x, _rooTransform);
            var a = y.GetComponent<BaseBox>();
            a.transform.position = data.ArrayPosition * size;
            a.transform.rotation = Quaternion.Euler(data.Rotation);
            a.Data = data;

            _boxes.Add(a);
        }
    }

    public Vector3 GetWorldPosition(Vector3 arrayPosition)
    {
        return arrayPosition * size;
    }
    
    public Vector3 GetIndexByWorldPosition(Vector3 worldPosition)
    {
        return worldPosition / size;
    }

    public void RemoveBox(BaseBox box)
    {
        _boxes.Remove(box);
    }
}
