using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Linq;


using BeamMPTools.Core;
using BeamMPTools.Utils;
using BeamMPTools.Installer;

namespace BeamMPTools.UI
{
    public partial class Form1 : Form
    {
        // =========================
        // 字段
        // =========================

        private readonly PathDetector _detector;

        private readonly ClientInstaller _clientInstaller;

        // =========================
        // 构造函数
        // =========================

        public Form1()
        {
            InitializeComponent();

            Load += Form1_Load;

            _detector = new PathDetector();

            _clientInstaller = new ClientInstaller();
        }


        // =========================
        // Form Load
        // =========================

        private async void Form1_Load(
            object sender,
            EventArgs e)
        {
            Logger.OutputBox = txtLog;

            Logger.Info("BeamMP Tools 启动成功...");
            Logger.Info("本软件使用 GPL 3.0 许可证");

            Logger.Info("正在初始化环境检测...");

            await SafeDetectAsync();
        }

        // =========================
        // 安全检测包装
        // =========================

        private async Task SafeDetectAsync()
        {
            try
            {
                await _detector.DetectAsync();

                PrintDetectionResult();
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"检测过程中发生错误: {ex.Message}");
            }
        }

        // =========================
        // 输出检测结果
        // =========================

        private void PrintDetectionResult()
        {
            // BeamNG

            if (_detector.IsGameFound())
            {
                Logger.Info(
                    $"找到 BeamNG.drive:\n{_detector.BeamNGPath}");
            }
            else
            {
                Logger.Error(
                    "未找到 BeamNG.drive 安装目录。");
            }

            // BeamMP

            if (_detector.IsBeamMPFound())
            {
                Logger.Info(
                    $"找到 BeamMP:\n{_detector.BeamMPPath}");
            }
            else
            {
                Logger.Error(
                    "未找到 BeamMP 安装目录。");
            }

            // Resources

            if (_detector.IsBeamMPResourcesFound())
            {
                Logger.Info(
                    $"找到 BeamMP Resources:\n{_detector.BeamMPResourcesPath}");
            }
            else
            {
                Logger.Info(
                    "未检测到 BeamMP Resources 文件夹。\n" +
                    "可能尚未联机下载服务器资源，也可能是小工具本身问题，记得反馈给我。");
            }
        }

        // =========================
        // 手动检测
        // =========================

        private async void btnDetect_Click(
            object sender,
            EventArgs e)
        {
            btnDetect.Enabled = false;

            Logger.Info("开始重新检测本地环境...");

            await SafeDetectAsync();

            btnDetect.Enabled = true;
        }

        // =========================
        // 安装 BeamMP
        // =========================

        private async void btnInstallBeamMP_Click(
            object sender,
            EventArgs e)
        {
            btnInstallBeamMP.Enabled = false;

            try
            {
                await _detector.DetectAsync();

                // 已安装检测

                if (_detector.IsBeamMPFound())
                {
                    DialogResult result =
                        MessageBox.Show(
                            "检测到已安装 BeamMP。\n\n" +
                            "继续安装将覆盖当前客户端。\n\n" +
                            "是否继续？",

                            "BeamMP 已安装",

                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                    if (result == DialogResult.No)
                    {
                        Logger.Info(
                            "用户取消了安装操作。");

                        return;
                    }

                    Logger.Info(
                        "用户确认继续安装。");
                }
                else
                {
                    Logger.Info(
                        "未检测到 BeamMP，准备全新安装...");
                }

                // 开始安装

                Logger.Info(
                    "开始安装 BeamMP 客户端...");

                await _clientInstaller.InstallAsync();

                Logger.Info(
                    "BeamMP 安装完成。");

                // 重新检测

                await SafeDetectAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"安装失败: {ex.Message}");
            }
            finally
            {
                btnInstallBeamMP.Enabled = true;
            }
        }

        // =========================
        // 打开 Mods / Resources
        // =========================

