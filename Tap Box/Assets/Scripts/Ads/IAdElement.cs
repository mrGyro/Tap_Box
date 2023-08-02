using UniRx;

namespace Ads
{
    public interface IAdElement
    {
        ReactiveProperty<bool> IsReady { get; set; }
        bool isEnable { get; set; }
        void Show(string place);
        void Hide();
        void Load();
        void Init();
    }
}