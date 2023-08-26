using System.Collections.Generic;
using System.Linq;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using UnityEngine;

namespace LevelCreator
{
    public class ShadowBox : MonoBehaviour
    {
        private BaseBox _basebox;

        public void Setup(BaseBox basebox)
        {
            if (_basebox != null)
            {
                Destroy(_basebox.gameObject);
            }

            _basebox = Instantiate(basebox, transform);
            _basebox.gameObject.layer = 0;
        }

        public void Hide()
        {
            if (_basebox != null)
            {
                _basebox.gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            if (_basebox != null)
            {
                _basebox.gameObject.SetActive(true);
            }
        }

        public BaseBox GetTargetBox()
        {
            return _basebox;
        }

        public void SetPosition(List<Vector3> positions)
        {
            var bigBox = _basebox as BigBoxTapFlowBox;
            if (bigBox != null)
            {
                _basebox.transform.position = positions[0] * GameField.Size;
                bigBox.UpdatePositions();
            }
            else
            {
                _basebox.transform.position = positions[0];
            }
        }

        public Vector3 GetPositionForCreate()
        {
            switch (_basebox.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    return _basebox.transform.position;

                case BaseBox.BlockType.BigBoxTapFlowBox:
                    var bigBox = _basebox.transform.GetComponent<BigBoxTapFlowBox>();
                    return bigBox.GetBoxPositions()[0].transform.position;
            }

            return _basebox.transform.position;
        }

        private Transform GetCenter(BigBoxTapFlowBox box)
        {
            if (box == null)
            {
                return null;
            }

            var positions = box.GetBoxPositions();
            Vector3 min;
            Vector3 max;
            min.x = positions.Min(x => x.transform.position.x);
            min.y = positions.Min(x => x.transform.position.y);
            min.z = positions.Min(x => x.transform.position.z);

            max.x = positions.Max(x => x.transform.position.x);
            max.y = positions.Max(x => x.transform.position.y);
            max.z = positions.Max(x => x.transform.position.z);

            Vector3 middle = Vector3.Lerp(min, max, 0.5f);
            float minDistance = float.MaxValue;
            Transform result = null;
            foreach (var variable in positions)
            {
                float distance = Vector3.Distance(variable.transform.position, middle);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = variable.transform;
                }
            }

            return result;
        }

        private Vector3 GetCenter(List<Vector3> positions)
        {
            Vector3 min;
            Vector3 max;
            min.x = positions.Min(x => x.x);
            min.y = positions.Min(x => x.y);
            min.z = positions.Min(x => x.z);

            max.x = positions.Max(x => x.x);
            max.y = positions.Max(x => x.y);
            max.z = positions.Max(x => x.z);

            Vector3 middle = Vector3.Lerp(min, max, 0.5f);
            float minDistance = float.MaxValue;
            Vector3 result = Vector3.zero;
            foreach (var variable in positions)
            {
                float distance = Vector3.Distance(variable, middle);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = variable;
                }
            }

            return result;
        }
    }
}