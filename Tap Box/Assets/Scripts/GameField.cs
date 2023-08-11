using System;
using System.Collections.Generic;
using System.Linq;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using Boxes.SwipableBox;
using Boxes.TapFlowBox;
using Currency;
using Cysharp.Threading.Tasks;
using LevelCreator;
using Managers;
using UI.Skins;
using Unity.VisualScripting;
using UnityEngine;

public class GameField : MonoBehaviour, IInitializable
{
    public static float Size = 1.08f;

    private Transform _rootTransform;
    private Vector3 _maxLevelSize;
    private Vector3 _minLevelSize;
    private List<BaseBox> _boxes;
    private LevelData _data;

    private int _turnsesCount;

    public void Initialize()
    {
        _rootTransform = transform;
        _boxes = new List<BaseBox>();
    }

    public int GetTurnsCount
    {
        get => _turnsesCount;
        set => _turnsesCount = value;
    }

    public int GetCountOfReward()
    {
        return _data.Reward;
    }

    public void BombBox(BaseBox box, Vector3 point, Vector3 size)
    {
        if (box == null)
        {
            return;
        }

        Vector3 boxPartPosition = box.transform.position;
        if (box is BigBoxTapFlowBox)
        {
            var bigBox = box as BigBoxTapFlowBox;
            var parts = bigBox.GetBoxPositions();
            float minDistance = float.MaxValue;
            foreach (var bigBoxPart in parts)
            {
                if (Vector3.Distance(bigBoxPart.transform.position, point) < minDistance)
                {
                    boxPartPosition = bigBoxPart.transform.position;
                }
            }
        }

        Vector3 minPos = boxPartPosition / Size - size;
        Vector3 maxPos = boxPartPosition / Size + size;

        List<Vector3> positions = new List<Vector3>();
        for (int x = (int)minPos.x; x <= maxPos.x; x++)
        {
            for (int y = (int)minPos.y; y <= maxPos.y; y++)
            {
                for (int z = (int)minPos.z; z <= maxPos.z; z++)
                {
                    positions.Add(new Vector3(x, y, z));
                }
            }
        }

        List<BaseBox> boxesForRemove = new List<BaseBox>();
        foreach (var arrayPosition in positions)
        {
            var boxInPosition = GetBoxInPosition(arrayPosition);

            if (boxInPosition != null && !boxesForRemove.Exists(x => x == boxInPosition))
            {
                boxesForRemove.Add(boxInPosition);
            }
        }

        if (boxesForRemove.Count == 0)
        {
            return;
        }

        GameManager.Instance.CurrencyController.RemoveCurrency(CurrencyController.Type.Coin, GameManager.Instance.UIManager.GetBombCos());
        GameManager.Instance.UIManager.ClickOnBomb();

        foreach (var baseBox in boxesForRemove)
        {
            BombRemoveBlock(baseBox);
        }
    }

    public int GetTurnsCountAfterLoose()
    {
        int additionalCount = (int)(_boxes.Count * 0.15f);
        additionalCount = Mathf.Clamp(additionalCount, 5, 50);
        return _boxes.Count + additionalCount;
    }

    private BaseBox GetBoxInPosition(Vector3 arrayPosition)
    {
        foreach (var variable in _boxes)
        {
            switch (variable.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    if (variable.IsBoxInPosition(arrayPosition))
                        return variable;
                    continue;
                case BaseBox.BlockType.BigBoxTapFlowBox:
                    var bigBox = (variable as BigBoxTapFlowBox);
                    if (bigBox != null)
                    {
                        if (bigBox.IsBoxInPosition(arrayPosition))
                            return bigBox;
                    }

                    continue;
            }
        }

        return null;
    }

    private async void BombRemoveBlock(BaseBox box)
    {
        Vector3 dif = new Vector3(0.03f, 0.03f, 0.03f);
        while (Vector3.Distance(box.transform.localScale, Vector3.zero) > 0.1f)
        {
            await UniTask.Yield();
            box.transform.localScale -= dif;
        }

        RemoveBox(box);
        Destroy(box.gameObject);
    }

    public string GetCurrentLevelID()
    {
        return _data.ID;
    }

    public async UniTask LoadLevelByName(string levelName)
    {
        await CreateLevel(levelName);
    }

    public void DoCheatRemoveAction()
    {
        if (_isActiveRemove && _isDown)
        {
            RemoveAvailableBox();
        }
    }

    public void DoDown()
    {
        _isDown = true;
    }

    public void DoUp()
    {
        _isDown = false;
    }

    private async void CheckForWin()
    {
        if (_boxes.Count == 0)
        {
            await ShowWinWindow();
        }
    }

