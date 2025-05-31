using System.IO;
using System.Reflection;
using WPFLibBase;

namespace WPFHostLib
{
    public class WpfViewFactory
    {
        public static WpfUserControl? GetWpfViewInstance(string assemblyName)
        {
            if (!string.IsNullOrWhiteSpace(assemblyName))
            {
                var className = string.Empty;
                foreach (var viewKeyValuePair in WpfViewKeys.ViewsDictionary)
                {
                    if (viewKeyValuePair.Key.Equals(assemblyName))
                    {
                        className = viewKeyValuePair.Value;
                    }
                }
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrWhiteSpace(assemblyLocation))
                {
                    var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                    if (!string.IsNullOrWhiteSpace(assemblyDirectory))
                    {
                        var assemblyFullPath = Path.Combine(assemblyDirectory, assemblyName + ".dll");
                        var wpfModuleAssembly = Assembly.LoadFrom(assemblyFullPath);
                        var type = wpfModuleAssembly.GetType(className);
                        if (null != type)
                        {
                            return Activator.CreateInstance(type) as WpfUserControl;
                        }
                    }
                }
            }
            return null;
        }
    }
}
