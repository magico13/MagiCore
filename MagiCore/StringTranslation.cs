using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagiCore
{
    /// <summary>
    /// Similar to variable replacement, but purely for strings. For Dated Quicksave and Sensible Screenshot. Not for language translations unfortunately
    /// </summary>
    public class StringTranslation
    {
        /// <summary>
        /// The main entry point. Takes a string, replaces "variables", and returns the final string. Handles common values like "[year]"
        /// </summary>
        /// <param name="original">The original, source string</param>
        /// <param name="caller">The calling mod's name</param>
        /// <param name="DateString">A C# date string</param>
        /// <param name="extraVariables">Any additional variables to use for replacements</param>
        /// <returns>The string post replacements</returns>
        public static string AddFormatInfo(string original, string caller, string DateString, Dictionary<string, string> extraVariables = null)
        {
            string convertedDate = DateTime.Now.ToString(DateString);

            string replaced = original;
            replaced = ReplaceToken(replaced, "date", convertedDate);
            //Take original and replace all the common things (aka, the variables all our mods share, like [year])
            replaced = ReplaceStandardTokens(replaced);

            //Take that string and then replace all the extraVariables (mod specific things that get passed through, like the event that triggered a screenshot)
            if (extraVariables != null)
            {
                foreach (KeyValuePair<string, string> kvp in extraVariables)
                {
                    replaced = ReplaceToken(replaced, kvp.Key, kvp.Value); //ReplaceToken is a function that replaces [X] with the value (regardless of case or if it's wrapped with [] or <>)
                }
            }

            return replaced;
        }

        /// <summary>
        /// Replaces all instances of a particular token "[token] or <token>" with the provided value
        /// </summary>
        /// <param name="sourceString">The original string to perform the replacement in</param>
        /// <param name="variable">The variable to replace with the value</param>
        /// <param name="value">The value to replace the variable with</param>
        /// <returns>The string post replacement</returns>
        public static string ReplaceToken(string sourceString, string variable, string value)
        {
            string lwrString = sourceString.ToLower();

            int index = 0;
            while ((index = lwrString.IndexOf("[" + variable.ToLower() + "]")) >= 0 || (index = lwrString.IndexOf("<" + variable.ToLower() + ">")) >= 0)
            {
                string newStr = sourceString.Substring(0, index) + value; //verify
                if (index + 2 + variable.Length < sourceString.Length)
                    newStr += sourceString.Substring(index + 2 + variable.Length);
                sourceString = newStr;
                lwrString = sourceString.ToLower();
            }

            return sourceString;
        }

        /// <summary>
        /// Replaces the common tokens like "[year]" with their appropriate values as gathered from KSP
        /// </summary>
        /// <param name="sourceString">The source string to act on</param>
        /// <returns>The string post replacements</returns>
        public static string ReplaceStandardTokens(string sourceString)
        {
            string str = sourceString;
            str = ReplaceToken(str, "UT", Planetarium.fetch != null ? Math.Round(Planetarium.GetUniversalTime()).ToString() : "0");
            str = ReplaceToken(str, "save", HighLogic.SaveFolder != null && HighLogic.SaveFolder.Trim().Length > 0 ? HighLogic.SaveFolder : "NA");
            str = ReplaceToken(str, "version", Versioning.GetVersionString());
            str = ReplaceToken(str, "vessel", HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null ? FlightGlobals.ActiveVessel.vesselName : "NA");
            str = ReplaceToken(str, "body", Planetarium.fetch != null ? Planetarium.fetch.CurrentMainBody.bodyName : "NA");
            str = ReplaceToken(str, "situation", HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null ? FlightGlobals.ActiveVessel.situation.ToString() : "NA");
            str = ReplaceToken(str, "biome", HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null ? ScienceUtil.GetExperimentBiome(FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude) : "NA");


            int[] times = { 0, 0, 0, 0, 0 };
            if (Planetarium.fetch != null)
                times = Utilities.ConvertUT(Planetarium.GetUniversalTime());
            str = ReplaceToken(str, "year", times[0].ToString());
            str = ReplaceToken(str, "year0", times[0].ToString("D3"));
            str = ReplaceToken(str, "day", times[1].ToString());
            str = ReplaceToken(str, "day0", times[1].ToString("D3"));
            str = ReplaceToken(str, "hour", times[2].ToString());
            str = ReplaceToken(str, "hour0", times[2].ToString("D2"));
            str = ReplaceToken(str, "min", times[3].ToString());
            str = ReplaceToken(str, "min0", times[3].ToString("D2"));
            str = ReplaceToken(str, "sec", times[4].ToString());
            str = ReplaceToken(str, "sec0", times[4].ToString("D2"));


            string time = KSPUtil.PrintTimeCompact(0, false);
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
                time = KSPUtil.PrintTimeCompact((int)FlightGlobals.ActiveVessel.missionTime, false);
            time = time.Replace(":", "-"); //Can't use colons in filenames on Windows, so we'll replace them with "-"

            str = ReplaceToken(str, "MET", time);

            return str;
        }
    }
}
