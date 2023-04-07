using System;
using System.Collections.Generic;
using System.Linq;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator
{
    public class LevelCreator : MonoBehaviour
    {
        public static Action OnLevelChanged;
        [SerializeField] Camera camera;
        [SerializeField] float size;
        [SerializeField] Transform root;
        [SerializeField] Transform shadowBox;
        [SerializeField] private Image currentBoxIcon;
        [SerializeField] private BoxRotator boxRotator;
        [SerializeField] private BoxMover boxMover;

        [SerializeField] List<BaseBox> prefabs;
        [SerializeField] List<Sprite> loadableSprites;
        public List<BaseBox> Level;

        private BaseBox _currentSelectedBoxForInstantiate;
        private BaseBox _currentTargetBox;
        private int _layerMask;
        private const string GameFieldElement = "GameFieldElement";

        private int _currentIndex = 0;
        private bool _isActive = true;
        private List<BaseBox> _collisions = new();


        private void Start()
        {
            Upd();
            _currentSelectedBoxForInstantiate = prefabs[_currentIndex];
            SelectBlockForInstantiate();
            shadowBox.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _currentTargetBox = null;
            OnLevelChanged += OnLevelChangedValidation;
            ShowRedColor();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _isActive = !_isActive;
                Cursor.lockState = _isActive ? CursorLockMode.Locked : CursorLockMode.Confined;
                Cursor.visible = !_isActive;
            }

            if (!_isActive)
            {
                return;
            }

            BoxSelecting();
            BoxesVisibility();

            if (_currentTargetBox != null)
                return;


            MouseWheelSelectBox();
            CreateBox();
            RemoveBox();
        }

        private void BoxSelecting()
        {
            if (!Input.GetKeyDown(KeyCode.E))
                return;

            if (_currentTargetBox == null)
            {
                shadowBox.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                SelectBlock(Input.mousePosition);
                return;
            }

            shadowBox.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _currentTargetBox = null;
            boxRotator.SetBox(_currentTargetBox);
            boxMover.SetBox(_currentTargetBox);
        }

        private void BoxesVisibility()
        {
            if (Input.GetKeyDown(KeyCode.F))
                HideBox(Input.mousePosition);

            if (Input.GetKeyDown(KeyCode.G))
                ShowAll();
        }

        private void RemoveBox()
        {
            if (Input.GetMouseButtonDown(1))
            {
                RemoveBlock(Input.mousePosition);
            }
        }

        private void MouseWheelSelectBox()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                _currentIndex++;
                if (_currentIndex >= prefabs.Count)
                    _currentIndex = 0;
                _currentSelectedBoxForInstantiate = prefabs[_currentIndex];

                SelectBlockForInstantiate();
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                _currentIndex--;
                if (_currentIndex < 0)
                    _currentIndex = prefabs.Count - 1;
                _currentSelectedBoxForInstantiate = prefabs[_currentIndex];

                SelectBlockForInstantiate();
            }
        }

        private void HideBox(Vector3 mousePosition)
        {
            var box = RaycastBox(mousePosition);

            if (box == null)
            {
                return;
            }

            box.gameObject.SetActive(false);
        }

        private void ShowAll()
        {
            foreach (var variable in Level)
            {
                variable.gameObject.SetActive(true);
            }
        }


        private void SelectBlock(Vector2 mousePosition)
        {
            var box = RaycastBox(mousePosition);

            if (box == null)
            {
                _currentTargetBox = null;
                boxRotator.SetBox(_currentTargetBox);
                boxMover.SetBox(_currentTargetBox);
                return;
            }

            _currentTargetBox = box;
            boxRotator.SetBox(_currentTargetBox);
            boxMover.SetBox(_currentTargetBox);
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

            Level.Remove(box);
            Destroy(box.gameObject);
        }

        private async UniTask Upd()
        {
            while (true)
            {
                if (_currentTargetBox == null)
                    GetNewBoxPosition(Input.mousePosition);

                await UniTask.Delay(100);
            }
        }

        private void SelectBlockForInstantiate()
        {
            //var sprite = loadableSprites[_currentIndex];
            currentBoxIcon.sprite = loadableSprites[_currentIndex];
        }

        private void CreateBox()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            if (Level.Count == 0)
            {
                _currentSelectedBoxForInstantiate = prefabs[_currentIndex];
                _currentSelectedBoxForInstantiate.Data.ArrayPosition = Vector3.zero;
                Create();
                return;
            }

            var box = RaycastBox(Input.mousePosition);

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

            Vector3 arrayPosition = Vector3.zero;
            var box = hit.transform.GetComponent<BaseBox>();
            switch (box.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    arrayPosition = box.Data.ArrayPosition.ToVector3();

                    break;
                case BaseBox.BlockType.BigBoxTapFlowBox:
                    var bigBox = hit.transform.GetComponent<BigBoxTapFlowBox>();
                    arrayPosition = bigBox.GetNearestPosition(hit.point);
                    break;
            }

            shadowBox.gameObject.SetActive(true);

            List<Vector3> nearBoxPositions = new List<Vector3>()
            {
                Vector3.up,
                Vector3.down,
                Vector3.left,
                Vector3.right,
                Vector3.forward,
                Vector3.back
            };

            float distance = float.MaxValue;
            Vector3 dir = Vector3.zero;
            foreach (var variable in nearBoxPositions)
            {
                float currentDistance = Vector3.Distance((arrayPosition + variable) * size, hit.point);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    dir = variable;
                }
            }

            Vector3 position = (arrayPosition + dir) * size;

