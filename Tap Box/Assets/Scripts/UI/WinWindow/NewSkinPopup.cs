using DefaultNamespace.Managers;
using UI.Skins;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WinWindow
{
    public class NewSkinPopup : PopUpBase
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _background;
        public override void Initialize()
        {
            ID = Constants.PopUps.WinPopUp;
            Priority = 1;
        }

        public void Setup(SkinData data)
        {
            
        }
    }
}
