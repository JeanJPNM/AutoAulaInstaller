using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Octokit;
using System.IO;
using IWshRuntimeLibrary;
using static System.Environment;
namespace AutoAulaInstaller
{
    class Program
    {
        public static string InstallationPath
        {
            get
            {
                var folderPath = GetFolderPath(SpecialFolder.UserProfile);
                return Path.Join(folderPath, "appdata/local/programs/auto_aula");
            }
        }
        static async Task Main(string[] args)
        {
            var client = new GitHubClient(new ProductHeaderValue("JeanJPNM"));
            var release = await client.Repository.Release.GetLatest("JeanJPNM", "auto_aula.dart");
            var zipAsset = release.ProgramZip();
            if (zipAsset == null)
            {
                Console.WriteLine("O arquivo zip não foi encontrado.");
                Console.WriteLine("Tente novamente mais tarde");
                return;
            }
            Console.WriteLine("Baixando release...");
            var filePath = await zipAsset.Download();
            ZipFile.ExtractToDirectory(filePath, InstallationPath, true);
            var executablePath = Path.Join(InstallationPath, "auto_aula.exe");
            var shell = new WshShell();
            IWshShortcut shortcut =(IWshShortcut) shell.CreateShortcut(Path.Join(GetFolderPath(SpecialFolder.StartMenu), "Auto Aula.lnk"));
            shortcut.TargetPath = executablePath.Replace('/', '\\');
            shortcut.Save();
        }
    }
}
