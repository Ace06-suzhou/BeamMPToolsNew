using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BeamMPTools.Utils;

namespace BeamMPTools.Installer
{
    public class ClientInstaller
    {
        // 双轨下载路线
        private readonly string[] DownloadUrls = new string[]
        {
            // 路线 1：杂草的只找到了一个。。。
            "https://ghproxy.net/https://github.com/BeamMP/BeamMP-Launcher/releases/download/v2.8.0/BeamMP-Launcher.exe",
            
            // 路线 2：官方原地址
            "https://github.com/BeamMP/BeamMP-Launcher/releases/download/v2.8.0/BeamMP-Launcher.exe"
        };

        public async Task InstallAsync()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string targetDir = Path.Combine(appData, "BeamMP-Launcher");
            string targetExe = Path.Combine(targetDir, "BeamMP-Launcher.exe");

            try
            {
                Logger.Info("准备下载 BeamMP 客户端...");

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                    Logger.Info($"已创建目录: {targetDir}");
                }

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "BeamMPTools-Client");
                client.Timeout = TimeSpan.FromSeconds(30); // 考虑到要下载大文件，超时放宽到30秒

                bool downloadSuccess = false;

                foreach (string url in DownloadUrls)
                {
                    try
                    {
                        Logger.Info($"正在尝试连接节点: {new Uri(url).Host} ...");

                        // HttpCompletionOption.ResponseHeadersRead
                        using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        response.EnsureSuccessStatusCode();

                        // 获取文件总大小
                        long? totalBytes = response.Content.Headers.ContentLength;

                        // 核心校验：如果拿到的体积小于 1MB，直接判定是镜像站报错的 HTML 网页，拒绝下载
                        if (totalBytes.HasValue && totalBytes.Value < 1024 * 1024)
                        {
                            Logger.Error("检测到该节点返回的文件体积异常，已拦截，正在切换下一节点...");
                            continue;
                        }

                        // 开始流式读取与写入
                        using Stream contentStream = await response.Content.ReadAsStreamAsync();
                        using FileStream fileStream = new FileStream(targetExe, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                        byte[] buffer = new byte[8192];
                        long totalReadBytes = 0;
                        int readBytes;
                        int lastPercent = -1;

                        while ((readBytes = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, readBytes);
                            totalReadBytes += readBytes;

                            // 计算进度并打印进度条
                            if (totalBytes.HasValue && totalBytes.Value > 0)
                            {
                                int percent = (int)((totalReadBytes * 100) / totalBytes.Value);

                                // 限制每跳变 5% 才打印一次日志，防止把日志文件刷爆
                                if (percent != lastPercent && percent % 5 == 0)
                                {
                                    lastPercent = percent;
                                    string progressBar = DrawProgressBar(percent);
                                    Logger.Info($"下载进度: {progressBar} {percent}%");
                                }
                            }
                        }

                        downloadSuccess = true;
                        Logger.Info("文件下载并写入完成！");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"该节点下载中断或失败: {ex.Message}");
                        //清理不完整的残余文件
                        if (File.Exists(targetExe)) try { File.Delete(targetExe); } catch { }
                    }
                }

                if (downloadSuccess)
                {
                    Logger.Info("正在唤起 BeamMP 客户端进行初始环境配置...");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = targetExe,
                        UseShellExecute = true,
                        WorkingDirectory = targetDir
                    });

                    Logger.Info("客户端安装与部署全部完成！");
                }
                else
                {
                    Logger.Error("所有下载节点均不可用，安装中止。请检查网络状况。");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"安装过程中发生本地错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 绘制字符进度条的方法
        /// </summary>
        private string DrawProgressBar(int percent)
        {
            int barLength = 20; // 进度条总格数
            int filledLength = percent * barLength / 100;
            return "[" + new string('#', filledLength) + new string('-', barLength - filledLength) + "]";
        }
    }
}