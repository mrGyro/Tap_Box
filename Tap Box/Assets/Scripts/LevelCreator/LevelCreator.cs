using System;
using System.Collections.Generic;
using Boxes;
using Cysharp.Threading.Tasks;
using Lean.Touch;
using UnityEngine;

namespace LevelCreator
{
    public class LevelCreator : MonoBehaviour
    {
        [SerializeField] Camera camera;
        [SerializeField] float size;
        [SerializeField] Transform root;
        [SerializeField] Transform shadowBox;
        [SerializeField] List<BaseBox> prefabs;
        [SerializeField] List<BaseBox> level;

        private LeanFinger _finger;
        private BaseBox _currentSelected;
        private int _layerMask;
        private const string GameFieldElement = "GameFieldElement";

        private void Start()
        {
            Upd();
        }

        void OnEnable()
        {
            _layerMask = LayerMask.GetMask(GameFieldElement);
            LeanTouch.OnFingerTap += HandleFingerTap;
        }

        async UniTask Upd()
        {
            while (true)
            {
                if (_finger != null)
                {
                    GetNewBoxPosition(_finger.ScreenPosition);
                }

                await UniTask.Delay(50);
            }
        }

        private void HandleFingerTap(LeanFinger obj)
        {
            _finger = obj;

            if (level.Count == 0)
            {
                _currentSelected = prefabs[0];
                _currentSelected.Data.ArrayPosition = Vector3.zero;
                Create();
                return;
            }

            var box = RaycastBox(obj.ScreenPosition);

            if (box == null)
                return;

            Create();
        }

        private BaseBox RaycastBox(Vector2 screenPosition)
        {
            var hit = RaycastBox(screenPosition, _layerMask);
            if (hit.collider == null)
                return null;

            var box = hit.collider.GetComponent<BaseBox>();

            return box == null ? null : box;
        }

        private void GetNewBoxPosition(Vector2 screenPosition)
        {
            var hit = RaycastBox(screenPosition, _layerMask);
            if (hit.collider == null)
            {
                shadowBox.gameObject.SetActive(false);
                return;
            }

            shadowBox.gameObject.SetActive(true);
            Vector3 pos = hit.transform.position;

            float halfSize = size / 2;
            const float difference = 0.01f;


            if (Math.Abs(hit.point.x - (pos.x + halfSize)) < difference || Math.Abs(hit.point.x - (pos.x - halfSize)) < difference)
                pos.x = hit.point.x >= pos.x ? pos.x + size : pos.x - size;

            if (Math.Abs(hit.point.y - (pos.y + halfSize)) < difference || Math.Abs(hit.point.y - (pos.y - halfSize)) < difference)
                pos.y = hit.point.y >= pos.y ? pos.y + size : pos.y - size;

            if (Math.Abs(hit.point.z - (pos.z + halfSize)) < difference || Math.Abs(hit.point.z - (pos.z - halfSize)) < difference)
                pos.z = hit.point.z >= pos.z ? pos.z + size : pos.z - size;

            if (pos != hit.transform.position)
            {
                shadowBox.position = pos;
                _currentSelected.Data.ArrayPosition = new Vector3(pos.x / size, pos.y / size, pos.z / size);
            }
        }

        private RaycastHit RaycastBox(Vector2 screenPosition, int layerMask)
        {
            var ray = camera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, 1000, layerMask))
            {
                // Debug.DrawRay(camera.transform.position, ray.direction * 1000, Color.yellow, 5);
            }
            else
            {
                Debug.DrawLine(camera.transform.position, hit.point, Color.red, 5);
            }

            return hit;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= HandleFingerTap;
        }

        async void Create()
        {
            level ??= new List<BaseBox>();

            var boxGameObject = await InstantiateAssetAsync(_currentSelected.Data.Type.ToString());
            var box = boxGameObject.GetComponent<BaseBox>();
            box.transform.position = _currentSelected.Data.ArrayPosition * size;
            box.transform.rotation = Quaternion.Euler(_currentSelected.Data.Rotation);
            box.Data = _currentSelected.Data;
            level.Add(box);
        }

        private async UniTask<GameObject> InstantiateAssetAsync(string assetName)
        {
            var x = await AssetProvider.LoadAssetAsync<GameObject>(assetName);
            return x == null ? null : Instantiate(x, root);
        }
    }
}