    private bool _isActiveRemove = true;
    private bool _isDown = false;
#if UNITY_EDITOR
    private void Update()
    {
        if (_isActiveRemove && Input.GetKey(KeyCode.Q))
        {
            RemoveAvailableBox();
        }
    }
#endif

    public void RemoveAvailableBox()
    {
        _isActiveRemove = false;
        if (_boxes == null || _boxes.Count == 0)
        {
            _isActiveRemove = true;

            return;
        }

        BaseBox box = null;
        foreach (var baseBox in _boxes)
        {
            var bigBoxTapFlowBox = baseBox as BigBoxTapFlowBox;
            if (bigBoxTapFlowBox != null)
            {
                Vector3[] array = bigBoxTapFlowBox.GetBoxWorldPositionsAsVectors();
                var box2 = GameManager.Instance.GameField.GetNearestBoxInDirection(array, bigBoxTapFlowBox.GetDirection(), baseBox);

                if (box2 == null)
                {
                    box = baseBox;
                    break;
                }
            }
            else
            {
                var nearestBoxInDirection = GameManager.Instance.GameField.GetNearestBoxInDirection(
                    new[] { baseBox.transform.position },
                    baseBox.GetComponent<FlowAwayReaction>().GetDirection(),
                    baseBox);

                if (nearestBoxInDirection == null)
                {
                    box = baseBox;
                    break;
                }
            }
        }

        if (box == null)
        {
            return;
        }

        GameManager.Instance.InputController.RemoveBox(box);
        _isActiveRemove = true;
    }

    private async UniTask ShowWinWindow()
    {
        await UniTask.Delay(1000);
        GameManager.Instance.UIManager.ShowPopUp(Constants.PopUps.WinPopUp);
        _data.LevelStatus = Status.Passed;
        _data.Data = null;
        _data.BestResult = _turnsesCount;
        GameManager.Instance.SaveLevel(_data);
    }

    private void SetNewCameraTargetPosition(Vector3 target, Vector3 cameraPosition)
    {
        GameManager.Instance.InputController.SetStartLevelSettings(target, cameraPosition);
    }

    public Vector3 GetNewCenter()
    {
        SetNewMaxMinSize();
        // Debug.DrawLine(_minLevelSize, _maxLevelSize, Color.red, 10);
        float x = _maxLevelSize.x + (_minLevelSize.x - _maxLevelSize.x) / 2;
        float y = _maxLevelSize.y + (_minLevelSize.y - _maxLevelSize.y) / 2;
        float z = _maxLevelSize.z + (_minLevelSize.z - _maxLevelSize.z) / 2;
        Vector3 newPosition = new Vector3(x, y, z);

        return newPosition;
    }

    public bool IsNotWinCondition()
    {
        return GetTurnsCount == 0 && _boxes.Count != 0;
    }

    public bool ExistBox(Vector3 boxArrayPosition)
    {
        return _boxes.Exists(x => x.Data.ArrayPosition.ToVector3() == boxArrayPosition);
    }

    public Vector3 GetWorldPosition(Vector3 arrayPosition)
    {
        return arrayPosition * Size;
    }

    public BaseBox GetNearestBoxInDirection(Vector3[] boxArrayPosition, Vector3 direction, BaseBox currentBox)
    {
        LayerMask mask = LayerMask.GetMask("GameFieldElement");
        Transform results = null;
        float minDistance = float.MaxValue;
        var currentBoxGameObject = currentBox.gameObject;
        int layer = currentBoxGameObject.layer;
        currentBoxGameObject.layer = 0;

        foreach (var variable in boxArrayPosition)
        {
            if (!Physics.Raycast(variable, direction * Size * 1000, out var hitBox, Mathf.Infinity, mask))
            {
                continue;
            }

            if (hitBox.transform == null)
            {
                continue;
            }

            float distance = Vector3.Distance(variable, hitBox.point);
            if (distance < Size)
            {
                results = hitBox.transform;
                break;
            }

            if (distance < minDistance)
            {
                minDistance = distance;
                results = hitBox.transform;
            }
        }

        currentBox.gameObject.layer = layer;
        return results == null ? null : results.GetComponent<BaseBox>();
    }

