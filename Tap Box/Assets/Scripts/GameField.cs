using System;
using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    [SerializeField] private Transform _rooTransform;
    [SerializeField] private float size;

    [SerializeField] private List<BaseBox> _boxes;
    [SerializeField] private List<BoxData> _datas;
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

    [Serializable]
    public class BoxData
    {
        public BaseBox.BlockType Type;
        public Vector3 ArrayPosition;
        public Vector3 Rotation;
    }
}
