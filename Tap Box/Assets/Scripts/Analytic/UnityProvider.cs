using System.Collections.Generic;
using Analitics;
using Unity.Services.Analytics;
using Unity.Services.Core;

namespace Analytic
{
    public class UnityProvider : AnalyticProviderBase
    {
        public override async void Initialize()
        {
            try
            {
                await UnityServices.InitializeAsync();
                //List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
            }
            catch (ConsentCheckException e)
            {
                // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
            }
        }

        public override void SendEvent(string eventName, string paramName, string value)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { paramName, value }
            };
            AnalyticsService.Instance.CustomData(eventName, parameters);
            AnalyticsService.Instance.Flush();
        }
    }
}