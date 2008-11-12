﻿using System;
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
        int allheight;
        int highestscreen = 0;
        int[] heightfromtop = new int[10];
        int usedwidth = 0;

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
                ratiobox.Value = decimal.Parse((string)ourkey.GetValue("ratio"));
                autostart.Checked = bool.Parse((string)ourkey.GetValue("autostart"));
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
            if (autostart.Checked)
                start_timer();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            regsave();
        }

        /// <summary>
        /// finds the height from the top of the highest screen, to the bottom of the lowest screen
        /// </summary>
        private void findallheight()
        {
            for (int i = 0; i < desktops.Length; i++)
                if (desktops[i].Bounds.Height + heightfromtop[i] > allheight)
                    allheight = desktops[i].Bounds.Height + heightfromtop[i];
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
                findallheight();
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
            ourkey.SetValue("ratio", ratiobox.Value);
            ourkey.SetValue("autostart", autostart.Checked);
            for (int i = 0; i < heightfromtop.Length; i++)
                ourkey.SetValue("heightfromtop" + i, heightfromtop[i]);
            ourkey.Close();
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
            heightfromtop = new int[10];
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
            regsave();
            findallheight();
        }

        #region ratiostuff
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
            return (getratio(b) >= getratio(screen) - (double)ratiobox.Value && getratio(b) <= getratio(screen) + (double)ratiobox.Value);
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
            return (getratio(b) >= x/y - (double)ratiobox.Value && getratio(b) <= x/y + (double)ratiobox.Value);
        }
        #endregion

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

        /// <summary>
        /// changes wallpaper to random image
        /// </summary>
        /// <param name="use">file name and path to use for wallpaper, for random image, use ""</param>
        private void changepaper(string use)
        {
            File.Delete(dirtb.Text + "\\Background.bmp");
            string path = dirtb.Text + "\\Background.bmp";
            Bitmap final = new Bitmap(totalwidth, allheight);
            string file = getrandompic(0);
            Bitmap b = new Bitmap(dirtb.Text + "\\" + file);
            final = makepicture(final, b, 0);
            for (int i = 1; i < desktops.Length; i++)
            {
                string touse;
                if(dualmon.Checked)
                    touse = getrandompic(totalwidth - usedwidth);
                else
                    touse = file;
                Bitmap b2 = new Bitmap(dirtb.Text + "\\" + touse);
                final = makepicture(final, b2, i);
            }
            usedwidth = 0;
            final.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            setwallpaper(path, 1, 0);

            #region oldstuff
            //if (b.Width < totalwidth && use == "" && dualmon.Checked)
            //{
            //    for (int i = 1; i < desktops.Length; i++)
            //    {
            //        Bitmap temp = b;
            //        Bitmap b2 = new Bitmap(dirtb.Text + "\\" + getrandompic(desktops[i].Bounds.Width));
            //        Graphics g;
            //        b = new Bitmap(temp.Width + b2.Width, temp.Height);
            //        g = Graphics.FromImage(b);
            //        g.DrawImage(temp, 0, 0, temp.Width, temp.Height);
            //        g.DrawImage(b2, temp.Width + 1, 0, b2.Width, b2.Height);
            //        g.Save();
            //    }
            //}
            //b.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            //RegistryKey ourkey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            //ourkey.SetValue("Wallpaper", path);
            //ourkey.SetValue("TileWallpaper", "0");
            //ourkey.SetValue("WallpaperStyle", "0");
            //if (use == "" || use != "" && b.Width > desktops[0].Bounds.Width)
            //{
            //    ourkey.SetValue("TileWallpaper", "1");
            //    ourkey.SetValue("WallpaperStyle", "0");
            //}
            //else if (sameratio(b,0)) 
            //{
            //    ourkey.SetValue("TileWallpaper", "0");
            //    ourkey.SetValue("WallpaperStyle", "2");
            //}
            //ourkey.Close();
            //SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,SPIF_SENDWININICHANGE);
#endregion
        }

        /// <summary>
        /// scales down bitmap by specified scale
        /// http://www.personalmicrocosms.com/Pages/dotnettips.aspx?c=24&t=50#tip
        /// </summary>
        /// <param name="Bitmap">bitmap to scale</param>
        /// <param name="ScaleFactorX">x scale</param>
        /// <param name="ScaleFactorY">y scale</param>
        /// <returns></returns>
        public static Bitmap scale(Bitmap Bitmap, int scalex, int scaley)
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
        /// gets random picture from directory based on given max width, if picture is close enough to max width that the scaling will be correct, picture is returned
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
        /// returns combined width between and including specified screens
        /// </summary>
        /// <param name="start">screen to start at</param>
        /// <param name="end">screen to end at</param>
        /// <returns></returns>
        private int widthofscreens(int start, int end)
        {
            int temp = 0;
            for (int i = start; i <= end; i++)
                temp += desktops[i].Bounds.Width;
            return temp;
        }

        /// <summary>
        /// puts picture in correct placement on total screen bitmap
        /// </summary>
        /// <param name="b">the bitmap representing the entire wallpaper</param>
        /// <param name="picin">the picture to add to the wallpaper</param>
        /// <param name="screen">the screen to start adding to</param>
        /// <returns>a bitmap with new picture added into it</returns>
        private Bitmap makepicture(Bitmap c, Bitmap picin, int screen)
        {
            Bitmap b = c;
            int action = 0;
            int workingwidth = desktops[screen].Bounds.Width;
            int workingheight = desktops[screen].Bounds.Height;
            
            if (usedwidth >= widthofscreens(0,screen))  //if the screen has already been filled over, return the bitmap back unchanged
                return b;
            // if picture is sufficiently larger than the working screen, find how many more screens to go out to
            if (picin.Width > desktops[screen].Bounds.Width && !sameratio(picin, screen) && desktops.Length > 1)   
            {
                int i = screen;

                while (i < desktops.Length - 1 && picin.Width > workingwidth && !sameratio(picin, workingwidth, workingheight))
                {
                    i++;
                    workingwidth = widthofscreens(screen, i);
                }
                    


                //do
                //{
                //    if(i < desktops.Length )
                //        i++;
                //    workingwidth += desktops[i].Bounds.Width;
                //} while (i < desktops.Length && picin.Width > workingwidth && !sameratio(picin, workingwidth, workingheight)) ;

                if (!sameratio(picin, workingwidth, workingheight))
                    workingwidth -= desktops[i-1].Bounds.Width;
                else
                    action = 1;
            }

            if(sameratio(picin, workingwidth, workingheight))
                action = 1;

            int realheight = picin.Height;
            int realwidth = picin.Width;
            int xpad = 0;
            int ypad = 0;
            Bitmap temp = new Bitmap(workingwidth, workingheight);
            if (action == 0) //scaled (scaled up or down to fit in the closest screen bound)
            {
                if ((double)picin.Width / (double)workingwidth < (double)picin.Height / (double)workingheight)
                {
                    realheight = workingheight;
                    realwidth = (int)((double)picin.Width / ((double)picin.Height / (double)workingheight));
                    ypad = 0;
                    xpad = (workingwidth - realwidth) / 2;
                }
                else
                {
                    realheight = (int)((double)picin.Height / ((double)picin.Width / (double)workingwidth));
                    realwidth = workingwidth;
                    ypad = (workingheight - realheight) / 2;
                    xpad = 0;
                }
                Graphics g;
                g = Graphics.FromImage(temp);
                g.DrawImage(scale(picin,realwidth,realheight), xpad, ypad, realwidth, realheight);
                g.Save();
            }

            else if (action == 1)    //stretched (stretched slightly to fill the entire screen) 
            {                       //tiled (no change in size, picture just added in)
                Graphics g;
                g = Graphics.FromImage(temp);
                g.DrawImage(scale(picin,workingwidth,workingheight), 0, 0, workingwidth, workingheight);
                g.Save();
            }

            Graphics h;
            h = Graphics.FromImage(b);
            h.DrawImage(temp, usedwidth, heightfromtop[screen], workingwidth, workingheight);
            h.Save();

            usedwidth += workingwidth;

            return b;
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

        #region events
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                trayicon.Visible = true;
                this.ShowInTaskbar = false;
                this.Hide();
            }
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
                if (checkforpics(di.GetFiles()))
                {
                    if (dirtb.Text != "")
                        File.Delete(dirtb.Text + "\\Background.bmp");
                    dirtb.Text = browsedialog.SelectedPath;
                    if (File.Exists(dirtb.Text + "\\Background.bmp"))
                        if (MessageBox.Show("Warning: There is a picture in this directory named \"Background.bmp\", this picture will be erased if no action is taken\nWould you like to rename it to \"Background2.bmp?\"","Warning",MessageBoxButtons.YesNo) == DialogResult.Yes)
                            File.Move(dirtb.Text + "\\Background.bmp", dirtb.Text + "\\Background2.bmp");
                }
                else
                    MessageBox.Show("There are no pictures in the specified directory");
                regsave();
            }
        }

        private void gobutton_Click(object sender, EventArgs e)
        {
            if (gobutton.Text == "Go")
            {
                start_timer();
            }
            else
            {
                timer.Stop();
                gobutton.Text = "Go";
            }
        }

        private void start_timer()
        {
            timer.Interval = decimal.ToInt32(timernum.Value * milidenoms[denomindex]);
            timer.Start();
            gobutton.Text = "Stop";
        }

        private void changebutton_Click(object sender, EventArgs e)
        {
            changepaper("");
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
        #endregion
    }
}