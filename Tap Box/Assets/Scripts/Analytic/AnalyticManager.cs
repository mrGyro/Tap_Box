using System.Collections.Generic;
using Analitics;
using Unity.VisualScripting;
using UnityEngine;

namespace Analytic
{
    public class AnalyticManager : MonoBehaviour, IInitializable
    {
        [SerializeField] private List<AnalyticProviderBase> _analyticProviders;

        public void Initialize()
        {
            foreach (var provider in _analyticProviders)
            {
                if (provider != null)
                {
                    provider.Initialize();
                }
            }
        }

        public virtual void SendEvent(string eventName, string paramName, string value)
        {
            foreach (var provider in _analyticProviders)
            {
                provider.SendEvent(eventName, paramName, value);
            }
        }
    }
}