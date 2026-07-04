using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

using WshShellLib = IWshRuntimeLibrary;

namespace BeamMPTools.Core
{
    public class PathDetector
    {
        // =====================================
        // 属性
        // =====================================

        public string BeamNGPath
        {
            get;
            private set;
        } = string.Empty;

        public string BeamMPPath
        {
            get;
            private set;
        } = string.Empty;

        public string BeamMPResourcesPath
        {
            get;
            private set;
        } = string.Empty;

        // =====================================
        // 忽略目录
        // =====================================

        private static readonly HashSet<string>
            IgnoreDirectories =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "Windows",
                "ProgramData",
                "AppData",
                "Temp",
                "$RECYCLE.BIN",
                "System Volume Information",
                "node_modules",
                ".git",
                ".vs",
                "bin",
                "obj",
                "packages",
                "cache",
                "Debug",
                "Release"
            };

        // =====================================
        // 主入口
        // =====================================

        public async Task DetectAsync(
            IProgress<int>? progress = null,
            CancellationToken token = default)
        {
            // =================================
            // BeamNG
            // =================================

            BeamNGPath =
                DetectRunningBeamNG()

                ?? DetectBeamNGFromSteam()

                ?? DetectBeamNGFromCommonPaths()

                ?? await DeepScanExecutableAsync(
                    "BeamNG.drive.exe",
                    progress,
                    token)

                ?? string.Empty;

            // =================================
            // BeamMP
            // =================================

            BeamMPPath =
                DetectRunningBeamMP()

                ?? DetectBeamMPFromRecent()

                ?? DetectBeamMPDefault()

                ?? DetectBeamMPFromCommonPaths()

                ?? await DeepScanExecutableAsync(
                    "BeamMP-Launcher.exe",
                    progress,
                    token)

                ?? string.Empty;

            // =================================
            // Resources
            // =================================

            BeamMPResourcesPath =
                DetectBeamMPResources()

                ?? string.Empty;
        }

        // =====================================
        // 正在运行的 BeamNG
        // =====================================

        private string? DetectRunningBeamNG()
        {
            return DetectExactProcess(
                "beamng.drive",
                true);
        }

        // =====================================
        // 正在运行的 BeamMP
        // =====================================

        private string? DetectRunningBeamMP()
        {
            string[] names =
            {
                "beammp-launcher",
                "beammp"
            };

            foreach (string name in names)
            {
                string? result =
                    DetectExactProcess(
                        name,
                        false);

                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            return null;
        }

        // =====================================
        // 精确进程检测
        // =====================================

        private string? DetectExactProcess(
            string processName,
            bool isBeamNG)
        {
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    string current =
                        process.ProcessName
                               .ToLower();

                    if (current != processName)
                        continue;

                    string? path =
                        Path.GetDirectoryName(
                            process.MainModule?
                                   .FileName);

                    if (string.IsNullOrEmpty(path))
                        continue;

                    bool valid =
                        isBeamNG
                        ? IsValidBeamNGPath(path)
                        : IsValidBeamMPPath(path);

                    if (valid)
                        return path;
                }
                catch
                {
                }
            }

