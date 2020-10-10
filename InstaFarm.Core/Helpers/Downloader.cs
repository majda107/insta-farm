using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace InstaFarm.Core.Helpers
{
    public class Downloader
    {
        private static HttpClient client = new HttpClient();

        public static async Task DownloadFile(string url, string filename)
        {
            var res = await Downloader.client.GetAsync(url);
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                await res.Content.CopyToAsync(fs);
            }
        }
    }
}