    public void RemoveBox(BaseBox box)
    {
        _boxes.Remove(box);

        Core.MessengerStatic.Messenger<BaseBox>.Broadcast(Constants.Events.OnBoxRemoveFromGameField, box);
        GameManager.Instance.GameField.CheckForWin();
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
        GameManager.Instance.UIManager.ShowUpToAllPopUp(Constants.PopUps.LoadingPopup);

        _data = await GameManager.Instance.Progress.LoadLevelData(levelName);

        var level = GameManager.Instance.Progress.LevelDatas.FirstOrDefault(x => x.ID == GameManager.Instance.Progress.LastStartedLevelID);

        if (level != null && GameManager.Instance.Progress.LastSavedLevelDataID == _data.ID && GameManager.Instance.Progress.LastLevelData != null)
        {
            await CreateBoxes(GameManager.Instance.Progress.LastLevelData);
            GetTurnsCount = GameManager.Instance.Progress.CurrentLevelTurnsLeftValue;
        }
        else
        {
            await CreateBoxes(_data.Data);
            GetTurnsCount = _boxes.Count + AddedTurns();
        }

        SetNewMaxMinSize();

        SetNewCameraTargetPosition(GetNewCenter(), _data.CameraPosition.ToVector3());
        GameManager.Instance.UIManager.ClosePopUp(Constants.PopUps.LoadingPopup);
    }

    private int AddedTurns()
    {
        var count = _boxes.Count * 0.08f;
        count = Mathf.Clamp(count, 5, 100);
        return (int)count;
    }

    private async UniTask CreateBoxes(List<BoxData> dataList)
    {
        ClearGameField();
        _boxes = new List<BaseBox>();
        var skinPopup = GameManager.Instance.UIManager.GetPopupByID("SkinsPopUp");
        if (skinPopup)
        {
            (skinPopup as SkinsPoUp)?.SetCurrentSize();
        }

        for (var i = 0; i < dataList.Count; i++)
        {
            var data = dataList[i];
            if (data.Type == BaseBox.BlockType.None)
                continue;

            var boxGameObject = await InstantiateAssetAsync(GetAddressableName(data));
            var box = boxGameObject.GetComponent<BaseBox>();
            box.transform.position = data.ArrayPosition.ToVector3() * Size;
            box.transform.rotation = Quaternion.Euler(data.Rotation);
            box.Data = data;
            box.name = box.Data.Type + "_" + _boxes.Count;
            _boxes.Add(box);

            // if (i % 10 == 0)
            // {
            //     await UniTask.Yield();
            //     GameManager.Instance.UIManager.ShowTurns();
            // }
        }

        for (var index = 0; index < _boxes.Count; index++)
        {
            var baseBox = _boxes[index];
            await baseBox.Init();
            if (index % 10 == 0)
            {
                await UniTask.Yield();
            }
        }
    }

    private string GetAddressableName(BoxData box)
    {
        switch (box.Type)
        {
            case BaseBox.BlockType.None:
            case BaseBox.BlockType.TapFlowBox:
            case BaseBox.BlockType.RotateRoadBox:
            case BaseBox.BlockType.SwipedBox:
                return $"{GameManager.Instance.Progress.CurrentBoxSkin}_{box.Type}";
            case BaseBox.BlockType.BigBoxTapFlowBox:
                return $"{GameManager.Instance.Progress.CurrentBoxSkin}_{box.Type}_{box.Size.x}_{box.Size.y}_{box.Size.z}";
        }

        return String.Empty;
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
        _maxLevelSize = _boxes.Count == 0 ? Vector3.zero : _boxes[0].Data.ArrayPosition;
        _minLevelSize = _boxes.Count == 0 ? Vector3.zero : _boxes[0].Data.ArrayPosition;
        foreach (var baseBox in _boxes)
        {
            switch (baseBox.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    SetMaxLevelSize(baseBox.Data.ArrayPosition);
                    SetMinLevelSize(baseBox.Data.ArrayPosition);
                    break;
                case BaseBox.BlockType.BigBoxTapFlowBox:
                    var bigBox = (baseBox as BigBoxTapFlowBox);
                    if (bigBox != null)
                    {
                        var positions = bigBox.GetBoxPositions();
                        foreach (var pos in positions)
                        {
                            SetMaxLevelSize(pos.ArrayPosition);
                            SetMinLevelSize(pos.ArrayPosition);
                        }
                    }

                    break;
            }
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
        if ((int)_maxLevelSize.x + 1 < (int)arrayPosition.x)
        {
            return false;
        }

        if ((int)_maxLevelSize.y + 1 < (int)arrayPosition.y)
        {
            return false;
        }

        if ((int)_maxLevelSize.z + 1 < (int)arrayPosition.z)
        {
            return false;
        }

        return true;
    }

    private bool CheckMinLevelSize(Vector3 arrayPosition)
    {
        if ((int)_minLevelSize.x - 1 > (int)arrayPosition.x)
        {
            return false;
        }

        if ((int)_minLevelSize.y - 1 > (int)arrayPosition.y)
        {
            return false;
        }

        if ((int)_minLevelSize.z - 1 > (int)arrayPosition.z)
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