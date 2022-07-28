namespace CapitalizerLib
{
    public static class Settings
    {
        /// <summary>
        /// The string to find when the Find and Replace mode is selected
        /// </summary>
        public static string FindString { get; set; } = "_";

        /// <summary>
        /// The string to replace the found string with when the Find and Replace mode is selected
        /// </summary>
        public static string ReplaceWithString { get; set; } = " ";
    }
}
