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
        private float _size = 1.03f;

        public override async UniTask BoxReactionStart()
        {
            await _reaction.ReactionStart();
            await _reaction.ReactionEnd();
        }

        public async override UniTask Init()
        {
            await UniTask.Yield();
            foreach (var VARIABLE in boxePositions)
            {
                VARIABLE.UpdateArrayPosition();
            }
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