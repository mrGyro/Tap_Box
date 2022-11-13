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

        // public virtual void Rotate(List<LeanFinger> fingers)
        // {
        //     var degrees = LeanGesture.GetTwistDegrees(fingers);
        //     //Debug.LogError($"{fingers[0].Age} + {fingers[0].Pressure} + {fingers[0].LastPressure} + {fingers[0].ScreenDelta} + {fingers[0].LastSnapshotScreenDelta}");
        //     //float x = Vector2.Distance(fingers[0].ScreenPosition, fingers[0].LastScreenPosition);
        //     float x = Vector2.Angle(fingers[0].ScreenPosition, fingers[0].LastScreenPosition);
        //     
        //     Camera.transform.RotateAround(_centr.position, fingers[0].ScreenDelta, x);
        // }


        [SerializeField] private float distanceToTarget = 10;

        private Vector3 previousPosition = Vector3.positiveInfinity;

        public virtual void Rotate(List<LeanFinger> fingers)
        {
            if (fingers[0].Age == 0)
            {
                //if(previousPosition == Vector3.positiveInfinity)
                previousPosition = Camera.ScreenToViewportPoint(fingers[0].LastScreenPosition);
                distanceToTarget = Vector3.Distance(Camera.transform.position, _centr.position);
                // previousPosition = Camera.ScreenToViewportPoint(Input.mousePosition);
            }
            else 
            {
                Vector3 newPosition = Camera.ScreenToViewportPoint(fingers[0].ScreenPosition);
                Vector3 direction = previousPosition - newPosition;

                float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
                float rotationAroundXAxis = direction.y * 180; // camera moves vertically

                Camera.transform.position = _centr.position;

                Camera.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
                Camera.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

                Camera.transform.Translate(new Vector3(0, 0, -distanceToTarget));

                previousPosition = newPosition;
            }
        }
    }
}