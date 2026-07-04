BeamMPTools By Ace06

面向全体简中玩家与服务器主的BeamMP工具箱

作者B站：https://space.bilibili.com/317415750

本项目基于VS2022 IDE与.NET 8的C# Winform构建


支持：

- 打开BeamMP
- 打开BeamNG
- BeamNG.drive 路径检测
- BeamMP 安装检测
- BeamMP Resources 自动识别
- Steam 游戏完整性验证
- BeamMP 客户端安装，同时如果官方库无法下载也提供了国内镜像
- 多路径智能扫描
- Recent / Steam / Process 多重检测
- 深度磁盘扫描
- 异步检测与日志系统

---

# 功能特性

## 智能路径检测

相比传统只检测固定目录的方法：

BeamMPTools 使用了：

- Steam Library 检测
- Windows Recent 快捷方式反查
- 正在运行的进程检测
- 多驱动器递归扫描
- BeamMP 专项目录结构验证

能够识别：

- 非默认 Steam 库
- 自定义 BeamMP 安装位置
- 多磁盘安装
- 便携式 BeamMP
- 特殊路径安装

---

## BeamMP Resources 自动识别

自动定位：

BeamMP/Resources

用于：

* 模组管理/安装
* 服务器模组资源查看
* 快速打开资源目录

---

## 异步深度扫描

采用：

* async / await
* Parallel.ForEach
* CancellationToken

实现：

* 非阻塞 UI
* 多线程扫描
* 更快的大容量磁盘搜索

---

## 日志系统

内置线程安全日志系统：

* INFO
* ERROR
* UI Invoke 安全更新

方便后续：

* Debug
* 用户反馈
* 错误定位

---

# 截图

<img width="662" height="519" alt="314742dab34f6a8c61ae931c73de63dc" src="https://github.com/user-attachments/assets/fa9ce39d-d0ec-4c00-87f5-f7f8322351f7" />




---

# 系统要求

* Windows 10 / 11
* .NET 8 或更高版本
* BeamNG.drive
* BeamMP（可选）

---

# 使用方法

## 1. 下载
如果您不想安装.NET 8，请使用BeamMPToolsFull.exe

从 Releases 页面下载：


BeamMPTools.exe or BeamMPToolsFull.exe


---

## 2. 启动

直接运行：


BeamMPTools.exe or BeamMPToolsFull.exe


---

## 3. 检测环境

点击：


检测


程序会自动：

* 检测 BeamNG.drive
* 检测 BeamMP
* 检测 Resources

---

# 技术实现

## 核心技术

* C#
* WinForms
* .NET
* Async/Await
* Parallel.ForEach
* Registry
* Process Detection
* Windows Shortcut Parsing

---

## 检测逻辑

### BeamNG.drive

优先级：

1. 正在运行的进程
2. Steam Registry
3. Steam LibraryFolders.vdf
4. 常见目录
5. 深度扫描

---

### BeamMP

优先级：

1. 正在运行的 BeamMP
2. Windows Recent
3. 默认 AppData
4. 常见目录
5. 深度扫描

---

# 开源协议

本项目基于：


GPL-3.0 License



---

# 注意事项

本工具：

* 不会修改 BeamNG.drive 游戏文件
* 不会上传用户数据
* 不包含任何作弊功能
* 不会注入游戏进程

---

# 已知问题

* 极少数特殊安装环境可能无法自动识别
* 某些精简系统可能缺少 Recent 快捷方式
* OneDrive 同步目录可能导致路径异常



---

# 致谢

感谢：

* 所有测试用户

---

# Star

如果这个项目帮到了你，欢迎给个 Star ⭐，爱你
