using System;
using Core.MessengerStatic;
using UnityEngine;

public class TouchTutorial : MonoBehaviour
{
    private void OnEnable()
    {
        Messenger.AddListener(Constants.Events.OnBoxClicked, OnClick);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(Constants.Events.OnBoxClicked, OnClick);
    }

    private void OnClick()
    {
        gameObject.SetActive(false);
    }
}