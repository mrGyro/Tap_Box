using System.Collections.Generic;
using System.Linq;
using Boxes;
using Boxes.SwipableBox;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameField : MonoBehaviour
{
    public static GameField Instance;
    public InputController InputController;
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
            if (data.Type == BaseBox.BlockType.None)
                continue;

            var boxGameObject = await InstantiateAssetAsync(data.Type.ToString());
            var box = boxGameObject.GetComponent<BaseBox>();
            box.transform.position = data.ArrayPosition * size;
            box.transform.rotation = Quaternion.Euler(data.Rotation);
            box.Data = data;
            _boxes.Add(box);
        }
    }

    public int GetBoxesCount()
    {
        return _boxes.Count;
    }

    public bool ExistBox(BaseBox box)
    {
        return _boxes.Exists(x=> x.Data.ArrayPosition == box.Data.ArrayPosition);
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

    public List<BaseBox> GetNearestBoxesLine(BaseBox box, BaseBox.BlockType type)
    {
        var line = new List<BaseBox> { box };
        var nearestBoxes = GetNearestBoxes(box, type);

        var buffer = new List<BaseBox>(nearestBoxes);
        while (buffer.Count > 0)
        {
            List<BaseBox> list = new List<BaseBox>();
            for (var index = 0; index < buffer.Count; index++)
            {
                if (!nearestBoxes.Exists(x => x == buffer[index]))
                {
                    nearestBoxes.Add(buffer[index]);
                }

                foreach (var VARIABLE in GetNearestBoxes(buffer[index], buffer[index].Data.Type))
                {
                    if (!line.Exists(x => x == VARIABLE))
                        line.Add(VARIABLE);

                    if (!nearestBoxes.Exists(x => x == VARIABLE))
                        list.Add(VARIABLE);
                }
            }

            buffer = new List<BaseBox>(list);
        }

        return line;
    }

    public void SetActiveGlobalInput(bool value)
    {
        InputController.SetActiveInput(value);
    }

    public List<Vector3> EmptyPositionBetweenTwoBoxes(Vector3 destination, Vector3 origin)
    {
        Vector3 direction = (destination - origin).normalized;

        int count = 0;
        Vector3 current = origin;
        List<Vector3> result = new List<Vector3>();

        while (current != destination && count < 100)
        {
            current += direction;
            if (current == destination)
                break;
            result.Add(current);
            count++;
        }

        return result;
    }

    private BaseBox GetBoxFromArrayPosition(Vector3 position)
        => _boxes.FirstOrDefault(x => x.Data.ArrayPosition == position);

    private List<BaseBox> GetNearestBoxes(BaseBox box, BaseBox.BlockType type = BaseBox.BlockType.None)
    {
        var positions = new List<Vector3>
        {
            box.Data.ArrayPosition + Vector3.up,
            box.Data.ArrayPosition + Vector3.down,
            box.Data.ArrayPosition + Vector3.forward,
            box.Data.ArrayPosition + Vector3.back,
            box.Data.ArrayPosition + Vector3.left,
            box.Data.ArrayPosition + Vector3.right
        };

        var result = new List<BaseBox>();
        foreach (var pos in positions)
        {
            var nearBox = GetBoxFromArrayPosition(pos);
            if (nearBox != null)
            {
                if (type != BaseBox.BlockType.None && nearBox.Data.Type != type)
                    continue;
                result.Add(nearBox);
            }
        }

        return result;
    }

    public async UniTask<TapObject> CreateTapObject(string tapObjectName)
    {
        var boxGameObject = await InstantiateAssetAsync(tapObjectName);
        return boxGameObject == null ? null : boxGameObject.GetComponent<TapObject>();
    }

    private async UniTask<GameObject> InstantiateAssetAsync(string assetName)
    {
        var x = await AssetProvider.LoadAssetAsync<GameObject>(assetName);
        return x == null ? null : Instantiate(x, _rooTransform);
    }
}