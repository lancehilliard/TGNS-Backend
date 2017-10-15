namespace TGNS
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.FolderLabel = new System.Windows.Forms.Label();
            this.ExtensionLabel = new System.Windows.Forms.Label();
            this.VideoExtTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.LearnMoreLinkLabel = new System.Windows.Forms.LinkLabel();
            this.VideoPathTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.StartKeyTestButton = new System.Windows.Forms.Button();
            this.EndKeyTestButton = new System.Windows.Forms.Button();
            this.ClearLogButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.ObsAutoStartFilenameTextBox = new System.Windows.Forms.TextBox();
            this.AutoLaunchLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ShowIconCheckbox = new System.Windows.Forms.CheckBox();
            this.CasterModeCheckBox = new System.Windows.Forms.CheckBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.AutoCloseObsCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // FolderLabel
            // 
            this.FolderLabel.AutoSize = true;
            this.FolderLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FolderLabel.Location = new System.Drawing.Point(3, 41);
            this.FolderLabel.Name = "FolderLabel";
            this.FolderLabel.Size = new System.Drawing.Size(157, 13);
            this.FolderLabel.TabIndex = 5;
            this.FolderLabel.Text = "Recording Output Folder :";
            // 
            // ExtensionLabel
            // 
            this.ExtensionLabel.AutoSize = true;
            this.ExtensionLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExtensionLabel.Location = new System.Drawing.Point(15, 70);
            this.ExtensionLabel.Name = "ExtensionLabel";
            this.ExtensionLabel.Size = new System.Drawing.Size(145, 13);
            this.ExtensionLabel.TabIndex = 9;
            this.ExtensionLabel.Text = "Output File Extension :";
            // 
            // VideoExtTextBox
            // 
            this.VideoExtTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VideoExtTextBox.Location = new System.Drawing.Point(166, 65);
            this.VideoExtTextBox.Name = "VideoExtTextBox";
            this.VideoExtTextBox.Size = new System.Drawing.Size(42, 23);
            this.VideoExtTextBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.VideoExtTextBox, "File extension of OBS-recorded videos (e.g. flv, mp4)...\r\n\r\nAllows renaming of vi" +
        "deo files to include game details.");
            this.VideoExtTextBox.TextChanged += new System.EventHandler(this.VideoExtTextBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 114);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Log :";
            // 
            // LearnMoreLinkLabel
            // 
            this.LearnMoreLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LearnMoreLinkLabel.AutoSize = true;
            this.LearnMoreLinkLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LearnMoreLinkLabel.Location = new System.Drawing.Point(708, 102);
            this.LearnMoreLinkLabel.Name = "LearnMoreLinkLabel";
            this.LearnMoreLinkLabel.Size = new System.Drawing.Size(67, 13);
            this.LearnMoreLinkLabel.TabIndex = 15;
            this.LearnMoreLinkLabel.TabStop = true;
            this.LearnMoreLinkLabel.Text = "Learn More";
            this.LearnMoreLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LearnMoreLinkLabel_LinkClicked);
            // 
            // VideoPathTextBox
            // 
            this.VideoPathTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VideoPathTextBox.Location = new System.Drawing.Point(166, 36);
            this.VideoPathTextBox.Name = "VideoPathTextBox";
            this.VideoPathTextBox.Size = new System.Drawing.Size(334, 23);
            this.VideoPathTextBox.TabIndex = 0;
            this.toolTip1.SetToolTip(this.VideoPathTextBox, "Folder where OBS outputs videos (e.g. C:\\path\\to\\recorded\\videos)...\r\n\r\nAllows re" +
        "naming of video files to include game details.");
            this.VideoPathTextBox.TextChanged += new System.EventHandler(this.VideoPathTextBox_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.LogTextBox, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 130);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(772, 246);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // LogTextBox
            // 
            this.LogTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogTextBox.Location = new System.Drawing.Point(3, 3);
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogTextBox.Size = new System.Drawing.Size(766, 240);
            this.LogTextBox.TabIndex = 13;
            this.LogTextBox.TabStop = false;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(516, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(262, 24);
            this.label6.TabIndex = 18;
            this.label6.Text = "TGNS Recording Helper";
            // 
            // StartKeyTestButton
            // 
            this.StartKeyTestButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartKeyTestButton.Location = new System.Drawing.Point(166, 94);
            this.StartKeyTestButton.Name = "StartKeyTestButton";
            this.StartKeyTestButton.Size = new System.Drawing.Size(47, 21);
            this.StartKeyTestButton.TabIndex = 2;
            this.StartKeyTestButton.Text = "Start";
            this.StartKeyTestButton.UseVisualStyleBackColor = true;
            this.StartKeyTestButton.Click += new System.EventHandler(this.StartKeyTestButton_Click);
            // 
            // EndKeyTestButton
            // 
            this.EndKeyTestButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EndKeyTestButton.Location = new System.Drawing.Point(219, 94);
            this.EndKeyTestButton.Name = "EndKeyTestButton";
            this.EndKeyTestButton.Size = new System.Drawing.Size(47, 21);
            this.EndKeyTestButton.TabIndex = 3;
            this.EndKeyTestButton.Text = "Stop";
            this.EndKeyTestButton.UseVisualStyleBackColor = true;
            this.EndKeyTestButton.Click += new System.EventHandler(this.EndKeyTestButton_Click);
            // 
            // ClearLogButton
            // 
            this.ClearLogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearLogButton.Location = new System.Drawing.Point(680, 364);
            this.ClearLogButton.Name = "ClearLogButton";
            this.ClearLogButton.Size = new System.Drawing.Size(68, 21);
            this.ClearLogButton.TabIndex = 6;
            this.ClearLogButton.Text = "Clear Log";
            this.ClearLogButton.UseVisualStyleBackColor = true;
            this.ClearLogButton.Click += new System.EventHandler(this.ClearLogButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(51, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Test Start/Stop :";
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionLabel.Location = new System.Drawing.Point(543, 27);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(235, 13);
            this.DescriptionLabel.TabIndex = 20;
            this.DescriptionLabel.Text = "Automate OBS recordings per TGNS game.";
            // 
            // ObsAutoStartFilenameTextBox
            // 
            this.ObsAutoStartFilenameTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObsAutoStartFilenameTextBox.Location = new System.Drawing.Point(166, 7);
            this.ObsAutoStartFilenameTextBox.Name = "ObsAutoStartFilenameTextBox";
            this.ObsAutoStartFilenameTextBox.Size = new System.Drawing.Size(334, 23);
            this.ObsAutoStartFilenameTextBox.TabIndex = 21;
            this.toolTip1.SetToolTip(this.ObsAutoStartFilenameTextBox, "Full path to OBS.exe (e.g. C:\\Program Files (x86)\\OBS\\OBS.exe)...\r\n\r\nThis executa" +
        "ble will automatically launch when NS2 is running.\r\n\r\nNothing happens if this is" +
        " blank or its filename doesn\'t exist.");
            this.ObsAutoStartFilenameTextBox.TextChanged += new System.EventHandler(this.ObsAutoStartFilenameTextBox_TextChanged);
            // 
            // AutoLaunchLabel
            // 
            this.AutoLaunchLabel.AutoSize = true;
            this.AutoLaunchLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AutoLaunchLabel.Location = new System.Drawing.Point(3, 12);
            this.AutoLaunchLabel.Name = "AutoLaunchLabel";
            this.AutoLaunchLabel.Size = new System.Drawing.Size(157, 13);
            this.AutoLaunchLabel.TabIndex = 22;
            this.AutoLaunchLabel.Text = "OBS AutoLaunch Filename :";
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 100;
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 20;
            this.toolTip1.ShowAlways = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // ShowIconCheckbox
            // 
            this.ShowIconCheckbox.AutoSize = true;
            this.ShowIconCheckbox.Location = new System.Drawing.Point(365, 68);
            this.ShowIconCheckbox.Name = "ShowIconCheckbox";
            this.ShowIconCheckbox.Size = new System.Drawing.Size(105, 17);
            this.ShowIconCheckbox.TabIndex = 25;
            this.ShowIconCheckbox.Text = "Scoreboard Icon";
            this.toolTip1.SetToolTip(this.ShowIconCheckbox, "Show streaming icon in your scoreboard row.");
            this.ShowIconCheckbox.UseVisualStyleBackColor = true;
            this.ShowIconCheckbox.CheckedChanged += new System.EventHandler(this.ShowIconCheckbox_CheckedChanged);
            // 
            // CasterModeCheckBox
            // 
            this.CasterModeCheckBox.AutoSize = true;
            this.CasterModeCheckBox.Location = new System.Drawing.Point(365, 91);
            this.CasterModeCheckBox.Name = "CasterModeCheckBox";
            this.CasterModeCheckBox.Size = new System.Drawing.Size(86, 17);
            this.CasterModeCheckBox.TabIndex = 26;
            this.CasterModeCheckBox.Text = "Caster Mode";
            this.toolTip1.SetToolTip(this.CasterModeCheckBox, "Enable Caster Mode in NS2+.");
            this.CasterModeCheckBox.UseVisualStyleBackColor = true;
            this.CasterModeCheckBox.CheckedChanged += new System.EventHandler(this.CasterModeCheckBox_CheckedChanged);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "TGNS Recording Helper";
            this.notifyIcon1.BalloonTipTitle = "Right-click to exit...";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "TGNS Recording Helper";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowMenuItem,
            this.ExitMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(79, 48);
            // 
            // ShowMenuItem
            // 
            this.ShowMenuItem.Name = "ShowMenuItem";
            this.ShowMenuItem.Size = new System.Drawing.Size(78, 22);
            this.ShowMenuItem.Text = "Show";
            this.ShowMenuItem.Click += new System.EventHandler(this.ShowMenuItem_Click);
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.Size = new System.Drawing.Size(78, 22);
            this.ExitMenuItem.Text = "Exit";
            this.ExitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(722, 49);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 50);
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // AutoCloseObsCheckBox
            // 
            this.AutoCloseObsCheckBox.AutoSize = true;
            this.AutoCloseObsCheckBox.Location = new System.Drawing.Point(476, 68);
            this.AutoCloseObsCheckBox.Name = "AutoCloseObsCheckBox";
            this.AutoCloseObsCheckBox.Size = new System.Drawing.Size(99, 17);
            this.AutoCloseObsCheckBox.TabIndex = 27;
            this.AutoCloseObsCheckBox.Text = "AutoClose OBS";
            this.toolTip1.SetToolTip(this.AutoCloseObsCheckBox, "Close OBS when NS2 exits");
            this.AutoCloseObsCheckBox.UseVisualStyleBackColor = true;
            this.AutoCloseObsCheckBox.CheckedChanged += new System.EventHandler(this.AutoCloseObsCheckBox_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 388);
            this.Controls.Add(this.AutoCloseObsCheckBox);
            this.Controls.Add(this.CasterModeCheckBox);
            this.Controls.Add(this.ShowIconCheckbox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ObsAutoStartFilenameTextBox);
            this.Controls.Add(this.AutoLaunchLabel);
            this.Controls.Add(this.DescriptionLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ClearLogButton);
            this.Controls.Add(this.EndKeyTestButton);
            this.Controls.Add(this.StartKeyTestButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.VideoPathTextBox);
            this.Controls.Add(this.VideoExtTextBox);
            this.Controls.Add(this.ExtensionLabel);
            this.Controls.Add(this.FolderLabel);
            this.Controls.Add(this.LearnMoreLinkLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 376);
            this.Name = "Form1";
            this.Text = "TGNS Recording Helper";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label FolderLabel;
        private System.Windows.Forms.Label ExtensionLabel;
        private System.Windows.Forms.TextBox VideoExtTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel LearnMoreLinkLabel;
        private System.Windows.Forms.TextBox VideoPathTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button StartKeyTestButton;
        private System.Windows.Forms.Button EndKeyTestButton;
        private System.Windows.Forms.Button ClearLogButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.TextBox ObsAutoStartFilenameTextBox;
        private System.Windows.Forms.Label AutoLaunchLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem ShowMenuItem;
        private System.Windows.Forms.CheckBox ShowIconCheckbox;
        private System.Windows.Forms.CheckBox CasterModeCheckBox;
        private System.Windows.Forms.CheckBox AutoCloseObsCheckBox;
    }
}

