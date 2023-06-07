using System.Collections.Generic;
using Boxes;
using Core.MessengerStatic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Lean.Touch;
using Managers;
using Sounds;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] LeanCameraZoom _zoom;
    [SerializeField] LeanRotate _rotate;

    private bool _isTouchEnable = true;
    private bool _isZoomEnable = true;
    private bool _isSwipeEnable = true;
    private int _layerMask;
    private const string GameFieldElement = "GameFieldElement";
    private AndroidNativeVibrationService _nativeVibration;
    private Vector3 _newTarget;
    
    public void SetActiveAllInput(bool value)
    {
        LeanTouch.Fingers.Clear();
        _isTouchEnable = value;
        _isZoomEnable = value;
        _isSwipeEnable = value;
    }
    
    public void SetActiveTouchInput(bool value)
    {
        LeanTouch.Fingers.Clear();
        _isTouchEnable = value;
    }
    
    public void SetActiveZoomInput(bool value)
    {
        LeanTouch.Fingers.Clear();
        _isZoomEnable = value;
    }
    
    public void SetActiveRotateInput(bool value)
    {
        LeanTouch.Fingers.Clear();
        _isSwipeEnable = value;
    }

    public void CentreteCamera()
    {
        MoveCameraTarget();
    }

    private void Start()
    {
        _nativeVibration = new AndroidNativeVibrationService();
        _layerMask = LayerMask.GetMask(GameFieldElement);
        _zoom.SetZomValue(100);
        _rotate.SetActive(false);
        _newTarget = Vector3.zero;
    }

    private async UniTask MoveCameraTarget()
    {
        while (Vector3.Distance(_newTarget, _rotate.GetTargetPosition()) > 0.1f)
        {
            await UniTask.WaitForEndOfFrame(this);
            Vector3 newPosition = Vector3.Lerp(_rotate.GetTargetPosition(), _newTarget, 0.01f);
            _rotate.SetTargetPosition(newPosition);
        }
    }

    public void SetCameraTarget(Vector3 targetPosition)
    {
        _newTarget = targetPosition;
    }

    public async void SetStartLevelSettings(Vector3 targetPosition, Vector3 cameraPosition)
    {
        _rotate.SetActive(false);
        _rotate.SetTargetPosition(targetPosition);
        _rotate.SetStartPosition(cameraPosition);
        SetCameraTarget(targetPosition);
        await UniTask.WaitForEndOfFrame(this);
        _rotate.SetActive(true);
    }
    
    private void Swipe(List<LeanFinger> fingers)
    {
        switch (fingers.Count)
        {
            case 1:
            {
                if (_isSwipeEnable)
                {
                    _rotate.Rotate(fingers);
                }
                break;
            }
            case 2:
                if (_isZoomEnable)
                {
                    _zoom.SetZoom(fingers);
                }
                break;
        }
    }

    private void HandleFingerTap(LeanFinger finger)
    {
        if (!_isTouchEnable)
            return;
        
        var box = RaycastBox(finger.ScreenPosition);

        if (box == null)
            return;

        Vibration();
        GameManager.Instance.SoundManager.Play(new ClipDataMessage() { Id = Constants.Sounds.Game.TapOnBox, SoundType = SoundData.SoundType.Game });

        box.BoxReactionStart();
        _newTarget = GameManager.Instance.GameField.GetNewCenter();

        GameManager.Instance.GameField.GetTurnsCount--;
        Messenger.Broadcast(Constants.Events.OnBoxClicked);
        if (GameManager.Instance.GameField.IsNotWinCondition())
        {
            GameManager.Instance.UIManager.ShowPopUp(Constants.PopUps.LosePopUp);
            Messenger.Broadcast(Constants.Events.OnGameLoose);
        }
    }

    private void Vibration()
    {
        if (!GameManager.Instance.Progress.CurrentVibroSetting)
        {
            return;
        }
        
        _nativeVibration.Vibrate(30);
    }
    
    private BaseBox RaycastBox(Vector2 screenPosition)
    {
        var hit = RaycastBox(screenPosition, _layerMask);
        if (hit.collider == null)
        {
            return null;
        }
        
        var box = hit.collider.GetComponent<BaseBox>();

        if (box != null)
        {
            Messenger<Vector3>.Broadcast(Constants.Events.OnTapShow, hit.point);
        }

        return box == null ? null : box;
    }

    public RaycastHit RaycastBox(Vector2 screenPosition, int layerMask)
    {
        var ray = camera.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out var hit, 1000, layerMask))
        {
            Debug.DrawRay(camera.transform.position, ray.direction * 1000, Color.yellow, 5);
        }

        return hit;
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerTap += HandleFingerTap;
        LeanTouch.OnGesture += Swipe;
    }
    
    private void OnDestroy()
    {
        LeanTouch.OnFingerTap -= HandleFingerTap;
        LeanTouch.OnGesture -= Swipe;
    }
}