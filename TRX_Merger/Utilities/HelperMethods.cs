using System.IO;
using System.Linq;
using System.Reflection;

namespace TRX_Merger.Utilities
{
    public static class HelperMethods
    {
        public static string GetEmbeddedResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(name));

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    return null;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
