using System.Collections.Generic;
using System.Linq;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using Boxes.TapFlowBox;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LevelCreator.Validator
{
    public static class ValidatorController
    {
        private static Vector3 _maxLevelSize;
        private static Vector3 _minLevelSize;

        public static async void HidePassed(List<BaseBox> levelInput)
        {
            var result = await Validate(levelInput);

            foreach (var VARIABLE in levelInput)
            {
                if (result.Contains(VARIABLE))
                    continue;   
                VARIABLE.gameObject.SetActive(false);
            }
        }

        public static async UniTask<List<BaseBox>> Validate(List<BaseBox> levelInput)
        {
            BaseBox[] copy = new BaseBox[levelInput.Count];
            levelInput.CopyTo(copy);
            SetNewMaxMinSize(copy);

            List<BaseBox> level = copy.ToList();

            bool isBlockRemoved = true;
            while (isBlockRemoved)
            {
                isBlockRemoved = false;

                for (int i = level.Count - 1; i >= 0; i--)
                {
                    await UniTask.Delay(10);

                    var bigBox = level[i] as BigBoxTapFlowBox;
                    if (bigBox != null)
                    {
                        var reaction = bigBox.GetComponent<BigBoxFlowAwayReaction>();
                        Vector3[] array = bigBox.GetBoxPositionsAsVectors();
                        var boxForRemove = await GetNearestBoxInDirection(level.ToArray(), array, reaction.GetDirection(), level[i]);
                        if (boxForRemove == null)
                        {
                            level.Remove(level[i]);
                            isBlockRemoved = true;
                            break;
                        }
                    }
                    else
                    {
                        var reaction = level[i].GetComponent<FlowAwayReaction>();
                        var box = await GetNearestBoxInDirection(level.ToArray(), new[] { level[i].Data.ArrayPosition.ToVector3() }, reaction.GetDirection(), level[i]);

                        if (box == null)
                        {
                            level.Remove(level[i]);
                            isBlockRemoved = true;
                            break;
                        }
                    }
                }
            }

            return level;
        }

        private async static UniTask<BaseBox> GetNearestBoxInDirection(BaseBox[] boxes, Vector3[] boxArrayPosition, Vector3 direction, BaseBox currentBox)
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

                    foreach (var variable in boxes)
                    {
                        bool isBoxInPosition = false;
                        switch (variable.Data.Type)
                        {
                            case BaseBox.BlockType.None:
                            case BaseBox.BlockType.TapFlowBox:
                            case BaseBox.BlockType.RotateRoadBox:
                            case BaseBox.BlockType.SwipedBox:
                                isBoxInPosition = variable.IsBoxInPosition(arrayPosition[i]);
                                break;
                            case BaseBox.BlockType.BigBoxTapFlowBox:
                                var bigBox = (variable as BigBoxTapFlowBox);
                                if (bigBox != null)
                                {
                                    isBoxInPosition = bigBox.IsBoxInPosition(arrayPosition[i]);
                                }

                                break;
                        }

                        if (isBoxInPosition && currentBox != box)
                        {
                            box = variable;
                            break;
                        }
                    }
                }

                if (box != null && currentBox != box)
                {
                    return box;
                }

                for (int i = 0; i < arrayPosition.Length; i++)
                {
                    arrayPosition[i] += direction;
                }
            }

            return null;
        }

        private static void SetNewMaxMinSize(BaseBox[] boxes)
        {
            _maxLevelSize = boxes.Length == 0 ? Vector3.zero : boxes[0].Data.ArrayPosition;
            _minLevelSize = boxes.Length == 0 ? Vector3.zero : boxes[0].Data.ArrayPosition;
            foreach (var baseBox in boxes)
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

        private static void SetMaxLevelSize(Vector3 arrayPosition)
        {
            _maxLevelSize.x = _maxLevelSize.x < arrayPosition.x ? arrayPosition.x : _maxLevelSize.x;
            _maxLevelSize.y = _maxLevelSize.y < arrayPosition.y ? arrayPosition.y : _maxLevelSize.y;
            _maxLevelSize.z = _maxLevelSize.z < arrayPosition.z ? arrayPosition.z : _maxLevelSize.z;
        }

        private static void SetMinLevelSize(Vector3 arrayPosition)
        {
            _minLevelSize.x = _minLevelSize.x > arrayPosition.x ? arrayPosition.x : _minLevelSize.x;
            _minLevelSize.y = _minLevelSize.y > arrayPosition.y ? arrayPosition.y : _minLevelSize.y;
            _minLevelSize.z = _minLevelSize.z > arrayPosition.z ? arrayPosition.z : _minLevelSize.z;
        }

        private static bool CheckMaxLevelSize(Vector3 arrayPosition)
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

        private static bool CheckMinLevelSize(Vector3 arrayPosition)
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

        private static bool RemoveBlock()
        {
            return false;
        }
    }
}