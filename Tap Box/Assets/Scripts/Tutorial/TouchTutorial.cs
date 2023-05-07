using Core.MessengerStatic;
using UnityEngine;

public class TouchTutorial : MonoBehaviour
{
    void Start()
    {

        Messenger.AddListener(Constants.Events.OnBoxClicked, OnClick);
    }

    private void OnClick()
    {
        Messenger.RemoveListener(Constants.Events.OnBoxClicked, OnClick);
        gameObject.SetActive(false);
    }
}