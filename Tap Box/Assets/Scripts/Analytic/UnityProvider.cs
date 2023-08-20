using System.Collections.Generic;
using Analitics;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Analytic
{
    public class UnityProvider : AnalyticProviderBase
    {
        public override async void Initialize()
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
        }

        public override void SendEvent(string eventName, string paramName, string value)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { paramName, value }
            };
            
            Debug.Log(paramName + " " + value);
            AnalyticsService.Instance.CustomData(eventName, parameters);
            AnalyticsService.Instance.Flush();
        }
    }
}