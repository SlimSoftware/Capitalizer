using System.IO;
using System.Text.RegularExpressions;

namespace CapitalizerLib
{
    public enum CapitalizeMode { EveryWord, LikeSentence, LowerCase, UpperCase, 
        RemoveExtraSpaces, FindReplace };

    public static class CapitalizeHelper
    {
        public static string Capitalize(string fileName, CapitalizeMode mode)
        {
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

            switch (mode)
            {
                case CapitalizeMode.EveryWord:
                    fileNameWithoutExt = fileNameWithoutExt.ToLower();
                    fileNameWithoutExt = Regex.Replace(fileNameWithoutExt, @"\b\w", m => m.Value.ToUpper());
                    break;
                case CapitalizeMode.LikeSentence:
                    fileNameWithoutExt = CapitalizeFirstLetter(fileNameWithoutExt);
                    break;
                case CapitalizeMode.LowerCase:
                    fileNameWithoutExt = fileNameWithoutExt.ToLower();
                    break;
                case CapitalizeMode.UpperCase:
                    fileNameWithoutExt = fileNameWithoutExt.ToUpper();
                    break;
                case CapitalizeMode.RemoveExtraSpaces:
                    fileNameWithoutExt = RemoveExtraSpaces(fileNameWithoutExt);
                    break;
                case CapitalizeMode.FindReplace:
                    fileNameWithoutExt = fileNameWithoutExt.Replace(Settings.FindString, Settings.ReplaceWithString);
                    break;
            }

            string fileNameExt = Path.GetExtension(fileName);
            return $"{fileNameWithoutExt}{fileNameExt.ToLower()}";
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