//            Debug.LogError(hit.point + " " + dir + " " + arrayPosition + " " + position + " " + distance);
            shadowBox.position = position;
            _currentSelectedBoxForInstantiate.Data.ArrayPosition = new Vector3(position.x / size, position.y / size, position.z / size);
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
            Level ??= new List<BaseBox>();

            if (Level.FirstOrDefault(x => x.IsBoxInPosition(_currentSelectedBoxForInstantiate.Data.ArrayPosition.ToVector3())) != null)
                return;

            var boxGameObject = await InstantiateAssetAsync(GetAddressableName(_currentSelectedBoxForInstantiate.Data));
            var box = boxGameObject.GetComponent<BaseBox>();

            Vector3 boxPosition = _currentSelectedBoxForInstantiate.Data.ArrayPosition.ToVector3();
            box.transform.position = boxPosition * size;
            box.transform.rotation = Quaternion.Euler(_currentSelectedBoxForInstantiate.Data.Rotation);

            box.Data = new BoxData()
            {
                Type = _currentSelectedBoxForInstantiate.Data.Type,
                ArrayPosition = _currentSelectedBoxForInstantiate.Data.ArrayPosition,
                Rotation = _currentSelectedBoxForInstantiate.Data.Rotation,
                Size = _currentSelectedBoxForInstantiate.Data.Size
            };

            Level.Add(box);

            switch (box.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    await box.Init();
                    break;
                case BaseBox.BlockType.BigBoxTapFlowBox:
                    var bigBox = box.transform.GetComponent<BigBoxTapFlowBox>();
                    await bigBox.Init();
                    break;
            }

            OnLevelChanged?.Invoke();
        }

        private string GetAddressableName(BoxData box)
        {
            switch (box.Type)
            {
                case BaseBox.BlockType.None:

                case BaseBox.BlockType.TapFlowBox:

                case BaseBox.BlockType.RotateRoadBox:

                case BaseBox.BlockType.SwipedBox:
                    return $"Default_{box.Type}";
                case BaseBox.BlockType.BigBoxTapFlowBox:
                    return $"Default_{box.Type}_{box.Size.x}_{box.Size.y}_{box.Size.z}";
            }

            return String.Empty;
        }

        public async void CreateBox(BoxData data)
        {
            Level ??= new List<BaseBox>();

            var boxGameObject = await InstantiateAssetAsync("Default_" + data.Type);
            var box = boxGameObject.GetComponent<BaseBox>();
            box.transform.position = data.ArrayPosition.ToVector3() * size;
            box.transform.rotation = Quaternion.Euler(data.Rotation);
            box.Data = data;
            Level.Add(box);
            OnLevelChanged?.Invoke();
        }

        public void RemoveAllBoxes()
        {
            if (Level == null || Level.Count == 0)
                return;

            foreach (var variable in Level)
            {
                GameObject o = variable.gameObject;
                o.SetActive(false);
                Destroy(o);
            }

            Level = new List<BaseBox>();
        }

        private async UniTask<GameObject> InstantiateAssetAsync(string assetName)
        {
            var x = await AssetProvider.LoadAssetAsync<GameObject>(assetName);
            return x == null ? null : Instantiate(x, root);
        }

        private void OnLevelChangedValidation()
        {
            foreach (var variable in _collisions)
            {
                var meshRenderer = variable.transform.GetChild(0).GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                    continue;

                meshRenderer.materials[0].SetFloat(DissolveSlider, -1);
                meshRenderer.materials[1].SetFloat(DissolveSlider, -1);
            }

            _collisions = new List<BaseBox>();
            foreach (var box in Level)
            {
                foreach (var box2 in Level)
                {
                    if (box == box2)
                        continue;


                    if (!IsBlockCrossPosition(box, box2))
                    {
                        continue;
                    }

                    if (!_collisions.Contains(box))
                        _collisions.Add(box);

                    if (!_collisions.Contains(box2))
                        _collisions.Add(box2);
                }
            }

            if (_collisions.Count > 0)
                Debug.LogError("Has collisions: " + _collisions.Count);
        }

        private bool IsBlockCrossPosition(BaseBox box, BaseBox box2)
        {
            List<Vector3> positions = new List<Vector3>();
            switch (box2.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    positions.Add(box2.Data.ArrayPosition.ToVector3());
                    break;
                case BaseBox.BlockType.BigBoxTapFlowBox:
                    var bigBox = box2.transform.GetComponent<BigBoxTapFlowBox>();
                    foreach (var bigBoxPosition in bigBox.GetBoxPositions())
                    {
                        positions.Add(bigBoxPosition.ArrayPosition);
                    }

                    break;
            }

            foreach (var position in positions)
            {
                switch (box.Data.Type)
                {
                    case BaseBox.BlockType.None:
                    case BaseBox.BlockType.TapFlowBox:
                    case BaseBox.BlockType.RotateRoadBox:
                    case BaseBox.BlockType.SwipedBox:
                        if (box.IsBoxInPosition(position))
                            return true;
                        break;
                    case BaseBox.BlockType.BigBoxTapFlowBox:
                        var bigBox = box.transform.GetComponent<BigBoxTapFlowBox>();
                        if (bigBox.IsBoxInPosition(position))
                            return true;
                        break;
                }
            }

            return false;
        }

        private static readonly int DissolveSlider = Shader.PropertyToID("Dissolve_Slider_Reference");

        private async UniTask ShowRedColor()
        {
            float value = 0;
            bool direction = false;
            while (true)
            {
                await UniTask.WaitForEndOfFrame(this);
                if (_collisions.Count == 0)
                    continue;

                value += direction ? -0.01f : 0.01f;

                if (value < -1 || value > 0)
                    direction = !direction;

                foreach (var variable in _collisions)
                {
                    var meshRenderer = variable.transform.GetChild(0).GetComponent<MeshRenderer>();
                    if (meshRenderer == null)
                        continue;

                    meshRenderer.materials[0].SetFloat(DissolveSlider, value);
                    meshRenderer.materials[1].SetFloat(DissolveSlider, value);
                }
            }
        }
    }
}