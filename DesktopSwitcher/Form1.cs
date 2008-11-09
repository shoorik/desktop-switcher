using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace DesktopSwitcher
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        string exts = ".jpg.jpeg.bmp.png";
        string[] denoms = new string[] { "Seconds", "Minutes", "Hours", "Days" };
        int[] milidenoms = new int[] { 1000, 60000, 3600000, 8640000 };
        int denomindex = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Tick += new EventHandler(timer_Tick);
            trayicon.Icon = this.Icon;
            try
            {
                RegistryKey ourkey = Registry.Users;
                ourkey = ourkey.OpenSubKey(@".DEFAULT\Software\Schraitle\Desktop");
                dirtb.Text = (string)ourkey.GetValue("dir");
                timernum.Value = decimal.Parse((string)ourkey.GetValue("interval"));
                startmintool.Checked = bool.Parse((string)ourkey.GetValue("startmin"));
                denomindex = (int)ourkey.GetValue("denomindex");
                dualmon.Checked = bool.Parse((string)ourkey.GetValue("dualmon"));
                ourkey.Close();
            }
            catch (Exception x) { x.ToString(); }

            if (startmintool.Checked)
                this.WindowState = FormWindowState.Minimized;
            timedenom.Text = denoms[denomindex];
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            regsave();
        }

        private void regsave()
        {
            RegistryKey ourkey = Registry.Users;
            ourkey = ourkey.CreateSubKey(@".DEFAULT\Software\Schraitle\Desktop");
            ourkey.OpenSubKey(@".DEFAULT\Software\Schraitle\Desktop", true);
            ourkey.SetValue("dir", dirtb.Text);
            ourkey.SetValue("interval", timernum.Value);
            ourkey.SetValue("startmin", startmintool.Checked);
            ourkey.SetValue("denomindex", denomindex);
            ourkey.SetValue("dualmon", dualmon.Checked);
            ourkey.Close();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            changepaper("");
        }

        private void dirbutton_Click(object sender, EventArgs e)
        {
            if (browsedialog.ShowDialog() == DialogResult.OK) 
            {
                DirectoryInfo di = new DirectoryInfo(browsedialog.SelectedPath);
                if(checkforpics(di.GetFiles()))
                    dirtb.Text = browsedialog.SelectedPath;
                else
                    MessageBox.Show("There are no pictures in the specified directory");
                regsave();
            }
        }

        /// <summary>
        /// checks given list of files for picture images (BMP, JPG, JPEG)
        /// </summary>
        /// <param name="files">the file list to check</param>
        private bool checkforpics(FileInfo[] files)
        {
            foreach(FileInfo f in files)
                if (exts.Contains(f.Extension.ToLower())) 
                    return true;
            return false;
        }

        private void changebutton_Click(object sender, EventArgs e)
        {
            changepaper("");
        }

        /// <summary>
        /// changes wallpaper to random image
        /// </summary>
        /// <param name="use">file name and path to use for wallpaper, for random image, use ""</param>
        private void changepaper(string use)
        {
            File.Delete(dirtb.Text + "\\Background.bmp");
            string file;
            string path = dirtb.Text + "\\" + use;
            if (use != "")
                file = use;
            else
                file = dirtb.Text + "\\" + getrandompic(0);
            path = dirtb.Text + "\\Background.bmp";
            Bitmap b = new Bitmap(file);
            if (b.Width < 2560 && use == "" && dualmon.Checked)
            {
                Bitmap temp = b;
                Bitmap b2 = new Bitmap(dirtb.Text + "\\" + getrandompic(temp.Width));
                Graphics g;
                b = new Bitmap(temp.Width + b2.Width, temp.Height);
                g = Graphics.FromImage(b);
                g.DrawImage(temp, 0, 0,temp.Width,temp.Height);
                g.DrawImage(b2, temp.Width + 1, 0, b2.Width, b2.Height);
                g.Save();
            }
            b.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            RegistryKey ourkey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            ourkey.SetValue("Wallpaper", path);
            ourkey.SetValue("TileWallpaper", (use == "" || use != "" && b.Width > 1024) ? "1": "0");
            ourkey.Close();
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,SPIF_SENDWININICHANGE);
        }

        /// <summary>
        /// gets random picture from directory
        /// </summary>
        /// <param name="maxwidth">maximum picture width that is returned any size = 0</param>
        private string getrandompic(int maxwidth)
        {
            bool ok = true;
            ArrayList pics = new ArrayList();
            DirectoryInfo di = new DirectoryInfo(dirtb.Text);
            foreach (FileInfo f in di.GetFiles())
                if (exts.Contains(f.Extension.ToLower()))
                    pics.Add(f);
            FileInfo temp;
            Bitmap b;
            do
            {
                int c = new Random().Next(pics.Count);
                temp = (FileInfo)pics[c];
                b = new Bitmap(dirtb.Text + "\\" + temp.Name);
                if (b.Width > maxwidth)
                    ok = true;
                else
                    ok = false;
                if (maxwidth == 0)
                    ok = false;
            } while (ok);
            return temp.Name;
        }

        private void gobutton_Click(object sender, EventArgs e)
        {
            if (gobutton.Text == "Go")
            {
                timer.Interval = decimal.ToInt32(timernum.Value * milidenoms[denomindex]);
                timer.Start();
                gobutton.Text = "Stop";
            }
            else
            {
                timer.Stop();
                gobutton.Text = "Go";
            }
        }

        private void choose_Click(object sender, EventArgs e)
        {
            getpicdialog.InitialDirectory = dirtb.Text;
            if(getpicdialog.ShowDialog() == DialogResult.OK)
                changepaper(getpicdialog.FileName);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                trayicon.Visible = true;
                this.ShowInTaskbar = false;
                this.Hide();
            }
        }

        private void trayicon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            changepaper("");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void traychange_Click(object sender, EventArgs e)
        {
            changepaper("");
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
            trayicon.Visible = false;

        }

        private void timedenom_Click(object sender, EventArgs e)
        {
            if (++denomindex == 4)
                denomindex = 0;
            timedenom.Text = denoms[denomindex];
        }
    }
}
