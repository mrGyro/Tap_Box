using System;
using System.Collections.Generic;
using Boxes;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] List<BaseBox> prefabs;
        [SerializeField] List<Sprite> loadableSprites;

        [SerializeField] List<BaseBox> level;

        private BaseBox _currentSelectedBoxForInstantiate;
        private BaseBox _currentTargetBox;
        private int _layerMask;
        private const string GameFieldElement = "GameFieldElement";

        private Vector2 newPos;
        private int _currentIndex = 0;

        private void Start()
        {
            Upd();
            _currentSelectedBoxForInstantiate = prefabs[_currentIndex];
            SelectBlockForInstantoate();
            upButton.onClick.AddListener(RotateUp);
            downButton.onClick.AddListener(RotateDown);
            leftButton.onClick.AddListener(RotateLeft);
            rightButton.onClick.AddListener(RotateRight);
        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_currentTargetBox == null)
                {
                    SelectBlock(Input.mousePosition);
                }
                else
                {
                    _currentTargetBox = null;
                }
            }

            if (_currentTargetBox != null)
            {
                shadowBox.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;

                return;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                _currentIndex++;
                if (_currentIndex >= prefabs.Count)
                    _currentIndex = 0;
                _currentSelectedBoxForInstantiate = prefabs[_currentIndex];

                SelectBlockForInstantoate();
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                _currentIndex--;
                if (_currentIndex < 0)
                    _currentIndex = prefabs.Count - 1;
                _currentSelectedBoxForInstantiate = prefabs[_currentIndex];

                SelectBlockForInstantoate();
            }

            if (Input.GetMouseButtonDown(0))
            {
                LeftMouseButton(Input.mousePosition);
            }

            if (Input.GetMouseButtonDown(1))
            {
                RemoveBlock(Input.mousePosition);
            }
        }
        
        private void RotateLeft()
        {
            if (_currentTargetBox == null)
                return;
            
            _currentTargetBox.transform.Rotate(Vector3.up, -90);

        }

        private void RotateRight()
        {
            if (_currentTargetBox == null)
                return;
            _currentTargetBox.transform.Rotate(Vector3.up, 90);

        }

        private void RotateUp()
        {
            if (_currentTargetBox == null)
                return;
            _currentTargetBox.transform.Rotate(Vector3.left, 90);

        }

        private void RotateDown()
        {
            if (_currentTargetBox == null)
                return;
            _currentTargetBox.transform.Rotate(Vector3.left, -90);
        }

        private void SelectBlock(Vector2 mousePosition)
        {
            var box = RaycastBox(mousePosition);

            if (box == null)
            {

                _currentTargetBox = null;
                return;
            }
Debug.LogError("select " + box.name);
            _currentTargetBox = box;
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

        private void SelectBlockForInstantoate()
        {
            var sprite = loadableSprites.Find(x => x.name == _currentSelectedBoxForInstantiate.Data.Type.ToString());
            currentBoxIcon.sprite = sprite;
        }

        private void LeftMouseButton(Vector2 pos)
        {
            newPos = pos;

            if (level.Count == 0)
            {
                _currentSelectedBoxForInstantiate = prefabs[_currentIndex];
                _currentSelectedBoxForInstantiate.Data.ArrayPosition = Vector3.zero;
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
                _currentSelectedBoxForInstantiate.Data.ArrayPosition = new Vector3(pos.x / size, pos.y / size, pos.z / size);
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

            var boxGameObject = await InstantiateAssetAsync(_currentSelectedBoxForInstantiate.Data.Type.ToString());
            var box = boxGameObject.GetComponent<BaseBox>();
            box.transform.position = _currentSelectedBoxForInstantiate.Data.ArrayPosition * size;
            box.transform.rotation = Quaternion.Euler(_currentSelectedBoxForInstantiate.Data.Rotation);
            box.Data = _currentSelectedBoxForInstantiate.Data;
            level.Add(box);
        }

        private async UniTask<GameObject> InstantiateAssetAsync(string assetName)
        {
            var x = await AssetProvider.LoadAssetAsync<GameObject>(assetName);
            return x == null ? null : Instantiate(x, root);
        }
    }
}