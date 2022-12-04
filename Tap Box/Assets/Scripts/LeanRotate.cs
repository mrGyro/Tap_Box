using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

namespace DefaultNamespace
{
    public class LeanRotate : MonoBehaviour
    {
        [Tooltip("The camera we will be moving (None = MainCamera)")]
        public Camera Camera;

        [SerializeField] Transform _centr;
        [SerializeField] private float distanceToTarget = 10;

        private Vector3 previousPosition = Vector3.positiveInfinity;

        public virtual void Rotate(List<LeanFinger> fingers)
        {
            if (fingers[0].Age == 0)
            {
                previousPosition = Camera.ScreenToViewportPoint(fingers[0].LastScreenPosition);
                distanceToTarget = Vector3.Distance(Camera.transform.position, _centr.position);
            }
            else 
            {
                Vector3 newPosition = Camera.ScreenToViewportPoint(fingers[0].ScreenPosition);
                Vector3 direction = previousPosition - newPosition;
                
                float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
                float rotationAroundXAxis = direction.y * 180; // camera moves vertically
                
                Camera.transform.position = _centr.position;
                
                Camera.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!
                Camera.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
                
                Camera.transform.Translate(new Vector3(0, 0, -distanceToTarget));
                //Camera.transform.LookAt(Vector3.zero);
                previousPosition = newPosition;
            }
        }
    }
}