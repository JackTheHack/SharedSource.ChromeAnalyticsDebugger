using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Data.Items;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SharedSource.ChromeAnalyticsDebugger
{
    public class PersonalisationHelper
    {
        
        public static readonly ID ProfilesId = new ID("{12BD7E35-437B-449C-B931-23CFA12C03D8}");

        public static List<ProfileItem> GetProfiles()
        {
            Item item = Context.Database.GetItem(ProfilesId);

            if (item != null)
            {
                return item
                    .GetChildren(ChildListOptions.None)
                    .Select(i => new ProfileItem(i))
                    .Where(i => i != null)
                    .ToList();
            }
            return new List<ProfileItem>();
        }

        public static string GetProfilePattern(string profileName)
        {
            var profiles = Tracker.CurrentVisit.Profiles;
            int minScoreCount = Settings.GetIntSetting("Analytics.Patterns.MinimalProfileScoreCount", 0);           

            var profileItem = PersonalisationHelper.GetProfiles().Where(i => i.NameField == profileName).FirstOrDefault();


            var row = profiles[profileItem.ProfileName];

            //If any profiles have a matched PatternLabel then check if it's the Pattern selected by the condition
            if (row != null && row.PatternId != Guid.Empty && row.Count > minScoreCount)
            {
                var patternItem = Context.Database.GetItem(ID.Parse(row.PatternId));
                return patternItem != null ? new PatternCardItem(patternItem).NameField : "Default";

            }

            var visitor = Tracker.Current.Contact;
            if (visitor == null)
            {
                return null;
            }

            try
            {
                return visitor.Tags[profileName];
            }
            catch (DeletedRowInaccessibleException)
            {
                //Tag was deleted
                return null;
            }           
        }

        public static bool MatchedPattern(string profileName, string patternLabel)
        {
            try
            {
                if (Tracker.Current.Contact == null)
                {
                    return false;
                }

                var matchedPatterns = Tracker.Current.Contact.Tags[profileName+".Matched"];

                if (string.IsNullOrEmpty(matchedPatterns))
                {
                    return false;
                }

                var patterns = matchedPatterns.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

                return patterns.Contains(patternLabel);
            }
            catch (DeletedRowInaccessibleException)
            {
                return false;
            }
        }

        public static bool MatchesCurrentOrPreviousPattern(string profileName, string patternLabel)
        {
            var profilePattern = GetProfilePattern(profileName);

            return profilePattern == patternLabel;
        }
    }
}