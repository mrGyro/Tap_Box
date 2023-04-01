using System;
using System.Collections.Generic;
using System.Linq;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using Boxes.SwipableBox;
using Cysharp.Threading.Tasks;
using LevelCreator;
using Unity.VisualScripting;
using UnityEngine;

public class GameField : MonoBehaviour, IInitializable
{
    [SerializeField] private float size;

    private Transform _rootTransform;
    private Vector3 _maxLevelSize;
    private Vector3 _minLevelSize;
    private List<BaseBox> _boxes;
    private LevelData _data;

    private int _boxesCount;

    public void Initialize()
    {
        _rootTransform = transform;
        _boxes = new List<BaseBox>();
    }

    public int GetBoxCount
    {
        get { return _boxesCount; }
        set
        {
            _boxesCount = value;


            if (_boxesCount == 0 && _boxes.Count != 0)
            {
                _boxesCount = 0;
                Managers.Instance.UIManager.ShowPopUp(Constants.PopUps.LosePopUp);
                Core.MessengerStatic.Messenger.Broadcast(Constants.Events.OnGameLoose);
            }

            Core.MessengerStatic.Messenger.Broadcast(Constants.Events.OnBoxClicked);
        }
    }

    public async UniTask LoadLevelByName(string levelName)
    {
        await CreateLevel(levelName);
    }

    public async void CheckForWin()
    {
        if (_boxes.Count == 0)
        {
            await UniTask.Delay(1000);
            Managers.Instance.UIManager.ShowPopUp(Constants.PopUps.WinPopUp);
            _data.LevelStatus = Status.Passed;
            _data.Data = null;
            Managers.Instance.SaveLevel(_data);

            return;
        }

        SetNewMaxMinSize();
    }

    private void SetNewCameraTargetPosition()
    {
        Vector3 newPosition = new Vector3(
            _maxLevelSize.x - (_maxLevelSize.x + Mathf.Abs(_minLevelSize.x)) / 2,
            _maxLevelSize.y - (_maxLevelSize.y + Mathf.Abs(_minLevelSize.y)) / 2,
            _maxLevelSize.z - (_maxLevelSize.z + Mathf.Abs(_minLevelSize.z)) / 2);

        Managers.Instance.InputController.SetStartLevelSettings(newPosition);
    }

    public bool ExistBox(Vector3 boxArrayPosition)
    {
        return _boxes.Exists(x => x.Data.ArrayPosition.ToVector3() == boxArrayPosition);
    }

    public Vector3 GetWorldPosition(Vector3 arrayPosition)
    {
        return arrayPosition * size;
    }

    public BaseBox GetNearestBoxInDirection(Vector3[] boxArrayPosition, Vector3 direction, BaseBox currentBox)
    {
        Vector3[] arrayPosition = boxArrayPosition;

        for (int i = 0; i < arrayPosition.Length; i++)
        {
            arrayPosition[i] += direction;
        }

        bool notOutOfRange = true;
        while (notOutOfRange)
        {
            BaseBox box = null;

            for (int i = 0; i < arrayPosition.Length; i++)
            {
                notOutOfRange = CheckMaxLevelSize(arrayPosition[i]) && CheckMinLevelSize(arrayPosition[i]);
                if (!notOutOfRange)
                {
                    return null;
                }
                
                foreach (var VARIABLE in _boxes)
                {
                    bool isBoxInPosition = false;
                    switch (VARIABLE.Data.Type)
                    {
                        case BaseBox.BlockType.None:
                        case BaseBox.BlockType.TapFlowBox:
                        case BaseBox.BlockType.RotateRoadBox:
                        case BaseBox.BlockType.SwipedBox:
                            isBoxInPosition = VARIABLE.IsBoxInPosition(arrayPosition[i]);
                            break;
                        case BaseBox.BlockType.BigBoxTapFlowBox:
                            var bigBox = (VARIABLE as BigBoxTapFlowBox);
                            if (bigBox != null)
                            {
                                isBoxInPosition = bigBox.IsBoxInPosition(arrayPosition[i]);
                            }
                            break;
                    }
                    if (isBoxInPosition && currentBox != box)
                    {
                        box = VARIABLE;
                        break;
                    }
                }
            }

            if (box != null && currentBox != box)
            {
                Debug.LogError(box.name);
                return box;
            }

            for (int i = 0; i < arrayPosition.Length; i++)
            {
                arrayPosition[i] += direction;
            }
        }

        return null;
    }

