using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagiCore
{
    public class MagiCore
    {
        public static Version GetVersion()
        {
            return new Version(1, 0, 0, 0);
        }
    }

    public class Utilities
    {
        public static int[] ConvertUT(double UT)
        {
            double time = UT;
            int[] ret = { 0, 0, 0, 0, 0 };
            ret[0] = (int)Math.Floor(time / (KSPUtil.Year)) + 1; //year
            time %= (KSPUtil.Year);
            ret[1] = (int)Math.Floor(time / KSPUtil.Day) + 1; //days
            time %= (KSPUtil.Day);
            ret[2] = (int)Math.Floor(time / (3600)); //hours
            time %= (3600);
            ret[3] = (int)Math.Floor(time / (60)); //minutes
            time %= (60);
            ret[4] = (int)Math.Floor(time); //seconds

            return ret;
        }
    }

    public class StringTranslation
    {
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


        public static string ReplaceToken(string sourceString, string variable, string value)
        {
            string lwrString = sourceString.ToLower();

            int index = 0;
            while ((index = lwrString.IndexOf("[" + variable.ToLower() + "]")) >= 0 || (index = lwrString.IndexOf("<" + variable.ToLower() + ">")) >= 0)
            {
                string newStr = sourceString.Substring(0, index) + value; //verify
                if (index + 2 + variable.Length < sourceString.Length)
                    newStr += sourceString.Substring(index+2+variable.Length);
                sourceString = newStr;
                lwrString = sourceString.ToLower();
            }

            return sourceString;
        }

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
    /*public class MagiCore
    {
        //In case we want to add this stuff in later
        private static Dictionary<string, Func<string, string>> registeredMods = new Dictionary<string, Func<string, string>>();

        public void Start()
        {
            RegisterMod("MagiCore", DefaultParser);
        }

        private string DefaultParser(string original)
        {
            return AddFormatInfo(original, "MagiCore", "yyyy-MM-dd--HH-mm-ss");
        }

        public static void RegisterMod(string modName, Func<string, string> callback)
        {
            if (!registeredMods.ContainsKey(modName))
                registeredMods.Add(modName, callback);
            else
                registeredMods[modName] = callback;
        }
    }
}
*/