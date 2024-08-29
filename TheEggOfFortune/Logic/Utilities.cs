using Microsoft.Maui.Storage;
using System.IO;
using System.Threading.Tasks;

namespace TheEggOfFortune.Logic
{
    internal static class Utilities
    {
        internal static async Task<string> LoadTextfileAsync(string filename)
        {
            using (Stream stream = await FileSystem.OpenAppPackageFileAsync(filename))
            {
                using (StreamReader reader = new(stream))
                {
                    return (await reader.ReadToEndAsync()).Replace("\r\n", "\n").Replace("\r", "").Trim();
                }
            }
        }
    }
}
