namespace UtilityLib
{
    public class PathValidator
    {
        public static bool IsValidFilePath(string filePath, string requiredExtension, ref string message)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                message = "Can't be empty";
                return false;
            }

            if (!filePath.EndsWith(requiredExtension, StringComparison.OrdinalIgnoreCase))
            {
                message = $"File must have {requiredExtension} extension.";
                return false;
            }

            string? fileDir = Path.GetDirectoryName(filePath);
            string? fileName = Path.GetFileName(filePath);
            string? fileExt = Path.GetExtension(filePath);

            if (string.IsNullOrWhiteSpace(fileDir) || string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileExt))
            {
                message = "Not a valid full path";
                return false;
            }

            if ((!string.IsNullOrWhiteSpace(fileDir)) && (!string.IsNullOrWhiteSpace(fileName)) && (!string.IsNullOrWhiteSpace(fileExt)))
            {
                //it's a full file path
                if (!Directory.Exists(fileDir))
                {
                    message = "File directory doesn't exist.";
                    return false;
                }
            }
            return true;
        }
    }
}
