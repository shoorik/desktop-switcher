namespace DesktopSwitcher
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
            this.dirtb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dirbutton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dualmon = new System.Windows.Forms.ToolStripMenuItem();
            this.autostart = new System.Windows.Forms.ToolStripMenuItem();
            this.winstart = new System.Windows.Forms.ToolStripMenuItem();
            this.startmintool = new System.Windows.Forms.ToolStripMenuItem();
            this.subdirs = new System.Windows.Forms.ToolStripMenuItem();
            this.statenable = new System.Windows.Forms.ToolStripMenuItem();
            this.showtips = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.diagnosticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenslist = new System.Windows.Forms.ToolStripMenuItem();
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentPicturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changebutton = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.browsedialog = new System.Windows.Forms.FolderBrowserDialog();
            this.gobutton = new System.Windows.Forms.Button();
            this.timernum = new System.Windows.Forms.NumericUpDown();
            this.choose = new System.Windows.Forms.Button();
            this.getpicdialog = new System.Windows.Forms.OpenFileDialog();
            this.trayicon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.traychange = new System.Windows.Forms.ToolStripMenuItem();
            this.currentPicturesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exit = new System.Windows.Forms.ToolStripMenuItem();
            this.ratiobox = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.denombox = new System.Windows.Forms.ComboBox();
            this.usebox = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timernum)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ratiobox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usebox)).BeginInit();
            this.SuspendLayout();
            // 
            // dirtb
            // 
            this.dirtb.Location = new System.Drawing.Point(70, 32);
            this.dirtb.Name = "dirtb";
            this.dirtb.Size = new System.Drawing.Size(295, 20);
            this.dirtb.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Directory:";
            // 
            // dirbutton
            // 
            this.dirbutton.Location = new System.Drawing.Point(376, 30);
            this.dirbutton.Name = "dirbutton";
            this.dirbutton.Size = new System.Drawing.Size(28, 23);
            this.dirbutton.TabIndex = 4;
            this.dirbutton.Text = "...";
            this.dirbutton.UseVisualStyleBackColor = true;
            this.dirbutton.Click += new System.EventHandler(this.dirbutton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Interval:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.screenslist,
            this.customizeToolStripMenuItem,
            this.currentPicturesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(409, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dualmon,
            this.autostart,
            this.winstart,
            this.startmintool,
            this.subdirs,
            this.statenable,
            this.showtips,
            this.toolStripSeparator1,
            this.diagnosticToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // dualmon
            // 
            this.dualmon.Checked = true;
            this.dualmon.CheckOnClick = true;
            this.dualmon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dualmon.Name = "dualmon";
            this.dualmon.Size = new System.Drawing.Size(191, 22);
            this.dualmon.Text = "Dual Monitor";
            this.dualmon.Click += new System.EventHandler(this.dualmon_Click);
            // 
            // autostart
            // 
            this.autostart.CheckOnClick = true;
            this.autostart.Name = "autostart";
            this.autostart.Size = new System.Drawing.Size(191, 22);
            this.autostart.Text = "Start Automatically";
            // 
            // winstart
            // 
            this.winstart.Name = "winstart";
            this.winstart.Size = new System.Drawing.Size(191, 22);
            this.winstart.Text = "Start with Windows";
            this.winstart.Click += new System.EventHandler(this.winstart_Click);
            // 
            // startmintool
            // 
            this.startmintool.CheckOnClick = true;
            this.startmintool.Name = "startmintool";
            this.startmintool.Size = new System.Drawing.Size(191, 22);
            this.startmintool.Text = "Start Minimized";
            // 
            // subdirs
            // 
            this.subdirs.CheckOnClick = true;
            this.subdirs.Name = "subdirs";
            this.subdirs.Size = new System.Drawing.Size(191, 22);
            this.subdirs.Text = "Include Subdirectories";
            // 
            // statenable
            // 
            this.statenable.CheckOnClick = true;
            this.statenable.Name = "statenable";
            this.statenable.Size = new System.Drawing.Size(191, 22);
            this.statenable.Text = "Enable Stats";
            // 
            // showtips
            // 
            this.showtips.CheckOnClick = true;
            this.showtips.Name = "showtips";
            this.showtips.Size = new System.Drawing.Size(191, 22);
            this.showtips.Text = "Show Balloon Tips";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(188, 6);
            // 
            // diagnosticToolStripMenuItem
            // 
            this.diagnosticToolStripMenuItem.Name = "diagnosticToolStripMenuItem";
            this.diagnosticToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.diagnosticToolStripMenuItem.Text = "Screen Diagnostic";
            this.diagnosticToolStripMenuItem.Click += new System.EventHandler(this.diagnosticToolStripMenuItem_Click);
            // 
            // screenslist
            // 
            this.screenslist.Name = "screenslist";
            this.screenslist.Size = new System.Drawing.Size(57, 20);
            this.screenslist.Text = "Screens";
            // 
            // customizeToolStripMenuItem
            // 
            this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            this.customizeToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.customizeToolStripMenuItem.Text = "Customize";
            this.customizeToolStripMenuItem.Click += new System.EventHandler(this.customizeToolStripMenuItem_Click);
            // 
            // currentPicturesToolStripMenuItem
            // 
            this.currentPicturesToolStripMenuItem.Name = "currentPicturesToolStripMenuItem";
            this.currentPicturesToolStripMenuItem.Size = new System.Drawing.Size(97, 20);
            this.currentPicturesToolStripMenuItem.Text = "Current Pictures";
            this.currentPicturesToolStripMenuItem.Click += new System.EventHandler(this.currentPicturesToolStripMenuItem_Click);
            // 
            // changebutton
            // 
            this.changebutton.Location = new System.Drawing.Point(329, 62);
            this.changebutton.Name = "changebutton";
            this.changebutton.Size = new System.Drawing.Size(75, 41);
            this.changebutton.TabIndex = 9;
            this.changebutton.Text = "Change Background";
            this.changebutton.UseVisualStyleBackColor = true;
            this.changebutton.Click += new System.EventHandler(this.changebutton_Click);
            // 
            // gobutton
            // 
            this.gobutton.Location = new System.Drawing.Point(10, 80);
            this.gobutton.Name = "gobutton";
            this.gobutton.Size = new System.Drawing.Size(75, 23);
            this.gobutton.TabIndex = 0;
            this.gobutton.Text = "Go";
            this.gobutton.UseVisualStyleBackColor = true;
            this.gobutton.Click += new System.EventHandler(this.gobutton_Click);
            // 
            // timernum
            // 
            this.timernum.Location = new System.Drawing.Point(88, 58);
            this.timernum.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.timernum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.timernum.Name = "timernum";
            this.timernum.Size = new System.Drawing.Size(55, 20);
            this.timernum.TabIndex = 11;
            this.timernum.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // choose
            // 
            this.choose.Location = new System.Drawing.Point(248, 62);
            this.choose.Name = "choose";
            this.choose.Size = new System.Drawing.Size(75, 41);
            this.choose.TabIndex = 12;
            this.choose.Text = "Choose Picture";
            this.choose.UseVisualStyleBackColor = true;
            this.choose.Click += new System.EventHandler(this.choose_Click);
            // 
            // getpicdialog
            // 
            this.getpicdialog.Filter = "Image Files|*.bmp;*.BMP;*.jpg;*.JPG;*.png;*.PNG";
            // 
            // trayicon
            // 
            this.trayicon.ContextMenuStrip = this.contextMenuStrip1;
            this.trayicon.Text = "Desktop Switcher";
            this.trayicon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayicon_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.traychange,
            this.currentPicturesToolStripMenuItem1,
            this.restoreToolStripMenuItem,
            this.exit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(198, 92);
            // 
            // traychange
            // 
            this.traychange.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.traychange.Name = "traychange";
            this.traychange.Size = new System.Drawing.Size(197, 22);
            this.traychange.Text = "Change Background";
            this.traychange.Click += new System.EventHandler(this.traychange_Click);
            // 
            // currentPicturesToolStripMenuItem1
            // 
            this.currentPicturesToolStripMenuItem1.Name = "currentPicturesToolStripMenuItem1";
            this.currentPicturesToolStripMenuItem1.Size = new System.Drawing.Size(197, 22);
            this.currentPicturesToolStripMenuItem1.Text = "Current Pictures";
            this.currentPicturesToolStripMenuItem1.Click += new System.EventHandler(this.currentPicturesToolStripMenuItem1_Click);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // exit
            // 
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(197, 22);
            this.exit.Text = "Exit";
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // ratiobox
            // 
            this.ratiobox.Location = new System.Drawing.Point(193, 83);
            this.ratiobox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.ratiobox.Name = "ratiobox";
            this.ratiobox.Size = new System.Drawing.Size(33, 20);
            this.ratiobox.TabIndex = 13;
            this.ratiobox.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(101, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Scale Tolerance:";
            // 
            // denombox
            // 
            this.denombox.Cursor = System.Windows.Forms.Cursors.Default;
            this.denombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.denombox.FormattingEnabled = true;
            this.denombox.Items.AddRange(new object[] {
            "Seconds",
            "Minutes",
            "Hours",
            "Days"});
            this.denombox.Location = new System.Drawing.Point(146, 57);
            this.denombox.Name = "denombox";
            this.denombox.Size = new System.Drawing.Size(80, 21);
            this.denombox.TabIndex = 15;
            // 
            // usebox
            // 
            this.usebox.Location = new System.Drawing.Point(193, 109);
            this.usebox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.usebox.Name = "usebox";
            this.usebox.Size = new System.Drawing.Size(33, 20);
            this.usebox.TabIndex = 13;
            this.usebox.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(101, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Use Tolerance:";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 135);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.denombox);
            this.Controls.Add(this.ratiobox);
            this.Controls.Add(this.usebox);
            this.Controls.Add(this.timernum);
            this.Controls.Add(this.gobutton);
            this.Controls.Add(this.dirbutton);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.choose);
            this.Controls.Add(this.changebutton);
            this.Controls.Add(this.dirtb);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Desktop Switcher";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timernum)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ratiobox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usebox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox dirtb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button dirbutton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button changebutton;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.FolderBrowserDialog browsedialog;
        private System.Windows.Forms.Button gobutton;
        private System.Windows.Forms.NumericUpDown timernum;
        private System.Windows.Forms.Button choose;
        private System.Windows.Forms.OpenFileDialog getpicdialog;
        private System.Windows.Forms.NotifyIcon trayicon;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exit;
        private System.Windows.Forms.ToolStripMenuItem traychange;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dualmon;
        private System.Windows.Forms.ToolStripMenuItem screenslist;
        private System.Windows.Forms.ToolStripMenuItem startmintool;
        private System.Windows.Forms.NumericUpDown ratiobox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem diagnosticToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autostart;
        private System.Windows.Forms.ToolStripMenuItem subdirs;
        private System.Windows.Forms.ToolStripMenuItem currentPicturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showtips;
        private System.Windows.Forms.ToolStripMenuItem currentPicturesToolStripMenuItem1;
        private System.Windows.Forms.ComboBox denombox;
        private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statenable;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.NumericUpDown usebox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem winstart;
    }
}

