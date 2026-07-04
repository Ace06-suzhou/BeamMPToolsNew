using System;

namespace BeamMPTools.UI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnDetect;
        private System.Windows.Forms.Button btnInstallBeamMP;
        private System.Windows.Forms.Button btnOpenModsFolder; 
        private System.Windows.Forms.Button btnOpenWebsite; 
        private System.Windows.Forms.Button btnRepair;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnLaunchBeamMP;
        private System.Windows.Forms.Button btnLaunchBeamNG;



        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnDetect = new System.Windows.Forms.Button();
            this.btnInstallBeamMP = new System.Windows.Forms.Button();
            this.btnOpenModsFolder = new System.Windows.Forms.Button();
            this.btnOpenWebsite = new System.Windows.Forms.Button();
            this.btnRepair = new System.Windows.Forms.Button();
            this.btnLaunchBeamMP = new System.Windows.Forms.Button();
            this.btnLaunchBeamNG = new System.Windows.Forms.Button();


            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();

            System.Drawing.Color bgColor = System.Drawing.Color.FromArgb(30, 30, 30);
            System.Drawing.Color btnColor = System.Drawing.Color.FromArgb(45, 45, 48);
            System.Drawing.Color accentColor = System.Drawing.Color.FromArgb(255, 102, 0);
            System.Drawing.Color textColor = System.Drawing.Color.White;

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = accentColor;
            this.lblTitle.Location = new System.Drawing.Point(12, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(166, 30);
            this.lblTitle.TabIndex = 7;
            this.lblTitle.Text = "BeamMP Tools";

            void SetButtonStyle(System.Windows.Forms.Button btn, string text, int yPos)
            {
                btn.BackColor = btnColor;
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = accentColor;
                btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                btn.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                btn.ForeColor = textColor;
                btn.Location = new System.Drawing.Point(17, yPos);
                btn.Size = new System.Drawing.Size(180, 40);
                btn.Text = text;
                btn.Cursor = System.Windows.Forms.Cursors.Hand;
            }

            SetButtonStyle(this.btnDetect, "Detect Paths (检测路径)", 60);
            SetButtonStyle(this.btnInstallBeamMP, "Install BeamMP (安装客户端)", 110);
            SetButtonStyle(this.btnOpenModsFolder, "Open Mods Folder (打开模组文件夹)", 160);
            SetButtonStyle(this.btnOpenWebsite, "Open Docs Website (打开帮助文档)", 210);
            SetButtonStyle(this.btnRepair, "Repair (验证完整性)", 260);
            SetButtonStyle(this.btnLaunchBeamMP, "Launch BeamMP (启动联机)", 310);
            SetButtonStyle(this.btnLaunchBeamNG, "Launch BeamNG (启动游戏)", 360);


            // 绑定事件
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            this.btnInstallBeamMP.Click += new System.EventHandler(this.btnInstallBeamMP_Click);
            this.btnOpenModsFolder.Click += new System.EventHandler(this.btnOpenModsFolder_Click);
            this.btnOpenWebsite.Click += new System.EventHandler(this.btnOpenWebsite_Click);
            this.btnRepair.Click += new System.EventHandler(this.btnRepair_Click);
            this.btnLaunchBeamMP.Click += new System.EventHandler(this.btnLaunchBeamMP_Click);
            this.btnLaunchBeamNG.Click += new System.EventHandler(this.btnLaunchBeamNG_Click);



            this.txtLog.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtLog.ForeColor = System.Drawing.Color.LightGray;
            this.txtLog.Location = new System.Drawing.Point(215, 60);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(430, 350);
            this.txtLog.TabIndex = 6;
            this.txtLog.Text = "";

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = bgColor;
            this.ClientSize = new System.Drawing.Size(660, 430);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnRepair);
            this.Controls.Add(this.btnOpenWebsite);
            this.Controls.Add(this.btnOpenModsFolder);
            this.Controls.Add(this.btnInstallBeamMP);
            this.Controls.Add(this.btnDetect);
            this.Controls.Add(this.btnLaunchBeamMP);
            this.Controls.Add(this.btnLaunchBeamNG);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BeamMP Tools By Ace06";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}