        private async void btnOpenModsFolder_Click(
            object sender,
            EventArgs e)
        {
            btnOpenModsFolder.Enabled = false;

            try
            {
                await _detector.DetectAsync();

                if (!_detector.IsBeamMPFound())
                {
                    Logger.Error(
                        "未检测到 BeamMP 安装目录。");

                    return;
                }

                if (!_detector.IsBeamMPResourcesFound())
                {
                    Logger.Error(
                        "未检测到 BeamMP Resources 文件夹。\n" +
                        "可能尚未联机或未下载服务器资源。");

                    return;
                }

                string path =
                    _detector.BeamMPResourcesPath;

                Logger.Info(
                    $"正在打开目录:\n{path}");

                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"打开目录失败: {ex.Message}");
            }
            finally
            {
                btnOpenModsFolder.Enabled = true;
            }
        }

        // =========================
        // 打开 BeamMP Docs
        // =========================

        private void btnOpenWebsite_Click(
            object sender,
            EventArgs e)
        {
            const string url =
                "https://docs.beammp.com/zh/";

            Logger.Info(
                $"正在打开网站:\n{url}");

            try
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"打开网站失败: {ex.Message}");
            }
        }

        // =========================
        // Steam 文件校验
        // =========================

        private void btnRepair_Click(
            object sender,
            EventArgs e)
        {
            Logger.Info(
                "正在唤起 Steam 文件完整性验证...");

            try
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            "steam://validate/284160",

                        UseShellExecute = true
                    });

                Logger.Info(
                    "Steam 验证命令已发送。");
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"无法启动 Steam:\n{ex.Message}");
            }
        }

        private async void btnLaunchBeamMP_Click(
            object sender,
            EventArgs e)
          {
            btnLaunchBeamMP.Enabled = false;

            try
            {
                await _detector.DetectAsync();

                if (!_detector.IsBeamMPFound())
                {
                    Logger.Error(
                        "未检测到 BeamMP。");

                    return;
                }

                // 防止重复启动

                string[] beamMpProcesses =
                {
            "beammp-launcher",
            "beammp"
        };

                bool alreadyRunning =
                    Process.GetProcesses()
                        .Any(p =>
                        {
                            try
                            {
                                string processName =
                                    p.ProcessName
                                     .ToLower();

                                // 精确匹配进程名

                                if (!beamMpProcesses
                                    .Contains(processName))
                                {
                                    return false;
                                }

                                // 验证真实路径

                                string? processPath =
                                    p.MainModule?
                                     .FileName;

                                if (string.IsNullOrWhiteSpace(processPath))
                                    return false;

                                return processPath
                                    .ToLower()
                                    .Contains("beammp");
                            }
                            catch
                            {
                                return false;
                            }
                        });



                if (alreadyRunning)
                {
                    Logger.Info(
                        "BeamMP 已经在运行。");

                    return;
                }

                // 查找 EXE

                string[] possibleExe =
                {
            Path.Combine(
                _detector.BeamMPPath,
                "BeamMP-Launcher.exe"),

            Path.Combine(
                _detector.BeamMPPath,
                "BeamMP.exe")
        };

                string? exe =
                    possibleExe.FirstOrDefault(
                        File.Exists);

                if (exe == null)
                {
                    Logger.Error(
                        "未找到 BeamMP 启动程序。");

                    return;
                }

                Logger.Info(
                    $"正在启动 BeamMP:\n{exe}");

                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = exe,

                        WorkingDirectory =
                            Path.GetDirectoryName(exe),

                        UseShellExecute = true
                    });
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"启动 BeamMP 失败:\n{ex.Message}");
            }
            finally
            {
                btnLaunchBeamMP.Enabled = true;
            }
        }




        private async void btnLaunchBeamNG_Click(
            object sender,
            EventArgs e)
                {
                    btnLaunchBeamNG.Enabled = false;

            try
            {
                await _detector.DetectAsync();

                if (!_detector.IsGameFound())
                {
                    Logger.Error(
                        "未检测到 BeamNG.drive。");

                    return;
                }

                string exe =
                    Path.Combine(
                        _detector.BeamNGPath,
                        "BeamNG.drive.exe");

                if (!File.Exists(exe))
                {
                    Logger.Error(
                        "BeamNG.drive.exe 不存在。");

                    return;
                }

                Logger.Info(
                    $"正在启动 BeamNG.drive:\n{exe}");

                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = exe,

                        WorkingDirectory =
                            _detector.BeamNGPath,

                        UseShellExecute = true
                    });
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"启动 BeamNG 失败:\n{ex.Message}");
            }
            finally
            {
                btnLaunchBeamNG.Enabled = true;
            }
        }



            }
}
    

