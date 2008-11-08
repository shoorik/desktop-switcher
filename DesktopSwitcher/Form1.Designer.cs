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
            this.label1 = new System.Windows.Forms.Label();
            this.regtb = new System.Windows.Forms.TextBox();
            this.dirtb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dirbutton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.changebutton = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.browsedialog = new System.Windows.Forms.FolderBrowserDialog();
            this.gobutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Registry Key:";
            // 
            // regtb
            // 
            this.regtb.Location = new System.Drawing.Point(87, 59);
            this.regtb.Name = "regtb";
            this.regtb.Size = new System.Drawing.Size(354, 20);
            this.regtb.TabIndex = 1;
            this.regtb.Text = "HKEY_CURRENT_USER\\Control Panel\\Desktop\\Wallpaper";
            // 
            // dirtb
            // 
            this.dirtb.Location = new System.Drawing.Point(87, 32);
            this.dirtb.Name = "dirtb";
            this.dirtb.Size = new System.Drawing.Size(320, 20);
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
            this.dirbutton.Location = new System.Drawing.Point(413, 32);
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
            this.label3.Location = new System.Drawing.Point(39, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Interval:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(87, 113);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(34, 20);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = "30";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(121, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Minutes";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(496, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // changebutton
            // 
            this.changebutton.Location = new System.Drawing.Point(393, 168);
            this.changebutton.Name = "changebutton";
            this.changebutton.Size = new System.Drawing.Size(75, 41);
            this.changebutton.TabIndex = 9;
            this.changebutton.Text = "Change Background";
            this.changebutton.UseVisualStyleBackColor = true;
            this.changebutton.Click += new System.EventHandler(this.changebutton_Click);
            // 
            // gobutton
            // 
            this.gobutton.Location = new System.Drawing.Point(75, 168);
            this.gobutton.Name = "gobutton";
            this.gobutton.Size = new System.Drawing.Size(75, 23);
            this.gobutton.TabIndex = 10;
            this.gobutton.Text = "Go";
            this.gobutton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 238);
            this.Controls.Add(this.gobutton);
            this.Controls.Add(this.changebutton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dirbutton);
            this.Controls.Add(this.dirtb);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.regtb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox regtb;
        private System.Windows.Forms.TextBox dirtb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button dirbutton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button changebutton;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.FolderBrowserDialog browsedialog;
        private System.Windows.Forms.Button gobutton;
    }
}