    public void RemoveBox(BaseBox box)
    {
        _boxes.Remove(box);
        Core.MessengerStatic.Messenger<BaseBox>.Broadcast(Constants.Events.OnBoxRemoveFromGameField, box);
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

    public async UniTask ChangeSkin()
    {
        var list = new List<BoxData>();
        foreach (var box in _boxes)
            list.Add(box.Data);

        await CreateBoxes(list);
    }

    public List<BoxData> GetDataForSave()
    {
        var result = new List<BoxData>();
        foreach (var baseBox in _boxes)
        {
            result.Add(baseBox.Data);
        }

        return result.Count == 0 ? null : result;
    }


    private void ClearGameField()
    {
        for (int i = _boxes.Count - 1; i >= 0; i--)
        {
            Destroy(_boxes[i].gameObject);
        }

        _boxes.Clear();
    }

    private async UniTask CreateLevel(string levelName)
    {
        _data = await Managers.Instance.Progress.LoadLevelData(levelName);

        var level = Managers.Instance.Progress.LevelDatas.FirstOrDefault(x => x.ID == Managers.Instance.Progress.LastStartedLevelID);

        if (level != null && level.Data != null)
        {
            await CreateBoxes(level.Data);
        }
        else
        {
            await CreateBoxes(_data.Data);
        }

        SetNewMaxMinSize();
        SetNewCameraTargetPosition();
        GetBoxCount = _boxes.Count + AddedTurns();
    }

    private int AddedTurns()
    {
        var count = _boxes.Count * 0.05f;
        count = Mathf.Clamp(count, 1, 100);
        return (int)count;
    }

    private async UniTask CreateBoxes(List<BoxData> dataList)
    {
        ClearGameField();
        _boxes = new List<BaseBox>();

        foreach (var data in dataList)
        {
            if (data.Type == BaseBox.BlockType.None)
                continue;

            var boxGameObject = await InstantiateAssetAsync($"{Managers.Instance.Progress.CurrentSkin}_{data.Type}");
            var box = boxGameObject.GetComponent<BaseBox>();
            box.transform.position = data.ArrayPosition.ToVector3() * size;
            box.transform.rotation = Quaternion.Euler(data.Rotation);
            box.Data = data;
            box.name = box.Data.Type + "_" + _boxes.Count;
            _boxes.Add(box);
        }

        foreach (var baseBox in _boxes)
        {
            await baseBox.Init();
        }
    }

    public BaseBox GetBoxFromArrayPosition(Vector3 position)
        => _boxes.FirstOrDefault(x => x.Data.ArrayPosition.ToVector3() == position);

    public async UniTask<TapObject> CreateTapObject(string tapObjectName)
    {
        var boxGameObject = await InstantiateAssetAsync(tapObjectName);
        return boxGameObject == null ? null : boxGameObject.GetComponent<TapObject>();
    }

    private void SetNewMaxMinSize()
    {
        _maxLevelSize = Vector3.negativeInfinity;
        _minLevelSize = Vector3.positiveInfinity;
        foreach (var baseBox in _boxes)
        {
            SetMaxLevelSize(baseBox.Data.ArrayPosition);
            SetMinLevelSize(baseBox.Data.ArrayPosition);
        }
    }

    private void SetMaxLevelSize(Vector3 arrayPosition)
    {
        _maxLevelSize.x = _maxLevelSize.x < arrayPosition.x ? arrayPosition.x : _maxLevelSize.x;
        _maxLevelSize.y = _maxLevelSize.y < arrayPosition.y ? arrayPosition.y : _maxLevelSize.y;
        _maxLevelSize.z = _maxLevelSize.z < arrayPosition.z ? arrayPosition.z : _maxLevelSize.z;
    }

    private void SetMinLevelSize(Vector3 arrayPosition)
    {
        _minLevelSize.x = _minLevelSize.x > arrayPosition.x ? arrayPosition.x : _minLevelSize.x;
        _minLevelSize.y = _minLevelSize.y > arrayPosition.y ? arrayPosition.y : _minLevelSize.y;
        _minLevelSize.z = _minLevelSize.z > arrayPosition.z ? arrayPosition.z : _minLevelSize.z;
    }

    private bool CheckMaxLevelSize(Vector3 arrayPosition)
    {
        if ((int)_maxLevelSize.x < (int)arrayPosition.x)
        {
            return false;
        }

        if ((int)_maxLevelSize.y < (int)arrayPosition.y)
        {
            return false;
        }

        if ((int)_maxLevelSize.z < (int)arrayPosition.z)
        {
            return false;
        }

        return true;
    }

    private bool CheckMinLevelSize(Vector3 arrayPosition)
    {
        if ((int)_minLevelSize.x > (int)arrayPosition.x)
        {
            return false;
        }

        if ((int)_minLevelSize.y > (int)arrayPosition.y)
        {
            return false;
        }

        if ((int)_minLevelSize.z > (int)arrayPosition.z)
        {
            return false;
        }

        return true;
    }

    private async UniTask<GameObject> InstantiateAssetAsync(string assetName)
    {
        var x = await AssetProvider.LoadAssetAsync<GameObject>(assetName);
        return x == null ? null : Instantiate(x, _rootTransform);
    }
}