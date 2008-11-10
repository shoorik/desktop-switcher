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
        string[] denoms = new string[] { "Seconds", "Minutes", "Hours", "Days" };   //for interval settings
        int[] milidenoms = new int[] { 1000, 60000, 3600000, 8640000 };             //
        int denomindex = 1;                                                         //
        Screen[] desktops = Screen.AllScreens;  //array of all screens on system
        int totalwidth; //total width of all screens
        int highestscreen = 0;
        int[] heightfromtop = new int[10];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dualmon.Checked = desktops.Length > 1;
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
                try
                {
                    for (int i = 0; i < heightfromtop.Length; i++)
                        heightfromtop[i] = (int)ourkey.GetValue("heightfromtop" + i);
                }
                catch (Exception x) { x.ToString(); diagnostic(); }
                ourkey.Close();
            }
            catch (Exception x) { x.ToString(); }

            if (startmintool.Checked)
                this.WindowState = FormWindowState.Minimized;
            timedenom.Text = denoms[denomindex];
            getscreens();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            regsave();
        }

        /// <summary>
        /// counts number of screens and their sizes, displays them in menu bar, saves them in desktops array
        /// </summary>
        private void getscreens()
        {
            ToolStripMenuItem[] t = new ToolStripMenuItem[desktops.Length];
            for (int i = 0; i < desktops.Length; i++)
            {
                totalwidth += desktops[i].Bounds.Width;
                if (desktops[i].Bounds.Height > highestscreen)
                    highestscreen = desktops[i].Bounds.Height;
                t[i] = new ToolStripMenuItem();
                ToolStripMenuItem[] props = new ToolStripMenuItem[3];
                for (int j = 0; j < props.Length; j++)
                    props[j] = new ToolStripMenuItem();
                t[i].Text = "Screen " + (i + 1).ToString();
                if (desktops[i].Primary)
                    t[i].Text += "(P)";
                props[0].Text = "Width:  " + desktops[i].Bounds.Width.ToString();
                props[1].Text = "Height: " + desktops[i].Bounds.Height.ToString();
                props[2].Text = "Ratio:  " + getratio(i).ToString();

                t[i].DropDownItems.AddRange(new ToolStripItemCollection(menuStrip1, props));
            }
            screenslist.DropDownItems.AddRange(new ToolStripItemCollection(menuStrip1, t));
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
            for (int i = 0; i < heightfromtop.Length; i++)
                ourkey.SetValue("heightfromtop" + i, heightfromtop[i]);
            ourkey.Close();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            changepaper("");
        }

        /// <summary>
        /// captures an image of the screen when background is all white to determine the different screen orientations
        /// </summary>
        private void diagnostic()
        {
            Bitmap b = new Bitmap(10, 10);
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    b.SetPixel(i, j, Color.White);
            b.Save("C:\\schraitletemp.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            RegistryKey ourkey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            string path = (string)ourkey.GetValue("Wallpaper");
            string tile = (string)ourkey.GetValue("TileWallpaper");
            string style = (string)ourkey.GetValue("WallpaperStyle");
            ourkey.SetValue("Wallpaper", "C:\\schraitletemp.bmp");
            ourkey.SetValue("TileWallpaper", "1");
            ourkey.SetValue("WallpaperStyle", "0");
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, "C:\\schraitletemp.bmp", SPIF_SENDWININICHANGE);
            System.Threading.Thread.Sleep(500);
            SendKeys.SendWait("^{prtsc}");
            Bitmap test = new Bitmap(Clipboard.GetImage());
            ourkey.SetValue("Wallpaper", path);
            ourkey.SetValue("TileWallpaper", tile);
            ourkey.SetValue("WallpaperStyle", style);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_SENDWININICHANGE);
            File.Delete("c:\\schraitletemp.bmp");
            for (int i = 0; i < test.Height; i++)
                if (test.GetPixel(50, i) == Color.FromArgb(0, 0, 0))
                    heightfromtop[0] = i;
                else
                    i = test.Height;
            for (int i = 1; i < desktops.Length; i++)
                for (int j = 0; j < test.Height; j++)
                    if (test.GetPixel(desktops[i - 1].Bounds.Width + 50, j) == Color.FromArgb(0,0,0))
                        heightfromtop[i] = j;
                    else
                        j = test.Height;
            ourkey.Close();
        }

        /// <summary>
        /// gets ratio of picture
        /// </summary>
        /// <param name="b">bitmap to get ratio of</param>
        /// <returns></returns>
        private double getratio(Bitmap b)
        {
            return (double)b.Width / (double)b.Height;
        }

        /// <summary>
        /// gets ratio of screen
        /// </summary>
        /// <param name="screen">screen index of desktops array to get ratio of</param>
        /// <returns></returns>
        private double getratio(int screen)
        {
            return (double)desktops[screen].Bounds.Width / (double)desktops[screen].Bounds.Height;
        }

        /// <summary>
        /// determines whether or not the ratio of the picture is close enough to the ratio of the screen
        /// </summary>
        /// <param name="b">bitmap to test</param>
        /// <param name="screen">index of the screen in the desktop array</param>
        /// <returns></returns>
        private bool sameratio(Bitmap b, int screen)
        {
            return (getratio(b) > getratio(screen) - (double)ratiobox.Value && getratio(b) < getratio(screen) + (double)ratiobox.Value);
        }

        /// <summary>
        /// determines if ratio of bitmap is close enough to dimensions given
        /// </summary>
        /// <param name="b">bitmap to test</param>
        /// <param name="x">width dimension</param>
        /// <param name="y">height dimension</param>
        /// <returns></returns>
        private bool sameratio(Bitmap b, double x, double y)
        {
            return (getratio(b) > x/y - (double)ratiobox.Value && getratio(b) < x/y + (double)ratiobox.Value);
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
            string path = dirtb.Text + "\\Background.bmp";
            Bitmap final = new Bitmap(totalwidth, highestscreen);
            Bitmap b = new Bitmap(dirtb.Text + "\\" + getrandompic(0));
            if (b.Width < totalwidth && use == "" && dualmon.Checked)
            {
                for (int i = 1; i < desktops.Length; i++)
                {
                    Bitmap temp = b;
                    Bitmap b2 = new Bitmap(dirtb.Text + "\\" + getrandompic(desktops[i].Bounds.Width));
                    Graphics g;
                    b = new Bitmap(temp.Width + b2.Width, temp.Height);
                    g = Graphics.FromImage(b);
                    g.DrawImage(temp, 0, 0, temp.Width, temp.Height);
                    g.DrawImage(b2, temp.Width + 1, 0, b2.Width, b2.Height);
                    g.Save();
                }
            }
            b.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            RegistryKey ourkey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            ourkey.SetValue("Wallpaper", path);
            ourkey.SetValue("TileWallpaper", "0");
            ourkey.SetValue("WallpaperStyle", "0");
            if (use == "" || use != "" && b.Width > desktops[0].Bounds.Width)
            {
                ourkey.SetValue("TileWallpaper", "1");
                ourkey.SetValue("WallpaperStyle", "0");
            }
            else if (sameratio(b,0)) 
            {
                ourkey.SetValue("TileWallpaper", "0");
                ourkey.SetValue("WallpaperStyle", "2");
            }
            ourkey.Close();
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,SPIF_SENDWININICHANGE);
        }

        /// <summary>
        /// scales down bitmap by specified scale
        /// http://www.personalmicrocosms.com/Pages/dotnettips.aspx?c=24&t=50#tip
        /// </summary>
        /// <param name="Bitmap">bitmap to scale</param>
        /// <param name="ScaleFactorX">x scale</param>
        /// <param name="ScaleFactorY">y scale</param>
        /// <returns></returns>
        public static Bitmap Scale(Bitmap Bitmap, int scalex, int scaley )
        {
            Bitmap scaledBitmap = new Bitmap(scalex, scaley);

            // Scale the bitmap in high quality mode.
            using (Graphics gr = Graphics.FromImage(scaledBitmap))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                gr.DrawImage(Bitmap, new Rectangle(0, 0, scalex, scaley), new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), GraphicsUnit.Pixel);
            }

            return scaledBitmap;
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
            string file;
            getpicdialog.InitialDirectory = dirtb.Text;
            if (getpicdialog.ShowDialog() == DialogResult.OK)
                file = getpicdialog.FileName;
            else
                return;
            File.Delete(dirtb.Text + "\\Background.bmp");
            string path = dirtb.Text + "\\Background.bmp";
            Bitmap b = new Bitmap(totalwidth,highestscreen);


            b.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            setwallpaper(path, 0, 0);
        }

        /// <summary>
        /// puts picture in correct 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="picin"></param>
        /// <param name="screen"></param>
        /// <returns></returns>
        private Bitmap makepicture(Bitmap b, Bitmap picin, int screen)
        {

        }

        /// <summary>
        /// performs events necessary to change wallpaper
        /// </summary>
        /// <param name="path">path of new wallpaper</param>
        /// <param name="tile">tile switch</param>
        /// <param name="style">style switch, 2 = centered</param>
        private void setwallpaper(string path, int tile, int style)
        {
            RegistryKey ourkey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            ourkey.SetValue("Wallpaper", path);
            ourkey.SetValue("TileWallpaper", tile.ToString());
            ourkey.SetValue("WallpaperStyle", style.ToString());
            ourkey.Close();
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_SENDWININICHANGE);

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

        private void dualmon_Click(object sender, EventArgs e)
        {
            if (desktops.Length < 2)
                dualmon.Checked = false;
        }

        private void diagnosticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diagnostic();
        }
    }
}
