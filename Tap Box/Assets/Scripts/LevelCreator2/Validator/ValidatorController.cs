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
                    if (!IsBoxesInDirection(level[i], level))
                    {
                        level.Remove(level[i]);
                        isBlockRemoved = true;
                        break;
                    }
                }
            }

            return level;
        }

        private static bool IsBoxesInDirection(BaseBox box, List<BaseBox> level)
        {
            var bigBox = box as BigBoxTapFlowBox;
            if (bigBox != null)
            {
                var reaction = bigBox.GetComponent<BigBoxFlowAwayReaction>();
                var array = bigBox.GetBoxPositions();
                var direction = reaction.GetDirection();
                Debug.DrawLine(reaction.transform.position, reaction.transform.position + direction * 10, Color.magenta, 20);
                foreach (var VARIABLE in array)
                {
                    RaycastHit[] hits;
                    hits = Physics.RaycastAll(VARIABLE.transform.position, direction, 1000F);

                    if (IsHitBox(box, hits, level))
                    {
                        return true;
                    }
                }
            }
            else
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(box.transform.position, box.transform.forward, 1000F);
                Debug.DrawLine(box.transform.position, box.transform.position + box.transform.forward * 10, Color.green, 20);
                return IsHitBox(box, hits, level);
            }

            return false;
        }

        private static bool IsHitBox(BaseBox box, RaycastHit[] hits, List<BaseBox> level)
        {
            foreach (var hit in hits)
            {
                if (hit.transform == box.transform || hit.transform.GetComponent<BaseBox>() == null)
                    continue;

                if (level.FirstOrDefault(x => x.transform == hit.transform))
                {
                    return true;
                }
            }

            return false;
        }

        private async static UniTask<BaseBox> GetNearestBoxInDirection(BaseBox[] level, Vector3[] boxArrayPosition, Vector3 direction, BaseBox currentBox)
        {
            Vector3[] arrayPosition = new Vector3[boxArrayPosition.Length];
            for (int i = 0; i < boxArrayPosition.Length; i++)
            {
                arrayPosition[i] = boxArrayPosition[i];
            }

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

                    foreach (var variable in level)
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