using Core.MessengerStatic;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace VFX
{
    public class TapEffectController : MonoBehaviour, IInitializable
    {
        private GameObject _currentEffect;

        public async void Initialize()
        {
            Messenger<GameObject>.AddListener(Constants.Events.OnTapSkinChanged, OnTapSkinChanged);
            Messenger<Vector3>.AddListener(Constants.Events.OnTapShow, OnTapShow);
            _currentEffect = await GameManager.Instance.SkinsManager.GetTapSkin();
        }

        private void OnTapShow(Vector3 obj)
        {
            GameObject x = Instantiate(_currentEffect, obj, Quaternion.identity, transform);
            Destroy(x, 1);
        }

        private void OnTapSkinChanged(GameObject obj)
        {
            _currentEffect = obj;
        }

        private void OnDestroy()
        {
            Messenger<GameObject>.RemoveListener(Constants.Events.OnTapSkinChanged, OnTapSkinChanged);
            Messenger<Vector3>.RemoveListener(Constants.Events.OnTapShow, OnTapShow);
        }
    }
}