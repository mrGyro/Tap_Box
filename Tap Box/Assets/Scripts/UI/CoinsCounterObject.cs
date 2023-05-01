using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class CoinsCounterObject : MonoBehaviour
    {
        public async void PlayAnimation(Vector3 finish)
        {
            float duration = 20f;
            float startTime = Time.time;
            Vector3 x = Vector3.zero;

            while (Vector3.Distance(transform.position, finish) > 0.1f)
            {
                var t = (Time.time - startTime) / duration;
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, t);
                await UniTask.WaitForEndOfFrame(this);
            }
        }
        
        public async void PlayScaleAnimation()
        {
            float duration = 5f;
            float startTime = Time.time;
            Vector3 x = Vector3.zero;

            while (Vector3.Distance(transform.localScale, Vector3.one) > 0.1f)
            {
                var t = (Time.time - startTime) / duration;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t);
                await UniTask.WaitForEndOfFrame(this);
            }
        }
    }
}