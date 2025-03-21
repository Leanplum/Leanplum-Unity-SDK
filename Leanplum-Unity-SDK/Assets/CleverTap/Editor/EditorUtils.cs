using System.IO;
using System.Collections.Generic;

namespace CleverTapSDK.Private
{
	public static class EditorUtils
	{
        public static readonly string CLEVERTAP_ASSETS_FOLDER = "CleverTap";
        public static readonly string CLEVERTAP_CUSTOM_TEMPLATES_FOLDER = "CustomTemplates";
        public static readonly string CLEVERTAP_APP_ASSETS_FOLDER = "CleverTapSDK";
        
        public static void DirectoryCopy(string sourceDirName,
            string destDirName,
            bool copyChangedOnly = true,
            bool copySubDirs = true,
            HashSet<string> includeOnlySubDirsNamed = null)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension == ".meta")
                    continue;

                string destPath = Path.Combine(destDirName, file.Name);
                if (copyChangedOnly)
                {
                    bool overwrite = false;
                    bool exists = File.Exists(destPath);
                    if (exists)
                    {
                        overwrite = file.LastWriteTime > File.GetLastWriteTime(destPath);
                    }

                    if (!exists || overwrite)
                        file.CopyTo(destPath, overwrite);
                }
                else
                {
                    file.CopyTo(destPath, true);
                }
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    if (includeOnlySubDirsNamed != null && !includeOnlySubDirsNamed.Contains(subdir.Name))
                    {
                        continue;
                    }
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copyChangedOnly, copySubDirs, includeOnlySubDirsNamed);
                }
            }
        }
    }
}