using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Configuration;
using Sitecore.Pipelines;
using Sitecore.Pipelines.HttpRequest;

namespace SharedSource.ChromeAnalyticsDebugger.Pipelines
{
    public class AddChromeAnalyticsHeaders
    {
        public void Process(PipelineArgs args)
        {
            if (!Sitecore.Analytics.Configuration.AnalyticsSettings.Enabled)
            {
                return;
            }

            if (!Settings.GetBoolSetting("Analytics.ChromeDebugger", true))
            {
                return;
            }

            if (HttpContext.Current == null)
            {
                return;
            }

            var secretCookie = HttpContext.Current.Request.Headers["Sitecore.Analytics.ChromeDebugger.SecretCookie"];

            if (Settings.GetBoolSetting("Analytics.ChromeDebugger.UseSecret", false))
            {
                if (secretCookie == null || secretCookie != "Enabled " + Settings.GetSetting("Analytics.ChromeDebugger.SecretKey"))
                {
                    HttpContext.Current.Response.AddHeader("Sitecore.Analytics.Error", "InvalidKey");
                    return;
                }
            }

            if (string.IsNullOrEmpty(secretCookie))
            {
                return;
            }

            if (Tracker.Current == null)
            {
                return;
            }

            var data = new AnalyticsDataModel();

            SetPageProfileValues(data);
            SetProfileValues(data);
            SetGeoDetails(data);

            HttpContext.Current.Response.AddHeader("Sitecore.Analytics.Data", JsonConvert.SerializeObject(data));
        }

        private void SetGeoDetails(AnalyticsDataModel data)
        {
            data.GeoDetails = Tracker.Current.Interaction.HasGeoIpData ? Tracker.Current.Interaction.GeoData : null;
        }

        private void SetProfileValues(AnalyticsDataModel data)
        {            
            data.ContactIdentifier = Tracker.Current.Contact != null
                ? Tracker.Current.Contact.Identifiers.Identifier
                : null;
            data.PageCount = Tracker.Current.Interaction.PageCount;
            data.VisitValue = Tracker.Current.Interaction.Value;

                var currentProfiles = Tracker.Current.Interaction.Profiles.GetProfileNames();

                foreach (var profileName in currentProfiles)
                {
                    var profileData = Tracker.Current.Interaction.Profiles[profileName];
                    data.MatchedProfiles.Add(new AnalyticsMatchedProfile()
                    {
                        ProfileName = profileName,
                        PatternId = profileData.PatternId,
                        PatternName = profileData.PatternLabel,
                        Total = profileData.Total,
                        Values = profileData.ToList()
                    });
                }
            //}
        }

        private void SetPageProfileValues(AnalyticsDataModel data)
        {
            TrackingField trackingField = new TrackingField(Sitecore.Context.Item.Fields[Sitecore.Analytics.AnalyticsIds.TrackingField]);

            foreach (var profile in trackingField.Profiles)
            {
                data.ProfileValues.Add(new AnalyticsProfileValue()
                {
                    ProfileName = profile.Name,
                    ProfileId = profile.ProfileID.ToGuid(),
                    Values = profile.Keys.ToDictionary(i => i.Name, i => i.Value).ToList()
                });
            }
        }
    }


}