using UniRx;

namespace Ads
{
    public interface IAdElement
    {
        ReactiveProperty<bool> IsReady { get; set; }
        void Show();
        void Hide();
        void Load();
        void Init();
    }
}