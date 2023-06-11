using System;
using System.Collections.Generic;
using System.Linq;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LevelCreator.Validator
{
    public static class ValidatorController
    {
        private static Vector3 _maxLevelSize;
        private static Vector3 _minLevelSize;
        private static LayerMask _mask = LayerMask.GetMask("GameFieldElement");
        private static List<BaseBox> level;
        private static float _distanceToCheck = 1000f;
        private static int _zero = 0;

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
        
        public static async UniTask<bool> IsValidateLastAddedBlock(List<BaseBox> levelInput, BaseBox newBox)
        {
            // BaseBox[] copy = new BaseBox[levelInput.Count];
            // levelInput.CopyTo(copy);
            // SetNewMaxMinSize(copy);
            //
            // level = copy.ToList();
            BaseBox[] levelNew = levelInput.ToArray();
            SetNewMaxMinSize(levelNew);

            int layer = levelNew[0].gameObject.layer;

            bool isBlockRemoved = true;
            int defaultLayer = 0;
            int length = levelNew.Length;
            while (isBlockRemoved)
            {
                isBlockRemoved = false;
                await UniTask.Yield();
                for (var index = 0; index < length; index++)
                {
                    var variable = levelNew[index];
                    if (variable.gameObject.layer != layer)
                    {
                        continue;
                    }

                    variable.gameObject.layer = defaultLayer;

                    if (!IsBoxesInDirection(variable))
                    {
                        variable.gameObject.layer = defaultLayer;
                        isBlockRemoved = true;
                        if (variable == newBox)
                        {
                            for (var i = 0; i < levelNew.Length; i++)
                            {
                                levelNew[i].gameObject.layer = layer;
                            }

                            return true;
                        }

                        break;
                    }

                    variable.gameObject.layer = layer;
                }
            }

            var result = levelNew.FirstOrDefault(x => x.gameObject.layer == layer);
            for (var index = 0; index < levelNew.Length; index++)
            {
                levelNew[index].gameObject.layer = layer;
            }

            return result == null;
        }
        public static async UniTask<List<BaseBox>> Validate(List<BaseBox> levelInput)
        {
            BaseBox[] copy = new BaseBox[levelInput.Count];
            levelInput.CopyTo(copy);
            SetNewMaxMinSize(copy);

            level = copy.ToList();
            int length = level.Count;

            bool isBlockRemoved = true;
            int layer = level[0].gameObject.layer;
            int defaultLayer = 0;
            var startTime = DateTime.Now;
            while (isBlockRemoved)
            {
                isBlockRemoved = false;
                await UniTask.Yield();
                for (var index = 0; index < length; index++)
                {
                    var variable = level[index];
                    if (variable.gameObject.layer != layer)
                    {
                        continue;
                    }

                    variable.gameObject.layer = defaultLayer;

                    if (!IsBoxesInDirection(variable))
                    {
                        variable.gameObject.layer = defaultLayer;
                        isBlockRemoved = true;
                        break;
                    }

                    variable.gameObject.layer = layer;
                }
            }

            var result = level.Where(x => x.gameObject.layer == layer).ToList();
            foreach (var VARIABLE in level)
            {
                VARIABLE.gameObject.layer = layer;
            }

            DateTime date = DateTime.Now;
            double hours = (date - startTime).TotalSeconds;
            Debug.LogError(hours);
            return result;
        }

        private static RaycastHit[] results;
        private static bool IsBoxesInDirection(BaseBox box)
        {
            results = new RaycastHit[5];
            if (box.Data.Type == BaseBox.BlockType.BigBoxTapFlowBox)
            {
                var bigBox = box as BigBoxTapFlowBox;
                var array = bigBox.GetDirectionParts();
                var direction = bigBox.GetDirection();

                foreach (var bigBoxPart in array)
                {
                    var size = Physics.RaycastNonAlloc(bigBoxPart.transform.position, direction, results, _distanceToCheck, _mask);

                    if (size != _zero)
                    {
                        return true;
                    }
                }
            }
            else
            {
                var size = Physics.RaycastNonAlloc(box.transform.position, box.transform.forward, results, _distanceToCheck, _mask);
                return size != _zero;
            }

            return false;
        }

        private static bool IsHitBox(BaseBox box, RaycastHit[] hits)
        {
            foreach (var hit in hits)
            {
                if (hit.transform == box.transform)
                    continue;

                if (level.FirstOrDefault(x => x.transform == hit.transform))
                {
                    return true;
                }
            }

            return false;
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
        
        private static void SetNewMaxMinSize(List<BaseBox> boxes)
        {
            _maxLevelSize = boxes.Count == 0 ? Vector3.zero : boxes[0].Data.ArrayPosition;
            _minLevelSize = boxes.Count == 0 ? Vector3.zero : boxes[0].Data.ArrayPosition;
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