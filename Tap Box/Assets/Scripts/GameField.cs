using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Boxes;
using Boxes.SwipableBox;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

public class GameField : MonoBehaviour
{
    public static GameField Instance;
    public InputController InputController;
    [SerializeField] private Transform _rooTransform;
    [SerializeField] private float size;
    [SerializeField] private GameObject winPanel;

    [SerializeField] private List<BaseBox> _boxes;
    [SerializeField] private LevelData _datas;

    private int _currentLevelIndex = 1;

    private Vector3 MaxLevelSize;
    private Vector3 MinLevelSize;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        CreateLevel("Level_" + _currentLevelIndex);
    }

    public void LoadNextLevel()
    {
        _currentLevelIndex++;
        CreateLevel("Level_" + _currentLevelIndex);
    }

    public void CheckForWin()
    {
        if (_boxes.Count == 0)
        {
            winPanel.SetActive(true);
        }
    }

    public bool ExistBox(Vector3 boxArrayPosition)
    {
        return _boxes.Exists(x => x.Data.ArrayPosition.ToVector3() == boxArrayPosition);
    }

    public BaseBox GetBoxByArrayPosition(Vector3 boxArrayPosition)
    {
        return _boxes.FirstOrDefault(x => x.Data.ArrayPosition.ToVector3() == boxArrayPosition);
    }

    public Vector3 GetWorldPosition(Vector3 arrayPosition)
    {
        return arrayPosition * size;
    }

    public Vector3 GetIndexByWorldPosition(Vector3 worldPosition)
    {
        return worldPosition / size;
    }

    public BaseBox GetNearestBoxInDirection(Vector3 boxArrayPosition, Vector3 direction)
    {
        Vector3 currentPosition = boxArrayPosition;
        currentPosition += direction;
        while (CheckMaxLevelSize(currentPosition) && CheckMinLevelSize(currentPosition))
        {
            var box = GetBoxByArrayPosition(currentPosition);
            if (box != null)
            {
                return box;
            }

            currentPosition += direction;
        }

        return null;
    }

    public void RemoveBox(BaseBox box)
    {
        _boxes.Remove(box);
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

    private async void CreateLevel(string levelName)
    {
        _boxes = new List<BaseBox>();
        _datas = await LoadLevelData(levelName);
        foreach (var data in _datas.Data)
        {
            if (data.Type == BaseBox.BlockType.None)
                continue;

            var boxGameObject = await InstantiateAssetAsync(data.Type.ToString());
            var box = boxGameObject.GetComponent<BaseBox>();
            box.transform.position = data.ArrayPosition.ToVector3() * size;
            box.transform.rotation = Quaternion.Euler(data.Rotation);
            box.Data = data;
            _boxes.Add(box);

            SetMaxLevelSize(box.Data.ArrayPosition);
            SetMinLevelSize(box.Data.ArrayPosition);
        }
    }

    public BaseBox GetBoxFromArrayPosition(Vector3 position)
        => _boxes.FirstOrDefault(x => x.Data.ArrayPosition.ToVector3() == position);

    public async UniTask<TapObject> CreateTapObject(string tapObjectName)
    {
        var boxGameObject = await InstantiateAssetAsync(tapObjectName);
        return boxGameObject == null ? null : boxGameObject.GetComponent<TapObject>();
    }

    private void SetMaxLevelSize(Vector3 arrayPosition)
    {
        MaxLevelSize.x = MaxLevelSize.x < arrayPosition.x ? arrayPosition.x : MaxLevelSize.x;
        MaxLevelSize.y = MaxLevelSize.y < arrayPosition.y ? arrayPosition.y : MaxLevelSize.y;
        MaxLevelSize.z = MaxLevelSize.z < arrayPosition.z ? arrayPosition.z : MaxLevelSize.z;
    }

    private void SetMinLevelSize(Vector3 arrayPosition)
    {
        MinLevelSize.x = MinLevelSize.x > arrayPosition.x ? arrayPosition.x : MinLevelSize.x;
        MinLevelSize.y = MinLevelSize.y > arrayPosition.y ? arrayPosition.y : MinLevelSize.y;
        MinLevelSize.z = MinLevelSize.z > arrayPosition.z ? arrayPosition.z : MinLevelSize.z;
    }

    private bool CheckMaxLevelSize(Vector3 arrayPosition)
    {
        if (MaxLevelSize.x < arrayPosition.x)
            return false;
        if (MaxLevelSize.y < arrayPosition.y)
            return false;
        if (MaxLevelSize.z < arrayPosition.z)
            return false;

        return true;
    }

    private bool CheckMinLevelSize(Vector3 arrayPosition)
    {
        if (MinLevelSize.x > arrayPosition.x)
            return false;
        if (MinLevelSize.y > arrayPosition.y)
            return false;
        if (MinLevelSize.z > arrayPosition.z)
            return false;

        return true;
    }

    private async UniTask<GameObject> InstantiateAssetAsync(string assetName)
    {
        var x = await AssetProvider.LoadAssetAsync<GameObject>(assetName);
        return x == null ? null : Instantiate(x, _rooTransform);
    }

    private async UniTask<LevelData> LoadLevelData(string assetName)
    {
        var x = await AssetProvider.LoadAssetAsync<TextAsset>(assetName);
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(x.bytes, 0, x.bytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        LevelData obj = (LevelData)binForm.Deserialize(memStream);
        return obj;
    }
}