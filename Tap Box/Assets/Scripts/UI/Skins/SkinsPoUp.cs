using DefaultNamespace.Managers;

namespace UI.Skins
{
    public class SkinsPoUp : PopUpBase
    {
        // Start is called before the first frame update
        void Start()
        {
            ID = Constants.PopUps.SkinsPopUp;
            Priority = 0;
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
