using Managers;
using UnityEngine;

namespace Boxes.SwipableBox
{
    public class TapObject : MonoBehaviour
    {
        private Vector3 _arrayPosition;
        public void Setup(Vector3 arrayPosition, Vector3 direction)
        {
            transform.position = GameManager.Instance.GameField.GetWorldPosition(arrayPosition);
            transform.LookAt(direction);
            _arrayPosition = arrayPosition;
        }

        public Vector3 GetArrayPosition()
        {
            return _arrayPosition;
        }
    }
}