using System;
using System.IO;

namespace ECUSim
{
    public static class StaticKeys
    {
        public static string ECUSim_Docs_Folder = string.Empty;

        public static string Communication_Settings_File = string.Empty;

        public static string Messages_Config_File = string.Empty;

        static StaticKeys()
        {
            ECUSim_Docs_Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "PANKAJ.CO", "ECUSimulator");
            Communication_Settings_File = Path.Combine(ECUSim_Docs_Folder, "comSettings.json");
            Messages_Config_File = Path.Combine(ECUSim_Docs_Folder, "messagesConfig.json");
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
