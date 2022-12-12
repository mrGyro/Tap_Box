using System;
using System.Collections.Generic;
using Boxes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace LevelCreator
{
    public class LevelCreator : MonoBehaviour
    {
        [SerializeField] Camera camera;
        [SerializeField] float size;
        [SerializeField] Transform root;
        [SerializeField] Transform shadowBox;
        [SerializeField] private Image currentBoxIcon;
        [SerializeField] List<BaseBox> prefabs;
        [SerializeField] List<AssetReference> loadableSprites;

        [SerializeField] List<BaseBox> level;

        private BaseBox _currentSelected;
        private int _layerMask;
        private const string GameFieldElement = "GameFieldElement";

        private Vector2 newPos;
        private void Start()
        {
            Upd();
            SelectBlock(0);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                LeftMouseButton(Input.mousePosition);
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                RemoveBlock(Input.mousePosition);
            }
        }

        void OnEnable()
        {
            _layerMask = LayerMask.GetMask(GameFieldElement);
        }

        private void RemoveBlock(Vector2 mousePosition)
        {
            var box = RaycastBox(mousePosition);

            if (box == null)
                return;

            level.Remove(box);
            Destroy(box.gameObject);
        }

        private async UniTask Upd()
        {
            while (true)
            {
                GetNewBoxPosition(newPos);
                await UniTask.Delay(50);
            }
        }

        private async void SelectBlock(int newIndex)
        {
            Debug.LogError(loadableSprites[0].RuntimeKey);
            Debug.LogError(loadableSprites[0].SubObjectName);
            Debug.LogError(loadableSprites[0]);
            Debug.LogError(loadableSprites[0].Asset);
            //var x = loadableSprites.Find(x=>x.a)
            //var x = await Addressables.LoadAssetAsync<Sprite>(prefabs[0].Data.Type + "_icon");
            
           // currentBoxIcon.sprite = x;
        }

        private void LeftMouseButton(Vector2 pos)
        {
            newPos = pos;

            if (level.Count == 0)
            {
                _currentSelected = prefabs[0];
                _currentSelected.Data.ArrayPosition = Vector3.zero;
                Create();
                return;
            }

            var box = RaycastBox(pos);

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

        private async void Create()
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