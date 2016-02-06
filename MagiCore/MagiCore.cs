using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MagiCore
{
    public class MagiCore
    {
        public static Version GetVersion()
        {
            return new Version(1, 1, 0, 0);
        }
    }

    public class Utilities
    {
        /// <summary>
        /// Takes a time and splits it into an integer array describing the number of years, days, hours, minutes, seconds
        /// </summary>
        /// <param name="UT"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Formats a string from a time value into X days, X hours, X minutes, and X seconds.
        /// </summary>
        /// <param name="time">Time in seconds</param>
        /// <returns></returns>
        public static string GetFormattedTime(double time)
        {
            if (time > 0)
            {
                StringBuilder formatedTime = new StringBuilder();
                if (GameSettings.KERBIN_TIME)
                {
                    formatedTime.AppendFormat("{0,2:0} days, ", Math.Floor(time / 21600));
                    time = time % 21600;
                }
                else
                {
                    formatedTime.AppendFormat("{0,2:0} days, ", Math.Floor(time / 86400));
                    time = time % 86400;
                }
                formatedTime.AppendFormat("{0,2:0} hours, ", Math.Floor(time / 3600));
                time = time % 3600;
                formatedTime.AppendFormat("{0,2:0} minutes, ", Math.Floor(time / 60));
                time = time % 60;
                formatedTime.AppendFormat("{0,2:0} seconds", time);

                return formatedTime.ToString();
            }
            else
            {
                return "0 days,  0 hours,  0 minutes,  0 seconds";
            }

        }

        /// <summary>
        /// Formats a time in "colon format" such as "1:23:45:54"
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetColonFormattedTime(double time)
        {
            if (time > 0)
            {
                StringBuilder formatedTime = new StringBuilder();
                if (GameSettings.KERBIN_TIME)
                {
                    formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / 21600));
                    time = time % 21600;
                }
                else
                {
                    formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / 86400));
                    time = time % 86400;
                }
                formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / 3600));
                time = time % 3600;
                formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / 60));
                time = time % 60;
                formatedTime.AppendFormat("{0,2:00}", time);

                return formatedTime.ToString();
            }
            else
            {
                return "00:00:00:00";
            }
        }


        /// <summary>
        /// Converts a string containing time elements to a UT
        /// </summary>
        /// <param name="timeString"></param>
        /// <param name="toUT"></param>
        /// <returns>Time in seconds</returns>
        public static double ParseTimeString(string timeString, bool toUT = true)
        {
            //if it doesn't contain colons, we assume it's not colon formatted
            if (timeString.Contains(":"))
            {
                return ParseColonFormattedTime(timeString, toUT);
            }
            else if (timeString.Contains("s") || timeString.Contains("m") || timeString.Contains("h") || timeString.Contains("d") || timeString.Contains("y"))
            {
                return ParseCommonFormattedTime(timeString, toUT);
            }
            else
            {
                return double.Parse(timeString);
            }

        }

        /// <summary>
        /// Takes a string with qualifiers like y, d, h, m, s and converts it to seconds
        /// </summary>
        /// <param name="timeString"></param>
        /// <param name="toUT"></param>
        /// <returns></returns>
        public static double ParseCommonFormattedTime(string timeString, bool toUT = true)
        {
            //parses strings like "12d 14h 32m" or "3y8d"
            double time = -1;
            timeString = timeString.ToLower(); //make sure everything is lowercase
            string[] parts = Regex.Split(timeString, "([a-z])");//split on characters (should also include the character as the next element of the array)
            int len = parts.Length;
            double sPerDay = GameSettings.KERBIN_TIME ? 6 * 3600 : 24 * 3600;
            double sPerYear = GameSettings.KERBIN_TIME ? 426 * sPerDay : 365 * sPerDay;

            //loop over all the elements, if it's y,d,h,m,s then take the previous element as the number
            if (len > 1)
            {
                for (int i = 1; i < len; i++)
                {
                    double multiplier = 1;
                    double value = 0;

                    string s = parts[i].Trim();
                    if (s == "s")
                    {
                        //seconds
                        multiplier = 1;
                        double.TryParse(parts[i - 1], out value);
                    }
                    else if (s == "m")
                    {
                        //minutes
                        multiplier = 60;
                        double.TryParse(parts[i - 1], out value);
                    }
                    else if (s == "h")
                    {
                        //hours
                        multiplier = 3600;
                        double.TryParse(parts[i - 1], out value);
                    }
                    else if (s == "d")
                    {
                        //days
                        multiplier = sPerDay;
                        double.TryParse(parts[i - 1], out value);
                        if (toUT)
                            value -= 1;
                    }
                    else if (s == "y")
                    {
                        //years
                        multiplier = sPerYear;
                        double.TryParse(parts[i - 1], out value);
                        if (toUT)
                            value -= 1;
                    }

                    time += multiplier * value;
                }
            }
            return time;
        }

        /// <summary>
        /// Takes a colon formatted time string ("1:23:45:54") and converts it to seconds
        /// </summary>
        /// <param name="timeString"></param>
        /// <param name="toUT"></param>
        /// <returns></returns>
        public static double ParseColonFormattedTime(string timeString, bool toUT = true)
        {
            //toUT is for converting a string that is given as a formatted UT (Starting with Y1, D1)
            double time = -1;
            string[] parts = timeString.Split(':');
            int len = parts.Length;
            double sPerDay = GameSettings.KERBIN_TIME ? 6 * 3600 : 24 * 3600;
            double sPerYear = GameSettings.KERBIN_TIME ? 426 * sPerDay : 365 * sPerDay;
            try
            {
                time = double.Parse(parts[len - 1]);
                if (len > 1)
                    time += 60 * double.Parse(parts[len - 2]); //minutes
                if (len > 2)
                    time += 3600 * double.Parse(parts[len - 3]); //hours
                if (len > 3)
                {
                    time += sPerDay * double.Parse(parts[len - 4]); //days
                    if (toUT)
                        time -= sPerDay;
                }
                if (len > 4)
                {
                    time += sPerYear * double.Parse(parts[len - 5]); //years
                    if (toUT)
                        time -= sPerYear;
                }
            }
            catch
            {
                time = -1;
            }
            return time;
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

    public class MathParsing
    {
        /// <summary>
        /// Takes a string and evaluates it as a mathematical expression, replacing any variables "ie, [X]" with the provided values
        /// </summary>
        /// <param name="input"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static double ParseMath(string input, Dictionary<string, string> variables)
        {
            string raw = input;
            if (variables != null)
            {
                foreach (KeyValuePair<string, string> kvp in variables)
                {
                    if (input.Contains("[" + kvp.Key + "]"))
                    {
                        input = input.Replace("[" + kvp.Key + "]", kvp.Value);
                    }
                }
            }

            double currentVal = 0;
            string stack = "";
            string lastOp = "+";
            string[] ops = { "+", "-", "*", "/", "%", "^", "(", "e", "E" };
            string[] functions = { "min", "max", "l", "L", "abs", "sign", "if" }; //if (a < b ? stuff : other stuff)
            for (int i = 0; i < input.Length; ++i)
            {
                string ch = input[i].ToString();
                bool isOp = false, isFunction = false;
                foreach (string op in ops)
                {
                    if (op == ch)
                    {
                        isOp = true;
                        break;
                    }
                }
                if (!isOp)
                {
                    foreach (string fun in functions)
                    {
                        if (fun[0] == input[i])
                        {
                            isFunction = true;
                            break;
                        }
                    }
                }

                if (isOp)
                {
                    if (ch == "-" && (stack.Trim() == ""))
                    {
                        stack += ch;
                    }
                    else if (ch == "e" || ch == "E")
                    {
                        int index;
                        for (index = i + 2; index < input.Length; ++index)
                        {
                            string ch2 = input[index].ToString();
                            if (ops.Contains(ch2))
                                break;
                        }
                        string sub = input.Substring(i + 1, index - i - 1);
                        double exp = ParseMath(sub, variables);
                        double newVal = double.Parse(stack) * Math.Pow(10, exp);
                        currentVal = DoMath(currentVal, lastOp, newVal.ToString());
                        stack = "0";
                        lastOp = "+";
                        i = index - 1;
                    }
                    else if (ch == "(")
                    {
                        int j = FindEndParenthesis(input, i)[0];
                        string sub = input.Substring(i + 1, j - i - 1);
                        string val = ParseMath(sub, variables).ToString();
                        input = input.Substring(0, i) + val + input.Substring(j + 1);
                        --i;
                    }
                    else
                    {
                        currentVal = DoMath(currentVal, lastOp, stack);
                        lastOp = ch;
                        stack = "";
                    }
                }
                else if (isFunction)
                {
                    int subStart = input.IndexOf('(', i) + 1;
                    string function = input.Substring(i, subStart - i - 1);
                    int[] parenComma = FindEndParenthesis(input, subStart - 1);
                    int j = parenComma[0];
                    int comma = parenComma[1];
                    string sub = input.Substring(subStart, j - subStart);
                    double val = 0.0;

                    if (function == "if")
                    {
                        val = DoIfStatement(sub);
                    }
                    else if (function == "l")
                    {
                        val = ParseMath(sub, variables);
                        val = Math.Log(val);
                    }
                    else if (function == "L")
                    {
                        val = ParseMath(sub, variables);
                        val = Math.Log10(val);
                    }
                    else if (function == "max" || function == "min")
                    {
                        string[] parts = new string[2];
                        parts[0] = input.Substring(subStart, parenComma[1] - subStart);
                        parts[1] = input.Substring(parenComma[1] + 1, j - parenComma[1] - 1);
                        double sub1 = ParseMath(parts[0], variables);
                        double sub2 = ParseMath(parts[1], variables);
                        if (function == "max")
                            val = Math.Max(sub1, sub2);
                        else if (function == "min")
                            val = Math.Min(sub1, sub2);
                    }
                    else if (function == "sign")
                    {
                        val = ParseMath(sub, variables);
                        if (val >= 0)
                            val = 1;
                        else
                            val = -1;
                    }
                    else if (function == "abs")
                    {
                        val = ParseMath(sub, variables);
                        val = Math.Abs(val);
                    }

                    input = input.Substring(0, i) + val.ToString() + input.Substring(j + 1);
                    i--;
                }
                else
                {
                    stack += ch;
                }
            }
            currentVal = DoMath(currentVal, lastOp, stack);
            return currentVal;
        }

        /// <summary>
        /// Locates the terminating paranthesis for the given position. Also finds the location of any commas (for max, min, etc)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="curPos"></param>
        /// <returns></returns>
        private static int[] FindEndParenthesis(string str, int curPos)
        {
            int depth = 0;
            int j = 0, commaPos = -1;
            for (j = curPos + 1; j < str.Length; ++j)
            {
                if (str[j] == '(') depth++;
                if (str[j] == ')') depth--;

                if (str[j] == ',' && depth == 0)
                    commaPos = j;

                if (depth < 0)
                {
                    break;
                }
            }
            return new int[] { j, commaPos };
        }

        /// <summary>
        /// Takes the current value and performs the requested operation on it
        /// </summary>
        /// <param name="currentVal"></param>
        /// <param name="operation"></param>
        /// <param name="newVal"></param>
        /// <returns></returns>
        private static double DoMath(double currentVal, string operation, string newVal)
        {
            double newValue = 0;
            if (String.IsNullOrEmpty(newVal))
                return currentVal;
            if (!double.TryParse(newVal, out newValue))
            {
                UnityEngine.Debug.LogError("[MagiCore] Tried to parse a non-double value: " + newVal);
                return currentVal;
            }
            switch (operation)
            {
                case "+": currentVal += newValue; break;
                case "-": currentVal -= newValue; break;
                case "*": currentVal *= newValue; break;
                case "/": currentVal /= newValue; break;
                case "%": currentVal = currentVal % long.Parse(newVal); break;
                case "^": currentVal = Math.Pow(currentVal, newValue); break;
                default: break;
            }

            return currentVal;
        }

        /// <summary>
        /// Performs parsing of inline if statements
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        private static double DoIfStatement(string statement)
        {
            //At this point "statement" would look something like a < b ? stuff : other stuff
            //We need to grab the conditional and the two possible values, then do the conditional and evaluate the correct value
            string[] conditionals = { "<", ">", "<=", ">=", "==", "!=" }; //do we want to support && and ||, too? Ideally yes, but it's way tougher
            double val = 0.0;

            int indexOfQMark = statement.IndexOf("?");
            string conditionalSection = statement.Substring(0, indexOfQMark);
            //string[] condSides = conditionalSection.Split(conditionals, StringSplitOptions.RemoveEmptyEntries);
            foreach (string conditional in conditionals)
                conditionalSection = conditionalSection.Replace(conditional, ";" + conditional + ";");
            string[] parts = conditionalSection.Split(';');
            //this is a kind of horrible method that I got from here, but avoided excessive regex: http://stackoverflow.com/a/2484982

            //lets assume there are only three elements
            if (parts.Length < 3)
                return 0.0;

            //do math on part one
            //do math on part two
            //compare them
            double val1 = ParseMath(parts[0], null);
            double val2 = ParseMath(parts[2], null);

            int indexOfColon = indexOfQMark;
            int depth = 0;
            for (int i = indexOfQMark+1; i < statement.Length; i++)
            {
                char s = statement[i];
                if (s == '(') depth++;
                if (s == ')') depth--;

                if (s == ':' && depth == 0)
                    indexOfColon = i;

                if (depth < 0)
                    break;
            }

            string leftOption = statement.Substring(indexOfQMark + 1, indexOfColon-indexOfQMark);//starts at "?" and ends at last ":" for this depth
            string rightOption = statement.Substring(indexOfColon + 1); //starts at last ":" and ends at end of statment

            string selectedOption = "";

            switch (parts[1])
            {
                case "<": selectedOption = val1 < val2 ? leftOption : rightOption; break;
                case ">": selectedOption = val1 > val2 ? leftOption : rightOption; break;
                case "<=": selectedOption = val1 <= val2 ? leftOption : rightOption; break;
                case ">=": selectedOption = val1 >= val2 ? leftOption : rightOption; break;
                case "==": selectedOption = val1 == val2 ? leftOption : rightOption; break;
                case "!=": selectedOption = val1 != val2 ? leftOption : rightOption; break;
                default: selectedOption = leftOption; break;
            }

            val = ParseMath(selectedOption, null);

            return val;
        }
    }
}
    