            return null;
        }

        // =====================================
        // Steam 检测 BeamNG
        // =====================================

        private string? DetectBeamNGFromSteam()
        {
            string? steam =
                GetSteamInstallPath();

            if (string.IsNullOrEmpty(steam))
                return null;

            string vdf =
                Path.Combine(
                    steam,
                    "steamapps",
                    "libraryfolders.vdf");

            if (!File.Exists(vdf))
                return null;

            try
            {
                foreach (string line in
                    File.ReadAllLines(vdf))
                {
                    if (!line.Contains("\"path\""))
                        continue;

                    string? library =
                        ExtractVDFPath(line);

                    if (string.IsNullOrEmpty(library))
                        continue;

                    string candidate =
                        Path.Combine(
                            library,
                            "steamapps",
                            "common",
                            "BeamNG.drive");

                    if (IsValidBeamNGPath(candidate))
                        return candidate;
                }
            }
            catch
            {
            }

            return null;
        }

        // =====================================
        // Recent 检测 BeamMP
        // =====================================

        private string? DetectBeamMPFromRecent()
        {
            try
            {
                string recent =
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.ApplicationData),

                        @"Microsoft\Windows\Recent"
                    );

                if (!Directory.Exists(recent))
                    return null;

                foreach (string file in
                    Directory.GetFiles(
                        recent,
                        "*.lnk"))
                {
                    try
                    {
                        string lower =
                            Path.GetFileName(file)
                                .ToLower();

                        if (!lower.Contains("beammp"))
                            continue;

                        string? resolved =
                            ResolveShortcut(file);

                        if (string.IsNullOrEmpty(resolved))
                            continue;

                        if (IsValidBeamMPPath(resolved))
                            return resolved;
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        // =====================================
        // 默认 BeamMP
        // =====================================

        private string? DetectBeamMPDefault()
        {
            string roamingData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string[] possiblePaths =
            {
                Path.Combine(roamingData, "BeamMP-Launcher"),
                Path.Combine(roamingData, "BeamMP"),
                Path.Combine(localData, "BeamMP-Launcher"),
                Path.Combine(localData, "BeamMP")
    };

            foreach (string path in possiblePaths)
            {
                if (Directory.Exists(path) && IsValidBeamMPPath(path))
                {
                    return path;
                }
            }

            return null;
        }

        // =====================================
        // 常见路径 BeamNG
        // =====================================

        private string? DetectBeamNGFromCommonPaths()
        {
            string[] paths =
            {
                @"C:\SteamLibrary\steamapps\common\BeamNG.drive",
                @"D:\SteamLibrary\steamapps\common\BeamNG.drive",
                @"E:\SteamLibrary\steamapps\common\BeamNG.drive"
            };

            foreach (string path in paths)
            {
                if (IsValidBeamNGPath(path))
                    return path;
            }

            return null;
        }

        // =====================================
        // 常见路径 BeamMP
        // =====================================

        private string? DetectBeamMPFromCommonPaths()
        {
            string[] paths =
            {
                @"C:\BeamMP",
                @"D:\BeamMP",
                @"E:\BeamMP",

                @"C:\Games\BeamMP",
                @"D:\Games\BeamMP",

                @"C:\Program Files\BeamMP",
                @"C:\Program Files (x86)\BeamMP"
            };

            foreach (string path in paths)
            {
                if (IsValidBeamMPPath(path))
                    return path;
            }

            return null;
        }

        // =====================================
        // Resources
        // =====================================

        private string? DetectBeamMPResources()
        {
            if (string.IsNullOrWhiteSpace(
                BeamMPPath))
            {
                return null;
            }

            string[] possible =
            {
                Path.Combine(
                    BeamMPPath,
                    "Resources"),

                Path.Combine(
                    BeamMPPath,
                    "resources"),

                Path.Combine(
                    BeamMPPath,
                    "Client",
                    "Resources")
            };

            foreach (string path in possible)
            {
                if (Directory.Exists(path))
                    return path;
            }

            return null;
        }

        // =====================================
        // 深度扫描
        // =====================================

        private async Task<string?> DeepScanExecutableAsync(
            string exeName,
            IProgress<int>? progress,
            CancellationToken token)
        {
            List<string> roots =
                new List<string>();

            foreach (DriveInfo drive in
                DriveInfo.GetDrives())
            {
                try
                {
                    if (!drive.IsReady)
                        continue;

                    if (drive.DriveType !=
                        DriveType.Fixed)
                    {
                        continue;
                    }

                    roots.Add(
                        drive.RootDirectory
                             .FullName);
                }
                catch
                {
                }
            }

            int total = roots.Count;
            int current = 0;

            try
            {
                return await Task.Run(() =>
                {
                    string? result = null;

                    Parallel.ForEach(
                        roots,

                        new ParallelOptions
                        {
                            CancellationToken = token,

                            MaxDegreeOfParallelism =
                                Math.Min(
                                    4,
                                    Environment
                                    .ProcessorCount)
                        },

                        (root, state) =>
                        {
                            if (result != null)
                            {
                                state.Stop();
                                return;
                            }

                            try
                            {
                                string? found =
                                    RecursiveSearch(
                                        root,
                                        exeName,
                                        token);

                                if (found != null)
                                {
                                    bool valid =
                                        exeName
                                        .Contains("BeamNG")

                                        ? IsValidBeamNGPath(found)

                                        : IsValidBeamMPPath(found);

                                    if (valid)
                                    {
                                        result = found;
                                        state.Stop();
                                    }
                                }
                            }
                            catch
                            {
                            }

                            int percent =
                                Interlocked.Increment(
                                    ref current);

                            progress?.Report(
                                (percent * 100) / total);
                        });

                    return result;
                });
            }
            catch
            {
                return null;
            }
        }

        // =====================================
        // 递归搜索
        // =====================================

        private string? RecursiveSearch(
            string directory,
            string exeName,
            CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested)
                    return null;

                string folder =
                    Path.GetFileName(directory);

                if (IgnoreDirectories.Contains(folder))
                    return null;

                // 排除自己的工具目录

                string lower =
                    directory.ToLower();

                if (lower.Contains("starter"))
                    return null;

                if (lower.Contains("launcherstarter"))
                    return null;

                if (lower.Contains("beammptools"))
                    return null;

                string candidate =
                    Path.Combine(
                        directory,
                        exeName);

                if (File.Exists(candidate))
                    return directory;

                foreach (string subDir in
                    Directory.EnumerateDirectories(
                        directory))
                {
                    string? result =
                        RecursiveSearch(
                            subDir,
                            exeName,
                            token);

                    if (result != null)
                        return result;
                }
            }
            catch
            {
            }

            return null;
        }

        // =====================================
        // 快捷方式解析
        // =====================================

        private string? ResolveShortcut(
            string shortcutPath)
        {
            try
            {
                WshShellLib.WshShell shell =
                    new WshShellLib.WshShell();

                WshShellLib.IWshShortcut shortcut =
                    (WshShellLib.IWshShortcut)
                    shell.CreateShortcut(
                        shortcutPath);

                string target =
                    shortcut.TargetPath;

                if (!File.Exists(target))
                    return null;

                return Path.GetDirectoryName(
                    target);
            }
            catch
            {
                return null;
            }
        }

        // =====================================
        // Steam Path
        // =====================================

        private string? GetSteamInstallPath()
        {
            string[] regs =
            {
                @"SOFTWARE\WOW6432Node\Valve\Steam",
                @"SOFTWARE\Valve\Steam"
            };

            foreach (string reg in regs)
            {
                try
                {
                    using RegistryKey? key =
                        Registry.LocalMachine
                                .OpenSubKey(reg);

                    if (key == null)
                        continue;

                    string? value =
                        key.GetValue(
                            "InstallPath")
                            as string;

                    if (!string.IsNullOrWhiteSpace(
                        value))
                    {
                        return value;
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        // =====================================
        // 提取 VDF Path
        // =====================================

        private string? ExtractVDFPath(
            string line)
        {
            try
            {
                int first =
                    line.IndexOf(
                        '"',
                        line.IndexOf(
                            "\"path\"") + 6);

                int last =
                    line.LastIndexOf('"');

                if (first < 0 ||
                    last <= first)
                {
                    return null;
                }

                return line
                    .Substring(
                        first + 1,
                        last - first - 1)
                    .Replace(@"\\", @"\");
            }
            catch
            {
                return null;
            }
        }

        // =====================================
        // BeamNG 校验
        // =====================================

        private bool IsValidBeamNGPath(
            string path)
        {
            try
            {
                return File.Exists(
                    Path.Combine(
                        path,
                        "BeamNG.drive.exe"));
            }
            catch
            {
                return false;
            }
        }

        // =====================================
        // BeamMP 校验
        // =====================================

        private bool IsValidBeamMPPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                string lower = path.ToLower();

                // 排除自身项目目录
                if (lower.Contains("starter") ||
                    lower.Contains("launcherstarter") ||
                    lower.Contains("debug") ||
                    lower.Contains("release"))
                {
                    return false;
                }

                // 只要存在任何一个核心可执行文件即可
                bool hasLauncher = File.Exists(Path.Combine(path, "BeamMP-Launcher.exe"));
                bool hasBeamMPExe = File.Exists(Path.Combine(path, "BeamMP.exe"));

                // 移除对 Resources 和 CEF 的强制依赖
                return hasLauncher || hasBeamMPExe;
            }
            catch
            {
                return false;
            }
        }

        // =====================================
        // 状态
        // =====================================

        public bool IsGameFound()
        {
            return !string.IsNullOrWhiteSpace(
                BeamNGPath);
        }

        public bool IsBeamMPFound()
        {
            return !string.IsNullOrWhiteSpace(
                BeamMPPath);
        }

        public bool IsBeamMPResourcesFound()
        {
            return !string.IsNullOrWhiteSpace(
                BeamMPResourcesPath);
        }
    }
}
