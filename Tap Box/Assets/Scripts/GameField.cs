using System.Collections.Generic;
using System.Linq;
using Boxes;
using Boxes.SwipableBox;
using Cysharp.Threading.Tasks;
using LevelCreator;
using TMPro;
using UnityEngine;

public class GameField : MonoBehaviour
{
    [SerializeField] private Transform _rooTransform;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private float size;
    [SerializeField] private LevelsWindiw levelPanel;

    [SerializeField] private List<BaseBox> _boxes;
    [SerializeField] private LevelData _datas;

    private Vector3 _maxLevelSize;
    private Vector3 _minLevelSize;

    public void SetActiveLevelPanel(bool value)
    {
        levelPanel.gameObject.SetActive(value);
    }

    public void LoadLevelByName(string levelName)
    {
        ClearGameField();
        CreateLevel(levelName);
    }

    public void ClearGameField()
    {
        for (int i = _boxes.Count - 1; i >= 0; i--)
        {
            Destroy(_boxes[i].gameObject);
        }

        _boxes.Clear();
    }

    public async void CheckForWin()
    {
        if (_boxes.Count == 0)
        {
            await UniTask.Delay(1000);
            Game.Instance.WinWindow.SetActive(true);
            
            _datas.LevelStatus = Status.Passed;
            Game.Instance.SaveLevel(_datas);
            
            return;
        }

        SetNewMaxMinSize();
    }

    private void SetNewTargetPosition()
    {
        Vector3 newPosition = new Vector3(
            _maxLevelSize.x - (_maxLevelSize.x + Mathf.Abs(_minLevelSize.x)) / 2,
            _maxLevelSize.y - (_maxLevelSize.y + Mathf.Abs(_minLevelSize.y)) / 2,
            _maxLevelSize.z - (_maxLevelSize.z + Mathf.Abs(_minLevelSize.z)) / 2);

        Game.Instance.InputController.SetNewTargetPosition(newPosition);
    }

    public bool ExistBox(Vector3 boxArrayPosition)
    {
        return _boxes.Exists(x => x.Data.ArrayPosition.ToVector3() == boxArrayPosition);
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
            Debug.LogError((box == null) + " " + currentPosition + " " + direction);
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
        Game.Instance.InputController.SetActiveInput(value);
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
    
    private BaseBox GetBoxByArrayPosition(Vector3 boxArrayPosition)
    {
        return _boxes.FirstOrDefault(x => x.Data.ArrayPosition.ToVector3() == boxArrayPosition);
    }

    private async void CreateLevel(string levelName)
    {
        Debug.LogError(levelName);
        _datas = await Game.Instance.Progress.LoadLevelData(levelName);
        _boxes = new List<BaseBox>();
        
        var level = levelName.Remove(0, levelName.LastIndexOf('_') + 1);
        _levelText.text = "Level " + level;
        
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
        }

        foreach (var baseBox in _boxes)
        {
            await baseBox.Init();
        }

        SetNewMaxMinSize();
        SetNewTargetPosition();
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
        return x == null ? null : Instantiate(x, _rooTransform);
    }
}