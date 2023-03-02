using DefaultNamespace.Managers;

namespace UI.Skins
{
    public class SkinsPoUp : PopUpBase
    {
        private void Start()
        {
            ID = Constants.PopUps.SkinsPopUp;
            Priority = 1;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }
        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
