using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

namespace Tutorial
{
    public class TutorialZoom : MonoBehaviour
    {
        private float _startCameraPosition;

        private async void OnEnable()
        {
            gameObject.SetActive(true);
            GameManager.Instance.InputController.SetActiveAllInput(true);
            _startCameraPosition = GameManager.Instance.InputController.GetZoomValue();
            await WaitForZoom();
            
            gameObject.SetActive(false);
        }

        private async UniTask WaitForZoom()
        {
            while (Mathf.Abs(_startCameraPosition - GameManager.Instance.InputController.GetZoomValue()) < 10)
            {
                await UniTask.WaitForEndOfFrame(this);
            }
        }

    }
}