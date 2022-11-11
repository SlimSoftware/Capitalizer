
namespace CapitalizerLib
{
    public enum AddFolderMethod { AddFolder, AddContents };

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

        /// <summary>
        /// The remembered add folder method, null if the user has not saved the add method
        /// </summary>
        public static AddFolderMethod? AddFolderMethod { get; set; } = null;
    }
}
