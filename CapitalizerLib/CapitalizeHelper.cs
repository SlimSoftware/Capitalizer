using System.Text.RegularExpressions;

namespace CapitalizerLib
{
    public enum CapitalizeMode { EveryWord, LikeSentence, LowerCase, UpperCase, 
        RemoveExtraSpaces, FindReplace };

    public static class CapitalizeHelper
    {
        public static string Capitalize(string fileName, CapitalizeMode mode)
        {
            switch (mode)
            {
                case CapitalizeMode.EveryWord:
                    return Regex.Replace(fileName, @"\b([a-z])", m => m.Value.ToUpper());
                case CapitalizeMode.LikeSentence:
                    return CapitalizeFirstLetter(fileName);
                case CapitalizeMode.LowerCase:
                    return fileName.ToLower();
                case CapitalizeMode.UpperCase:
                    return fileName.ToUpper();
                case CapitalizeMode.RemoveExtraSpaces:
                    return RemoveExtraSpaces(fileName);
                case CapitalizeMode.FindReplace:
                    return fileName.Replace(Settings.FindString, Settings.ReplaceWithString);
                default:
                    return fileName;
            }
        }

        private static string CapitalizeFirstLetter(string input)
        {
            string firstLetter = input[0].ToString().ToUpper();
            string lowerCaseRemaining = input[1..].ToLower();
            return $"{firstLetter}{lowerCaseRemaining}";
        }
        
        private static string RemoveExtraSpaces(string input)
        {
            string newString = input;
            while (newString.Contains("  "))
            {
                newString = newString.Replace("  ", " ");
            }

            return newString;
        }
    }
}
