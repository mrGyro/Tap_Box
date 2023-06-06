using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using Cysharp.Threading.Tasks;
using LevelCreator.Validator;
using TMPro;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class CreateFrom3dObject : MonoBehaviour
{
    [SerializeField] private GameObject _baseBoxPlace;
    [SerializeField] private float _minDistance;
    [SerializeField] private Transform _rootGameField;
    [SerializeField] private Transform _rootPool;
    [SerializeField] private Transform _objectsWithCollidersForCreate;
    [SerializeField] private TMP_Text _countOfEmptyCellsCountText;
    [SerializeField] private List<BoxProbability> _possibleBoxesProbability;

    private List<Collider> _colliders;
    private Vector3 _maxLevelSize = Vector3.negativeInfinity;
    private Vector3 _minLevelSize = Vector3.positiveInfinity;

    private List<Vector3Class> _arrayPositions = new();
    private List<GameObject> _gameObjects = new();

    private List<Vector3Class> _emptyArrayPositions = new();
    private List<BaseBox> _listOfPossibleBox;
    private List<BaseBox> _level;

    private static System.Random rng = new((int)DateTime.Now.Ticks & 0x0000FFFF);
    private float _size = 1.03f;
    private bool _isGeneratorProccess;

    [Serializable]
    public class Vector3Class
    {
        public Vector3 Value;
    }

    public void StopGenerator()
    {
        _isGeneratorProccess = false;
    }
    
    [ContextMenu("Tools/CreateGameFieldFromArrayPositions")]
    public async void CreateGameFieldFromArrayPositions()
    {
        _level = new List<BaseBox>();
        _emptyArrayPositions = _arrayPositions.ToList();
        Shuffle(_emptyArrayPositions);
        _listOfPossibleBox = GetPossibleBoxes();
        _isGeneratorProccess = true;

        var startTime = System.DateTime.UtcNow;
        while (_emptyArrayPositions.Count > 0 && _isGeneratorProccess)
        {
            await UniTask.Yield();
            int indexOfProbability = GetIndexForNextBox();
            await TryPutBoxToField(indexOfProbability);
            BeckBoxVariantsToDefaultPosition();
            _countOfEmptyCellsCountText.text = _emptyArrayPositions.Count.ToString();
        }

        _isGeneratorProccess = false;
        System.TimeSpan ts = System.DateTime.UtcNow - startTime;
        Debug.Log("--------------------------------------" + ts.TotalSeconds);
        _countOfEmptyCellsCountText.text = _emptyArrayPositions.Count.ToString();
    }

    private async UniTask TryPutBoxToField(int indexOfProbability)
    {
        int countOfOperations = 0;
        var defaultPosition = Vector3.one * 1000;
        for (int i = indexOfProbability; i >= 0; i--)
        {
            int firstIndexOfInstance = GetFirstIndexWithSize(_listOfPossibleBox, _possibleBoxesProbability[i].Box.Data.Size.ToVector3());
            int lastIndexOfInstance = GetLastIndexWithSize(_listOfPossibleBox, _possibleBoxesProbability[i].Box.Data.Size.ToVector3());
            List<BaseBox> boxesForInstance = _listOfPossibleBox.GetRange(firstIndexOfInstance, lastIndexOfInstance - firstIndexOfInstance + 1);
            Shuffle(boxesForInstance);

            foreach (var boxForInstance in boxesForInstance)
            {
                for (int j = 0; j < _emptyArrayPositions.Count; j++)
                {
                    countOfOperations++;
                    bool canPut = await CanPutBoxInField(boxForInstance, _emptyArrayPositions[j].Value);
                    if (canPut)
                    {
                        PutBoxInField(boxForInstance);
                        RemoveFilledCell();
                        boxForInstance.transform.position = defaultPosition;
                        return;
                    }

                    boxForInstance.transform.position = defaultPosition;
                }
            }
        }

        Debug.LogError($"countOfOperations = {countOfOperations}");
    }

    private void RemoveFilledCell()
    {
        for (int i = _emptyArrayPositions.Count - 1; i >= 0; i--)
        {
            foreach (var box in _level)
            {
                switch (box.Data.Type)
                {
                    case BaseBox.BlockType.None:
                    case BaseBox.BlockType.TapFlowBox:
                    case BaseBox.BlockType.RotateRoadBox:
                    case BaseBox.BlockType.SwipedBox:
                        if (box.IsBoxInPosition(_emptyArrayPositions[i].Value))
                        {
                            _emptyArrayPositions.Remove(_emptyArrayPositions[i]);
                        }

                        continue;
                    case BaseBox.BlockType.BigBoxTapFlowBox:
                        var bigBox = (box as BigBoxTapFlowBox);
                        if (bigBox != null)
                        {
                            if (bigBox.IsBoxInPosition(_emptyArrayPositions[i].Value))
                            {
                                _emptyArrayPositions.Remove(_emptyArrayPositions[i]);
                            }
                        }

                        continue;
                }
            }
        }
    }


    private async UniTask<bool> CanPutBoxInField(BaseBox box, Vector3 position)
    {
        box.transform.position = position * _size;
        box.Data.ArrayPosition = position;
        var x = box as BigBoxTapFlowBox;
        if (x != null)
        {
            x.UpdatePositions();
        }

        if (!IsBoxInLevelShablon(box))
            return false;

        if (IsBoxInPosition(position, box))
            return false;

        bool isValid = await IsValidationPassed(box);
        if (!isValid)
            return false;

        return true;
    }

    private async UniTask<bool> IsValidationPassed(BaseBox box)
    {
        var level = new List<BaseBox>();
        level.AddRange(_level);
        level.Add(box);
        var x = await ValidatorController.Validate(level);

        return x.Count == 0;
    }


    private bool IsBoxInLevelShablon(BaseBox box)
    {
        switch (box.Data.Type)
        {
            case BaseBox.BlockType.None:
            case BaseBox.BlockType.TapFlowBox:
            case BaseBox.BlockType.RotateRoadBox:
            case BaseBox.BlockType.SwipedBox:
                var result = _emptyArrayPositions.FirstOrDefault(x => x.Value == box.Data.ArrayPosition.ToVector3());
                return result != null;

            case BaseBox.BlockType.BigBoxTapFlowBox:

                var bigBox = (box as BigBoxTapFlowBox);
                if (bigBox != null)
                {
                    var positionsOfBox = bigBox.GetBoxPositionsAsVectors();
                    foreach (var variable in positionsOfBox)
                    {
                        var asd = _emptyArrayPositions.FirstOrDefault(x => x.Value == variable);
                        if (asd == null)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                break;
        }

        return false;
    }

    private void PutBoxInField(BaseBox box)
    {
        BaseBox newBox = Instantiate(box, box.transform.position, quaternion.identity, _rootGameField);
        newBox.transform.rotation = box.transform.rotation;
        _level.Add(newBox);
    }

    private int GetLastIndexWithSize(List<BaseBox> listOfPossibleBox, Vector3 size)
    {
        int index = 0;
        for (int i = 0; i < listOfPossibleBox.Count; i++)
        {
            if (listOfPossibleBox[i].Data.Size.ToVector3() == size)
            {
                index = i;
            }
        }

        return index;
    }

    private int GetFirstIndexWithSize(List<BaseBox> listOfPossibleBox, Vector3 size)
    {
        for (int i = 0; i < listOfPossibleBox.Count; i++)
        {
            if (listOfPossibleBox[i].Data.Size.ToVector3() == size)
            {
                return i;
            }
        }

        return 0;
    }

    private List<BaseBox> GetPossibleBoxes()
    {
        List<BaseBox> result = new List<BaseBox>();

        foreach (var possibleBox in _possibleBoxesProbability)
        {
            result.AddRange(GetAllVariantsOfBlock(possibleBox.Box));
        }

        return result;
    }

    private List<BaseBox> GetAllVariantsOfBlock(BaseBox box)
    {
        List<BaseBox> result = new List<BaseBox>();

        for (int i = 0; i < BoxRotator.GetSize(); i++)
        {
            var obj = Instantiate(box, Vector3.one * 1000, quaternion.identity, _rootPool);
            BoxRotator.Rotate(i, obj);
            result.Add(obj);
        }

        return result;
    }

    private void BeckBoxVariantsToDefaultPosition()
    {
        foreach (var VARIABLE in _listOfPossibleBox)
        {
            VARIABLE.transform.position = Vector3.one * 1000;
        }
    }

    private int GetIndexForNextBox()
    {
        int max = 0;
        foreach (var VARIABLE in _possibleBoxesProbability)
        {
            max += VARIABLE.Chanse;
        }

        int random = Random.Range(0, max);

        int summ = 0;
        for (int i = 0; i < _possibleBoxesProbability.Count; i++)
        {
            summ += _possibleBoxesProbability[0].Chanse;
            if (summ >= random)
            {
                return i;
            }
        }

        return _possibleBoxesProbability.Count - 1;
    }


    [Serializable]
    private class BoxProbability
    {
        public BaseBox Box;
        public int Chanse;
    }


#if UNITY_EDITOR
    [ContextMenu("Tools/CreateFrom3D")]
    public void CreateFrom3d()
    {
        _colliders ??= new List<Collider>();
        _colliders.Clear();
        foreach (Transform variable in _objectsWithCollidersForCreate)
        {
            var item = variable.GetComponent<Collider>();
            if (item != null)
            {
                _colliders.Add(item);
            }
        }

        foreach (var variable in _colliders)
        {
            SetMaxLevelSize(variable.bounds.min);
            SetMinLevelSize(variable.bounds.min);

            SetMaxLevelSize(variable.bounds.max);
            SetMinLevelSize(variable.bounds.max);
        }

        for (int x = (int)(_minLevelSize.x - 1); x < _maxLevelSize.x + 1; x++)
        {
            for (int y = (int)(_minLevelSize.y - 1); y < _maxLevelSize.y + 1; y++)
            {
                for (int z = (int)(_minLevelSize.z - 1); z < _maxLevelSize.z + 1; z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    var closestPoint = GetColliderClosestPoint(pos);
                    Debug.DrawLine(closestPoint, pos, Color.blue, 20);

                    if (Vector3.Distance(closestPoint, pos) <= _minDistance)
                    {
                        GameObject g = Instantiate(_baseBoxPlace, pos, quaternion.identity, transform);
                        _arrayPositions.Add(new Vector3Class() { Value = g.transform.position });
                        g.transform.position *= 1.03f;
                        _gameObjects.Add(g);
                    }
                }
            }
        }

        foreach (Transform variable in _objectsWithCollidersForCreate)
        {
            variable.gameObject.SetActive(false);
        }
    }

    public bool IsBoxInPosition(Vector3 arrayPosition, BaseBox box)
    {
        foreach (var variable in _level)
        {
            if (variable == box)
            {
                continue;
            }

            switch (variable.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    if (variable.IsBoxInPosition(arrayPosition))
                    {
                        return true;
                    }

                    continue;
                case BaseBox.BlockType.BigBoxTapFlowBox:
                    var bigBox = (variable as BigBoxTapFlowBox);
                    if (bigBox != null)
                    {
                        if (bigBox.IsBoxInPosition(arrayPosition))
                            return true;
                    }

                    continue;
            }
        }

        return false;
    }

    [ContextMenu("Tools/RemoveAll")]
    public void RemoveAll()
    {
        foreach (var VARIABLE in _gameObjects)
        {
            Destroy(VARIABLE, 2f);
        }

        _gameObjects.Clear();
        _arrayPositions.Clear();
    }


    private Vector3 GetColliderClosestPoint(Vector3 point)
    {
        float minDistance = float.MaxValue;
        Vector3 closestPoint = Vector3.zero;
        foreach (var collider1 in _colliders)
        {
            var x = collider1.ClosestPoint(point);
            float distance = Vector3.Distance(point, x);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = x;
            }
        }

        return closestPoint;
    }

#endif

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


    public static void Shuffle(IList<BaseBox> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static void Shuffle(List<Vector3Class> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}