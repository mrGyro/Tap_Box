using System;
using Unity.VisualScripting;
using UnityEngine;

namespace UI.Skins
{
    public class StateButton : MonoBehaviour, IInitializable
    {
        public Action<StateButton, bool> OnClick;
        [SerializeField] private GameObject _defaultState;
        [SerializeField] private GameObject _selectedState;
        [SerializeField] private bool _state;

        public void InvertState()
        {
            SetState(!_state);
        }

        public void OnClickEvent()
        {
            OnClick?.Invoke(this, _state);
        }
        
        
        public void SetState(bool value)
        {
            if (_state == value)
            {
                return;
            }
            Debug.LogError(name + " " + value);

            _state = value;
            _defaultState.SetActive(!value);
            _selectedState.SetActive(value);
        }

        public void Initialize()
        {
            SetState(_state);
        }
    }
}