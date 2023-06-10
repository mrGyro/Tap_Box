using System;
using System.Collections.Generic;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using Cysharp.Threading.Tasks;
using LevelCreator;
using LevelCreator.Validator;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator2
{
    public class LevelCreator : MonoBehaviour
    {
        public static Action OnLevelChanged;
        //public static Action<BigBoxTapFlowBox> OnBoxChanged;
        [SerializeField] Camera camera;
        [SerializeField] Button _validate;
        [SerializeField] Button _showAll;
        [SerializeField] float size;
        [SerializeField] Transform root;
        [SerializeField] ShadowBox shadowBox;
        [SerializeField] private Image currentBoxIcon;
        [SerializeField] private BoxMover boxMover;
        [SerializeField] private GameObject _3dObj;

        [SerializeField] List<BaseBox> prefabs;
        [SerializeField] List<Sprite> loadableSprites;
        public List<BaseBox> Level;

        private BaseBox _currentSelectedBoxForInstantiate;
        private BaseBox _currentTargetBox;
        private int _layerMask;
        private const string GameFieldElement = "GameFieldElement";

        private int _currentIndex = 0;
        private float _size = 1.03f;
        private bool _isActive = true;
        private List<BaseBox> _collisions = new();
        private Vector3 _hit;
        private int rotateIndex = 0;

        private readonly List<Vector3> _directions = new()
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        private void Start()
        {
            Upd();
            _currentSelectedBoxForInstantiate = prefabs[_currentIndex];
            SelectBlockForInstantiate();
            shadowBox.Setup(_currentSelectedBoxForInstantiate);
            shadowBox.Hide();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _currentTargetBox = null;
            OnLevelChanged += OnLevelChangedValidation;
            _validate.onClick.AddListener(OnValidateAndHide);
            _showAll.onClick.AddListener(ShowAll);
        }

        public void CreateLevelFromChildren()
        {
            foreach (Transform  VARIABLE in transform)
            {
                var box = VARIABLE.GetComponent<BaseBox>();
                if (box != null)
                {
                    Level.Add(box);
                }
            }
        }

        private void OnValidateAndHide()
        {
            ValidatorController.HidePassed(Level);
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

            if (Input.GetKey(KeyCode.Q))
            {
                MouseWheelRotateBox();
            }
            else
            {
                MouseWheelSelectBox();
            }

            CreateBox();
            RemoveBox();
        }

        private void BoxSelecting()
        {
            if (!Input.GetKeyDown(KeyCode.E))
                return;

            if (_currentTargetBox == null)
            {
                shadowBox.Hide();
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                SelectBlock(Input.mousePosition);
                return;
            }

            shadowBox.Setup(_currentSelectedBoxForInstantiate);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _currentTargetBox = null;
            boxMover.SetBox(_currentTargetBox, this);
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
                shadowBox.Setup(_currentSelectedBoxForInstantiate);
                rotateIndex = 0;
                BoxRotator.Rotate(rotateIndex, shadowBox.GetTargetBox());
                UpdatePosition();
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                _currentIndex--;
                if (_currentIndex < 0)
                    _currentIndex = prefabs.Count - 1;
                _currentSelectedBoxForInstantiate = prefabs[_currentIndex];

                SelectBlockForInstantiate();
                shadowBox.Setup(_currentSelectedBoxForInstantiate);
                rotateIndex = 0;
                BoxRotator.Rotate(rotateIndex, shadowBox.GetTargetBox());
                UpdatePosition();
            }
        }

        private void MouseWheelRotateBox()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                rotateIndex++;
                if (rotateIndex > BoxRotator.GetSize() - 1)
                    rotateIndex = 0;

                BoxRotator.Rotate(rotateIndex, shadowBox.GetTargetBox());

                UpdatePosition();
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                rotateIndex--;
                if (rotateIndex < 0)
                    rotateIndex = BoxRotator.GetSize() - 1;

                BoxRotator.Rotate(rotateIndex, shadowBox.GetTargetBox());
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            (shadowBox.GetTargetBox() as BigBoxTapFlowBox)?.UpdatePositions();
            OnLevelChanged?.Invoke();

            // if (shadowBox.GetTargetBox().Data.Type == BaseBox.BlockType.BigBoxTapFlowBox)
            // {
            //     OnBoxChanged?.Invoke(shadowBox.GetTargetBox() as BigBoxTapFlowBox);
            // }
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
                boxMover.SetBox(_currentTargetBox, this);
                return;
            }

            _currentTargetBox = box;
            boxMover.SetBox(_currentTargetBox, this);
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
                {
                    GetNewBoxPosition(Input.mousePosition);
                }

                await UniTask.Delay(300);
            }
        }

        private void SelectBlockForInstantiate()
        {
            currentBoxIcon.sprite = loadableSprites[_currentIndex];
        }

        private void CreateBox()
        {
            if(_3dObj.activeSelf)
                return;
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
                shadowBox.Hide();
                return;
            }

            _hit = hit.point;
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

            float distance = float.MaxValue;
            Vector3 dir = Vector3.zero;
            foreach (var variable in _directions)
            {
                float currentDistance = Vector3.Distance((arrayPosition + variable) * size, hit.point);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    dir = variable;
                }
            }

            shadowBox.SetPosition(GetPositionsForInstantiate(arrayPosition, dir));
            shadowBox.Show();
        }

        private List<Vector3> GetPositionsForInstantiate(Vector3 position, Vector3 direction)
        {
            List<Vector3> result = new List<Vector3>();

            switch (_currentSelectedBoxForInstantiate.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    result = new List<Vector3>() { (position + direction) * size };
                    break;

                case BaseBox.BlockType.BigBoxTapFlowBox:
                    List<Vector3> bufferResults = new List<Vector3>();
                    float maxDistance = float.MaxValue;

                    var arrayPositions = GetRotationBoxPosition();
                    var possiblePositions = GetPossiblePositions(position, 5);

                    foreach (var possiblePosition in possiblePositions)
                    {
                        bufferResults.Clear();
                        foreach (var bigBoxPart in arrayPositions)
                        {
                            if (IsBoxInPosition(bigBoxPart + possiblePosition))
                                break;

                            bufferResults.Add(bigBoxPart + possiblePosition);
                        }

                        if (bufferResults.Count == arrayPositions.Count)
                        {
                            foreach (var vector3 in bufferResults)
                            {
                                if (Vector3.Distance(vector3 * _size, _hit) < maxDistance)
                                {
                                    result = new List<Vector3>(bufferResults);
                                    maxDistance = Vector3.Distance(vector3 * _size, _hit);
                                }
                            }
                        }

                        bufferResults.Clear();
                    }

                    break;
            }

            return result;
        }

        private List<Vector3> GetRotationBoxPosition()
        {
            // var arrayPositions = _currentSelectedBoxForInstantiate.GetComponent<BigBoxTapFlowBox>().GetBoxPositions();
            var shadowBoxPosition = (shadowBox.GetTargetBox() as BigBoxTapFlowBox).GetBoxPositions();

            Vector3 dif = shadowBoxPosition[0].ArrayPosition;
            List<Vector3> result = new List<Vector3>();

            foreach (var VARIABLE in shadowBoxPosition)
            {
                result.Add(VARIABLE.ArrayPosition - dif);
            }

            return result;
        }

        private List<Vector3> GetPossiblePositions(Vector3 arrayPosition, int maxDistance)
        {
            List<Vector3> positions = new List<Vector3>();

            Vector3 dir = new Vector3();
            for (int i = 0; i < maxDistance; i++)
            {
                for (int j = 0; j < maxDistance; j++)
                {
                    for (int k = 0; k < maxDistance; k++)
                    {
                        dir = new Vector3(arrayPosition.x + i, arrayPosition.y + j, arrayPosition.z + k);
                        positions.Add(dir);
                    }
                }
            }

            for (int i = 0; i < maxDistance; i++)
            {
                for (int j = 0; j < maxDistance; j++)
                {
                    for (int k = 0; k < maxDistance; k++)
                    {
                        dir = new Vector3(arrayPosition.x - i, arrayPosition.y - j, arrayPosition.z - k);
                        positions.Add(dir);
                    }
                }
            }

            return positions;
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
            BaseBox shadowBaseBox = shadowBox.GetTargetBox();
            Vector3 starPos = shadowBox.GetPositionForCreate();
            // if (IsBoxInPosition(shadowBaseBox))
            //     return;

            var boxGameObject = await InstantiateAssetAsync(GetAddressableName(shadowBaseBox.Data));
            var box = boxGameObject.GetComponent<BaseBox>();

            box.transform.position = starPos;
            box.transform.rotation = Quaternion.Euler(shadowBaseBox.Data.Rotation);
            Vector3 Arrayposition = new Vector3(
                starPos.x == 0 ? 0 : starPos.x / size,
                starPos.y == 0 ? 0 : starPos.y / size,
                starPos.z == 0 ? 0 : starPos.z / size);

            box.Data = new BoxData()
            {
                Type = shadowBaseBox.Data.Type,
                ArrayPosition = Arrayposition,
                Rotation = shadowBaseBox.Data.Rotation,
                Size = shadowBaseBox.Data.Size
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
            //Validator.ValidatorController.Validate(Level);
        }

        private bool IsBoxInPosition(Vector3 arrayPosition)
        {
            foreach (var variable in Level)
            {
                switch (variable.Data.Type)
                {
                    case BaseBox.BlockType.None:
                    case BaseBox.BlockType.TapFlowBox:
                    case BaseBox.BlockType.RotateRoadBox:
                    case BaseBox.BlockType.SwipedBox:
                        if (variable.IsBoxInPosition(arrayPosition))
                            return true;
                        continue;
                    case BaseBox.BlockType.BigBoxTapFlowBox:
                        var bigBox = (variable as BigBoxTapFlowBox);
                        if (bigBox != null)
                        {
                            if (bigBox.IsBoxInPosition(arrayPosition))
                                return true;
                        }

                        continue;
                }
            }

            return false;
        }
        
        public bool IsBoxInPosition(Vector3 arrayPosition, BaseBox box)
        {
            foreach (var variable in Level)
            {
                if (variable == box)
                {
                    continue;
                }
                switch (variable.Data.Type)
                {
                    case BaseBox.BlockType.None:
                    case BaseBox.BlockType.TapFlowBox:
                    case BaseBox.BlockType.RotateRoadBox:
                    case BaseBox.BlockType.SwipedBox:
                        if (variable.IsBoxInPosition(arrayPosition))
                        {
                            return true;
                        }
                        continue;
                    case BaseBox.BlockType.BigBoxTapFlowBox:
                        var bigBox = (variable as BigBoxTapFlowBox);
                        if (bigBox != null)
                        {
                            if (bigBox.IsBoxInPosition(arrayPosition))
                                return true;
                        }

                        continue;
                }
            }

            return false;
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

            var boxGameObject = await InstantiateAssetAsync(GetAddressableName(data));
            var box = boxGameObject.GetComponent<BaseBox>();
            box.transform.position = data.ArrayPosition.ToVector3() * size;
            box.transform.rotation = Quaternion.Euler(data.Rotation);
            box.Data = data;
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
            {
                Debug.LogError("Has collisions: " + _collisions.Count);
            }

            ValidatorController.Validate(Level);
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
    }
}