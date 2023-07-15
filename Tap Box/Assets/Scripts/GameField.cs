using System;
using System.Collections.Generic;
using System.Linq;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using Boxes.SwipableBox;
using Boxes.TapFlowBox;
using Cysharp.Threading.Tasks;
using LevelCreator;
using LevelCreator.Validator;
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

    public string GetCurrentLevelID()
    {
        return _data.ID;
    }

    public async UniTask LoadLevelByName(string levelName)
    {
        await CreateLevel(levelName);
    }

    private async void CheckForWin()
    {
        if (_boxes.Count == 0)
        {
            await ShowWinWindow();
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemoveAvailableBox();
        }
    }
#endif

    public void RemoveAvailableBox()
    {
        if (_boxes == null || _boxes.Count == 0)
        {
            return;
        }

        BaseBox box = null;
        foreach (var VARIABLE in _boxes)
        {
            var bigBoxTapFlowBox = VARIABLE as BigBoxTapFlowBox;
            if (bigBoxTapFlowBox != null)
            {
                Vector3[] array = bigBoxTapFlowBox.GetBoxWorldPositionsAsVectors();
                var box2 = GameManager.Instance.GameField.GetNearestBoxInDirection(array, bigBoxTapFlowBox.GetDirection(), VARIABLE);

                if (box2 == null)
                {
                    box = VARIABLE;
                    break;
                }
            }
            else
            {
                Debug.DrawRay(VARIABLE.transform.position, VARIABLE.GetComponent<FlowAwayReaction>().GetDirection() * Size, Color.red, 30, true);

                var nearesBox = GameManager.Instance.GameField.GetNearestBoxInDirection(
                    new[] { VARIABLE.transform.position },
                    VARIABLE.GetComponent<FlowAwayReaction>().GetDirection(),
                    VARIABLE);

                if (nearesBox == null)
                {
                    box = VARIABLE;
                    break;
                }
            }
        }

        if (box == null)
        {
            return;
        }

        GameManager.Instance.InputController.RemoveBox(box);
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
        int layer = currentBox.gameObject.layer;
        currentBox.gameObject.layer = 0;

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
        }
        else
        {
            await CreateBoxes(_data.Data);
        }

        SetNewMaxMinSize();
        GetTurnsCount = _boxes.Count + AddedTurns();

        //GameManager.Instance.InputController.SetCameraTarget(GetNewCenter());
        SetNewCameraTargetPosition(GetNewCenter(), _data.CameraPosition.ToVector3());
        GameManager.Instance.UIManager.ClosePopUp(Constants.PopUps.LoadingPopup);
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

            if (i % 10 == 0)
            {
                await UniTask.Yield();
                GameManager.Instance.UIManager.ShowTurns();
            }
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