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
        [SerializeField] private float _distanse;
        [SerializeField] private float _distanseToDieAction;
        [SerializeField] private BaseBox _box;

        private List<TapObject> _tapObjects;
        private int _layerMask;
        private int _layerMaskTapObject;
        private IDieAction _dieAction;

        private const string GameFieldElement = "GameFieldElement";
        private const string TapObject = "TapObject";
        private const string TapObjectCancelation = "TapObjectSwipedBoxCansel";

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
                Managers.Instance.GameField.RemoveBox(_box);
                await MoveOut(box1 == null ? _parent.forward : -_parent.forward);
                Destroy(gameObject, 0.2f);
                return;
            }

            Managers.Instance.GameField.SetActiveGlobalInput(false);
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
            else
            {
                CreateCancelationObject(_parent.position, _parent.forward);
            }
        }

        private async void HandleFingerTap(LeanFinger finger)
        {
            var hit = Managers.Instance.InputController.RaycastBox(finger.ScreenPosition, _layerMaskTapObject);

            if (hit.collider == null)
                return;

            var box = hit.collider.GetComponent<TapObject>();
            if (box == null)
                return;

            ClearTapObject();

            if (!box.name.Contains(TapObjectCancelation))
            {
                _box.Data.ArrayPosition = box.GetArrayPosition();
                await MoveTo(Managers.Instance.GameField.GetWorldPosition(_box.Data.ArrayPosition));
            }
           
            BackToDefaultState();
        }

        private void BackToDefaultState()
        {
            IsReactionOnProcess = false;
            LeanTouch.OnFingerTap -= HandleFingerTap;
            Managers.Instance.GameField.SetActiveGlobalInput(true);
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(_parent.position, _parent.forward * 5, Color.red);
            Debug.DrawRay(_parent.position, -_parent.forward * 5, Color.red);
        }

        private async UniTask CreatePlacesForMove(Vector3 targetArrayPosition, Vector3 direction)
        {
            var positions = Managers.Instance.GameField.EmptyPositionBetweenTwoBoxes(_box.Data.ArrayPosition, targetArrayPosition);
            foreach (var arrayPosition in positions)
            {
                var tapObject = await Managers.Instance.GameField.CreateTapObject(TapObject);
                tapObject.Setup(arrayPosition, arrayPosition + direction);
                _tapObjects.Add(tapObject);
            }
        }
        
        private async UniTask CreateCancelationObject(Vector3 targetArrayPosition, Vector3 direction)
        {
            var tapObject = await Managers.Instance.GameField.CreateTapObject(TapObjectCancelation);
            tapObject.Setup(targetArrayPosition, targetArrayPosition + direction);
            _tapObjects.Add(tapObject);
        }

        private void ClearTapObject()
        {
            foreach (var variable in _tapObjects)
            {
                GameObject o = variable.gameObject;
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
            
            return Managers.Instance.GameField.ExistBox(box.Data.ArrayPosition) ? box : null;
        }
        
        private async UniTask MoveOut(Vector3 direction)
        {
            Managers.Instance.GameField.CheckForWin();

            bool isPlayDie = false;
            Vector3 startPos = _parent.position;
            while (Vector3.Distance(_parent.position, startPos) < _distanse)
            {
                if (!isPlayDie && Vector3.Distance(_parent.position, startPos) > _distanseToDieAction)
                {
                    isPlayDie = true;
                    GetComponent<IDieAction>()?.DieAction();
                }
                _parent.Translate(direction * (Time.deltaTime * _flyEwaySpeed), Space.World);
                await UniTask.Yield();
            }
            Destroy(_parent.gameObject);
        }

        private async UniTask MoveTo(Vector3 targetPosition)
        {
            var direction = (targetPosition - _parent.position).normalized;
            while (Vector3.Distance(_parent.position, targetPosition) > 0.15f)
            {
                _parent.Translate(direction * (Time.deltaTime * _moveSpeed), Space.World);
                await UniTask.Yield();
            }

            _parent.position = targetPosition;
        }
    }
}