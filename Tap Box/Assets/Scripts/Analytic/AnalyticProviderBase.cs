using Unity.VisualScripting;
using UnityEngine;

namespace Analitics
{
    public class AnalyticProviderBase : MonoBehaviour, IInitializable
    {
        public virtual void Initialize()
        {
            
        }

        public virtual void SendEvent(string eventName, string paramName, string value)
        {
            
        }
    }
}
