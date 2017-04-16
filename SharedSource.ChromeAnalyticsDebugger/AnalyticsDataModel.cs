using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Analytics.Tracking;

namespace SharedSource.ChromeAnalyticsDebugger
{
    public class AnalyticsDataModel
    {
        public AnalyticsDataModel()
        {
            ProfileValues = new List<AnalyticsProfileValue>();
            MatchedProfiles = new List<AnalyticsMatchedProfile>();
        }

        public List<AnalyticsProfileValue> ProfileValues { get; set; }
        public int VisitValue { get; set; }
        public int PageCount { get; set; }
        public List<AnalyticsMatchedProfile> MatchedProfiles { get; set; }
        public ContactLocation GeoDetails { get; set; }
        public string ContactIdentifier { get; set; }
    }

    public class AnalyticsMatchedProfile
    {
        public string ProfileName { get; set; }
        public Guid? PatternId { get; set; }
        public string PatternName { get; set; }
        public List<KeyValuePair<string, float>> Values { get; set; }
        public float Total { get; set; }
    }

    public class AnalyticsProfileValue
    {
        public string ProfileName { get; set; }
        public Guid ProfileId { get; set; }
        public List<KeyValuePair<string, float>> Values { get; set; } 
    }
}