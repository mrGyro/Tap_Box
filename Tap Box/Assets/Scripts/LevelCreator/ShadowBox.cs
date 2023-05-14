using System.Collections.Generic;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using UnityEngine;

namespace LevelCreator
{
    public class ShadowBox : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _boxes;
        private BaseBox _basebox;

        public void Setup(BaseBox basebox)
        {
            _basebox = basebox;
            SetDefaultPosition();
            Hide();

            switch (_basebox.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    _boxes[0].SetActive(true);

                    break;
                case BaseBox.BlockType.BigBoxTapFlowBox:
                    // var bigBox = _basebox.transform.GetComponent<BigBoxTapFlowBox>();
                    //   arrayPosition = bigBox.GetNearestPosition(hit.point);
                    break;
            }
        }

        public void Hide()
        {
            foreach (var VARIABLE in _boxes)
            {
                VARIABLE.SetActive(false);
            }
        }

        public void Show()
        {
            switch (_basebox.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    _boxes[0].SetActive(true);

                    break;
                case BaseBox.BlockType.BigBoxTapFlowBox:

                    var bigBox = _basebox.transform.GetComponent<BigBoxTapFlowBox>();
                    for (int i = 0; i < bigBox.GetBoxPositions().Length; i++)
                    {
                        _boxes[i].SetActive(true);
                    }

                    break;
            }
        }

        public void SetPosition(List<Vector3> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                _boxes[i].transform.position = positions[i] * 1.03f;
            }
        }

        public List<Vector3> GetPositionForCreate()
        {
            switch (_basebox.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    return new List<Vector3>() { _boxes[0].transform.position };

                case BaseBox.BlockType.BigBoxTapFlowBox:
                    var bigBox = _basebox.transform.GetComponent<BigBoxTapFlowBox>();

                    List<Vector3> result = new List<Vector3>();
                    for (int i = 0; i < bigBox.GetBoxPositions().Length; i++)
                    {
                        result.Add(_boxes[i].transform.position);
                    }

                    return result;
                    break;
            }

            return new List<Vector3>() { _boxes[0].transform.position };
        }

        private void SetDefaultPosition()
        {
            foreach (var VARIABLE in _boxes)
            {
                VARIABLE.transform.position = Vector3.zero;
            }
        }
    }
}