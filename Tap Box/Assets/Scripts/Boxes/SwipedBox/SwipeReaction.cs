using System.Collections.Generic;
using Boxes.Reactions;
using Cysharp.Threading.Tasks;
using Lean.Touch;
using UnityEngine;

namespace Boxes.SwipableBox
{
    public class SwipeReaction : BaseReaction
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private float _flyEwaySpeed;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private BaseBox _box;

        private List<TapObject> _tapObjects;
        private int _layerMask;
        private int _layerMaskTapObject;
        private const string GameFieldElement = "GameFieldElement";
        private const string TapObject = "TapObject";

        private void Start()
        {
            _layerMask = LayerMask.GetMask(GameFieldElement);
            _layerMaskTapObject = LayerMask.GetMask(TapObject);
            _tapObjects = new List<TapObject>();
        }

        public override async UniTask ReactionStart()
        {
            if (IsReactionOnProcess)
                return;

            IsReactionOnProcess = true;

            var box1 = GetNearestBox(_parent.forward);
            var box2 = GetNearestBox(-_parent.forward);

            if (box1 == null || box2 == null)
            {
                GameField.Instance.RemoveBox(_box);
                await MoveOut(box1 == null ? _parent.forward : -_parent.forward);
                Destroy(gameObject, 0.2f);
                return;
            }

            GameField.Instance.SetActiveGlobalInput(false);
            LeanTouch.OnFingerTap += HandleFingerTap;

            ClearTapObject();

            if (box1 != null)
            {
                await CreatePlacesForMove(box1.Data.ArrayPosition, -_parent.forward);
                Debug.DrawRay(box1.transform.position, box1.transform.up * 5, Color.red, 5);
            }

            if (box2 != null)
            {
                await CreatePlacesForMove(box2.Data.ArrayPosition, _parent.forward);
                Debug.DrawRay(box2.transform.position, box2.transform.up * 5, Color.red, 5);
            }

            if (_tapObjects.Count == 0)
            {
                BackToDefaultState();
            }
        }

        private async void HandleFingerTap(LeanFinger finger)
        {
            var hit = GameField.Instance.InputController.RaycastBox(finger.ScreenPosition, _layerMaskTapObject);

            if (hit.collider == null)
                return;

            var box = hit.collider.GetComponent<TapObject>();
            if (box == null)
                return;

            ClearTapObject();
            _box.Data.ArrayPosition = box.GetArrayPosition();
            await MoveTo(GameField.Instance.GetWorldPosition(_box.Data.ArrayPosition));
            BackToDefaultState();
        }

        private void BackToDefaultState()
        {
            IsReactionOnProcess = false;
            LeanTouch.OnFingerTap -= HandleFingerTap;
            GameField.Instance.SetActiveGlobalInput(true);
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(_parent.position, _parent.forward * 5, Color.red);
            Debug.DrawRay(_parent.position, -_parent.forward * 5, Color.red);
        }

        private async UniTask CreatePlacesForMove(Vector3 targetArrayPosition, Vector3 direction)
        {
            var positions = GameField.Instance.EmptyPositionBetweenTwoBoxes(_box.Data.ArrayPosition, targetArrayPosition);
            foreach (var VARIABLE in positions)
            {
                var tapObject = await GameField.Instance.CreateTapObject(TapObject);
                tapObject.Setup(VARIABLE, VARIABLE + direction);
                _tapObjects.Add(tapObject);
            }
        }

        private void ClearTapObject()
        {
            foreach (var VARIABLE in _tapObjects)
            {
                GameObject o = VARIABLE.gameObject;
                o.SetActive(false);
                Destroy(o, 0.5f);
            }

            _tapObjects.Clear();
        }

        private BaseBox GetNearestBox(Vector3 direction)
        {
            var ray = new Ray(_parent.position, direction * 1000);
            if (!Physics.Raycast(ray, out var hit, 1000, _layerMask))
            {
                return null;
            }

            var box = hit.transform.GetComponent<BaseBox>();
            if (box == null)
                return null;
            
            return GameField.Instance.ExistBox(box) ? box : null;
        }
        
        private async UniTask MoveOut(Vector3 direction)
        {
            var x = direction * 10;
            var destination = _parent.position + x * 3;
            while (Vector3.Distance(_parent.position, destination) > 1.03f)
            {
                _parent.Translate(direction * (Time.deltaTime * _flyEwaySpeed), Space.World);
                await UniTask.Delay(30);
            }
        }

        private async UniTask MoveTo(Vector3 targetPosition)
        {
            var direction = (targetPosition - _parent.position).normalized;
            while (Vector3.Distance(_parent.position, targetPosition) > 0.15f)
            {
                _parent.Translate(direction * (Time.deltaTime * _moveSpeed), Space.World);
                await UniTask.Delay(30);
            }

            _parent.position = targetPosition;
        }
    }
}