using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

public class TutorialRotate : MonoBehaviour
{
    private Vector3 _startCameraPosition;
    private Camera _camera;
    async void Start()
    {
        gameObject.SetActive(true);
        _camera = Camera.main;
        _startCameraPosition = _camera.transform.position;
        await WaitForDistance();
        GameManager.Instance.InputController.SetActiveAllInput(true);
        gameObject.SetActive(false);

    }

    private async UniTask WaitForDistance()
    {
        while (Vector3.Distance(_startCameraPosition, _camera.transform.position) < 20)
        {
            await UniTask.WaitForEndOfFrame(this);
        }
    }
    
}
