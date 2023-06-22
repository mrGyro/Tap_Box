using Boxes.Reactions;
using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Boxes.BigBoxTapFlowBox
{
    public class BigBoxTapFlowBox : BaseBox
    {
        [SerializeField] private BaseReaction _reaction;
        [SerializeField] private BaseBox boxe;
        [SerializeField] private BigBoxPart[] boxePositions;
        [SerializeField] private BigBoxPart[] boxeDirectionPositions;
        [SerializeField] private Transform renderer;


        public override async UniTask BoxReactionStart()
        {
            await _reaction.ReactionStart();
            await _reaction.ReactionEnd();
        }

        public override async UniTask Init()
        {
            foreach (var variable in boxePositions)
            {
                variable.UpdateArrayPosition();
            }
        }

        [ContextMenu("Calculate Positions")]
        private void CalculatePositions()
        {
            Init();
        } 
        
        [ContextMenu("Reset positions")]
        private void ResetPositionToLocalDefaultPlaces()
        {
            foreach (var VARIABLE in boxePositions)
            {
                VARIABLE.ResetPositionToLocalDefaultPlaces();
            }
        }
        
        [ContextMenu("Set By Center Position")]
        private void SetByCenterPosition()
        {
            Vector3 result = Vector3.zero;

            foreach (var VARIABLE in boxePositions)
            {
                result += VARIABLE.ArrayDirection * GameField.Size;
            }

            result /= boxePositions.Length;
            renderer.localPosition = result;

        }

        public override bool IsBoxInPosition(Vector3 position)
        {
            foreach (var VARIABLE in boxePositions)
            {
                if (position == VARIABLE.ArrayPosition)
                {
                    return true;
                }
            }

            return false;
        }

        public BigBoxPart[] GetBoxPositions()
        {
            return boxePositions;
        }
        
        public Vector3[] GetBoxPositionsAsVectors()
        {
            Vector3[] array = new Vector3[boxePositions.Length];

            for (int i = 0; i < boxePositions.Length; i++)
            {
                array[i] = boxePositions[i].ArrayPosition;
            }
            return array;
        }

        public Vector3 GetNearestPosition(Vector3 checkPosition)
        {
            BigBoxPart part = null;
            float distance = float.MaxValue;

            foreach (var VARIABLE in boxePositions)
            {
                float currentDistance = Vector3.Distance(VARIABLE.transform.position, checkPosition);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    part = VARIABLE;
                }
            }

            //Debug.LogError(part.ArrayPosition);
            return part.ArrayPosition;
        }

        public override void Rotate(Vector3 direction, float angle)
        {
            base.Rotate(direction, angle);
            UpdatePositions();
        }
        
        public Vector3 GetDirection()
        {
            return _reaction.transform.forward;
        }
        
        public override void Rotate(Vector3 angle)
        {
            base.Rotate(angle);
            UpdatePositions();
        }

        public BigBoxPart[] GetDirectionParts()
        {
            return boxeDirectionPositions;
        }

        public void UpdatePositions()
        {
            foreach (var bigBoxPart in boxePositions)
            {
                bigBoxPart.UpdateArrayPosition();
            }

            boxe.Data.ArrayPosition = boxePositions[0].ArrayPosition;
        }
    }
}