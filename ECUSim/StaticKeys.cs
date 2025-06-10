using System;
using System.IO;

namespace ECUSim
{
    public static class StaticKeys
    {
        public static string ECUSim_Docs_Folder = string.Empty;

        static StaticKeys()
        {
            ECUSim_Docs_Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ECUSimulator");
            CreateDocsFolder();
        }

        private static void CreateDocsFolder()
        {
            try
            {
                if (!Directory.Exists(ECUSim_Docs_Folder)) Directory.CreateDirectory(ECUSim_Docs_Folder);
            }
            catch (IOException ioexp) { }
            catch (Exception exp) { }
        }
    }
}
