using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Octokit;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Timers;
namespace AutoAulaInstaller
{
    static class Extensions
    {

        public static ReleaseAsset? ProgramZip(this Release release)
        {
            var pattern = new Regex(@"release\.zip", RegexOptions.IgnoreCase);
            var assets = new List<ReleaseAsset>(release.Assets);
            return assets.Find((asset) => pattern.IsMatch(asset.Name));
        }
        /// <summary>
        ///   Downloads the asset from github
        /// </summary>
        /// <param name="releaseAsset"></param>
        /// <returns>The path to the downloaded file</returns>
        public static async Task<string> Download(this ReleaseAsset releaseAsset)
        {
            using var webClient = new WebClient();
            using var timer = new Timer(1000);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) =>
            {
                const string label = "Baixando Asset:";
                var percentage = $"{e.ProgressPercentage}%";
                Console.Write($"\r{label} {releaseAsset.Name} ({percentage})");
            });
            var data = await webClient.DownloadDataTaskAsync(new Uri(releaseAsset.BrowserDownloadUrl));
            var filePath = Path.Join(Program.InstallationPath, releaseAsset.Name);
            await File.WriteAllBytesAsync(filePath, data);
            Console.WriteLine();
            return filePath;
        }
    }
}