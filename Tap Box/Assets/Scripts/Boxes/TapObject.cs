using UnityEngine;

namespace Boxes.SwipableBox
{
    public class TapObject : MonoBehaviour
    {
        private Vector3 _arrayPosition;
        public void Setup(Vector3 arrayPosition)
        {
            transform.position = GameField.Instance.GetWorldPosition(arrayPosition);
            _arrayPosition = arrayPosition;
        }

        public Vector3 GetArrayPosition()
        {
            return _arrayPosition;
        }
    }
}