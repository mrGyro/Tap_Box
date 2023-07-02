using Managers;
using Tutorial;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour, IInitializable
{
    [SerializeField] private TouchTutorial _touchTutorial;
    [SerializeField] private TutorialRotate _rotateTutorial;
    [SerializeField] private TutorialZoom _tutorialZoom;

    public void Initialize()
    {
        Core.MessengerStatic.Messenger<string>.AddListener(Constants.Events.OnLevelCreated, OnLevelChanged);
    }

    private void OnLevelChanged(string obj)
    {
        switch (obj)
        {
            case "1":
                ShowFirstStage();
                break;            
            case "2":
                ShowSecondStage();
                break;
            case "3":
                ShowThirdStage();
                break;
        }
    }

    private void ShowFirstStage()
    {
        GameManager.Instance.InputController.SetActiveAllInput(false);
        GameManager.Instance.InputController.SetActiveTouchInput(true);
        _touchTutorial.gameObject.SetActive(true);
    }    
    
    private void ShowSecondStage()
    {
        GameManager.Instance.InputController.SetActiveAllInput(false);
        GameManager.Instance.InputController.SetActiveRotateInput(true);
        _rotateTutorial.gameObject.SetActive(true);
    }    
    
    private void ShowThirdStage()
    {
        GameManager.Instance.InputController.SetActiveAllInput(false);
        GameManager.Instance.InputController.SetActiveRotateInput(true);
        _tutorialZoom.gameObject.SetActive(true);
    }
}
