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
        int[] milidenoms = new int[] { 1000, 60000, 3600000, 8640000 };             //                                                        //
        Screen[] desktops = Screen.AllScreens; //array of all screens on system
        int totalwidth; //total width of all screens
        int allheight = 0;
        int usedwidth = 0;
        string pics = "";
        bool usedpic = true;
        int farthestleft = 0;
        string lastpic = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dualmon.Checked = desktops.Length > 1;
            timer.Tick += new EventHandler(timer_Tick);
            trayicon.Icon = this.Icon;
            denombox.SelectedIndex = 1;
            try
            {
                RegistryKey ourkey = Registry.Users;
                ourkey = ourkey.OpenSubKey(@".DEFAULT\Software\Schraitle\Desktop");
                dirtb.Text = (string)ourkey.GetValue("dir");
                timernum.Value = decimal.Parse((string)ourkey.GetValue("interval"));
                startmintool.Checked = bool.Parse((string)ourkey.GetValue("startmin"));
                denombox.SelectedIndex = (int)ourkey.GetValue("denomindex");
                dualmon.Checked = bool.Parse((string)ourkey.GetValue("dualmon"));
                ratiobox.Value = decimal.Parse((string)ourkey.GetValue("ratio"));
                autostart.Checked = bool.Parse((string)ourkey.GetValue("autostart"));
                subdirs.Checked = bool.Parse((string)ourkey.GetValue("subdirs"));
                showtips.Checked = bool.Parse((string)ourkey.GetValue("balloon"));
                ourkey.Close();
            }
            catch (Exception x) { x.ToString(); }

            if (startmintool.Checked)
                this.WindowState = FormWindowState.Minimized;
            getscreens();
            if (autostart.Checked)
                start_timer();
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new System.EventHandler(displaySettingsChanged);

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            regsave();
        }

        /// <summary>
        /// counts number of screens and their sizes, displays them in menu bar, saves them in desktops array
        /// puts screens in array in physical order
        /// </summary>
        private void getscreens()
        {
            farthestleft = 0;
            totalwidth = 0;
            allheight = 0;
            int wide = farthestleft;
            Screen[] temp = Screen.AllScreens;
            ToolStripMenuItem[] t = new ToolStripMenuItem[desktops.Length];
            for (int i = 0; i < desktops.Length; i++)
            {
                totalwidth += desktops[i].Bounds.Width;
                if (desktops[i].Bounds.Height + desktops[i].WorkingArea.Y > allheight)
                    allheight = desktops[i].Bounds.Height + desktops[i].WorkingArea.Y;
                if (desktops[i].WorkingArea.X < farthestleft)
                    farthestleft = desktops[i].WorkingArea.X;
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
            for (int i = 0; i < desktops.Length; i++)
                if (desktops[i].WorkingArea.X == wide)
                {
                    temp[i] = desktops[i];
                    wide += temp[i].Bounds.Width;
                }
            desktops = temp;
            screenslist.DropDownItems.Clear();
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
            ourkey.SetValue("denomindex", denombox.SelectedIndex);
            ourkey.SetValue("dualmon", dualmon.Checked);
            ourkey.SetValue("ratio", ratiobox.Value);
            ourkey.SetValue("autostart", autostart.Checked);
            ourkey.SetValue("subdirs", subdirs.Checked);
            ourkey.SetValue("balloon", showtips.Checked);
            ourkey.Close();
        }

        /// <summary>
        /// gets information about the screens
        /// </summary>
        /// <param name="popup">whether or not to display screen information after diagnostic</param>
        private void diagnostic(bool popup)
        {
            desktops = Screen.AllScreens;
            getscreens();
            regsave();
            if (popup)
            {
                string done = " Screen(s)\n\nOrder:\n";
                for (int i = 0; i < desktops.Length; i++)
                    done += " | " + desktops[i].DeviceName.Substring(11, 1).ToString(); ;
                done += " |";
                MessageBox.Show("Done!\n\n" + desktops.Length.ToString() + done);
            }
        }

        #region ratiostuff
        /// <summary>
        /// gets ratio of picture
        /// </summary>
        /// <param name="b">bitmap to get ratio of</param>
        /// <returns></returns>
        private double getratio(ref Bitmap b)
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
        private bool sameratio(ref Bitmap b, int screen)
        {
            return (getratio(ref b) >= getratio(screen) - ((double)ratiobox.Value / 100 * getratio(screen)) && getratio(ref b) <= getratio(screen) + ((double)ratiobox.Value / 100 * getratio(screen)));
        }

        /// <summary>
        /// determines if ratio of bitmap is close enough to dimensions given
        /// </summary>
        /// <param name="b">bitmap to test</param>
        /// <param name="x">width dimension</param>
        /// <param name="y">height dimension</param>
        /// <returns></returns>
        private bool sameratio(ref Bitmap b, double x, double y)
        {
            return (getratio(ref b) >= x/y - ((double)ratiobox.Value / 100 * x/y) && getratio(ref b) <= x/y + ((double)ratiobox.Value / 100 * x/y));
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
            string path = dirtb.Text + "\\Background.bmp";
            File.Delete(dirtb.Text + "\\Background.bmp");
            Bitmap final = new Bitmap(totalwidth, allheight);
            string file = use;
            pics = "";
            //if (use == "")
            //    file = getrandompic(0);
            //pics = "Screen 1: " + file;
            //Bitmap b = new Bitmap(file);
            //makepicture(ref final,ref b, 0);
            for (int i = 0; i < desktops.Length; i++)
            {
                string touse;
                if(dualmon.Checked && use == "")
                    touse = getrandompic(totalwidth - usedwidth);
                else
                    touse = file;
                lastpic = touse;
                
                Bitmap b2 = new Bitmap(touse);
                makepicture(ref final,ref b2, i);
                if (usedpic)
                    pics += "\nScreen " + (i+1) + ": " + touse;
                usedpic = true;
                b2.Dispose();
                if (usedwidth == totalwidth)
                    i = desktops.Length;
            }
            usedwidth = 0;

            Bitmap newfinal = new Bitmap(final.Width, final.Height);         
            if(farthestleft < 0)
            {
                Bitmap right = final.Clone(new Rectangle((farthestleft * -1),0,final.Width-(farthestleft * -1),final.Height),final.PixelFormat);
                Bitmap left = final.Clone(new Rectangle(0,0,(farthestleft * -1),final.Height),final.PixelFormat);
                
                Graphics h;
                h = Graphics.FromImage(newfinal);
                h.DrawImage(right, 0, 0, right.Width, right.Height);
                h.DrawImage(left, right.Width,0,left.Width,left.Height);
                h.Save();
                h.Dispose();
                right.Dispose();
                left.Dispose();
                newfinal.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            else
                final.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);

                
            setwallpaper(path, 1, 0);
            if (showtips.Checked)
            {
                trayicon.BalloonTipText = pics;
                trayicon.ShowBalloonTip(10000,"", pics, ToolTipIcon.Info);
            }
            final.Dispose();
            newfinal.Dispose();
            //b.Dispose();
        }

        /// <summary>
        /// scales down bitmap to specified dimensions
        /// http://www.personalmicrocosms.com/Pages/dotnettips.aspx?c=24&t=50#tip
        /// </summary>
        /// <param name="Bitmap">bitmap to scale</param>
        /// <param name="ScaleFactorX">x scale</param>
        /// <param name="ScaleFactorY">y scale</param>
        /// <returns></returns>
        public static void scale(ref Bitmap Bitmap, ref Bitmap scaledBitmap, int scalex, int scaley)
        {
            scaledBitmap = new Bitmap(scalex, scaley);

            // Scale the bitmap in high quality mode.
            Graphics gr = Graphics.FromImage(scaledBitmap);
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gr.DrawImage(Bitmap, new Rectangle(0, 0, scalex, scaley), new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), GraphicsUnit.Pixel);
            gr.Save();
            gr.Dispose();
        }

        /// <summary>
        /// gets random picture from directory based on given max width, if picture is close enough to max width that the scaling will be correct, picture is returned, even if the width is greater than maxwidth
        /// </summary>
        /// <param name="maxwidth">maximum picture width that is returned; any size = 0</param>
        private string getrandompic(int maxwidth)
        {
            bool ok = true;
            ArrayList pics = new ArrayList();
            DirectoryInfo di = new DirectoryInfo(dirtb.Text);
            FileInfo[] all = di.GetFiles();
            if(subdirs.Checked)
                all = di.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo f in all)
                if (exts.Contains(f.Extension.ToLower()) && f.Extension != "")
                    pics.Add(f);
            FileInfo temp;
            Bitmap b;
            int fail = 0;
            do
            {
                int c = new Random().Next(pics.Count);
                temp = (FileInfo)pics[c];
                b = new Bitmap(temp.FullName);
                if (b.Width > maxwidth && !sameratio(ref b,maxwidth,allheight))
                    ok = false;
                else
                    ok = true;
                if (maxwidth == 0 || fail == 100)
                    ok = true;
                b.Dispose();
                fail++;
            } while (!ok);
            return temp.FullName;
        }

        private void choose_Click(object sender, EventArgs e)
        {
            getpicdialog.InitialDirectory = dirtb.Text;
            if (getpicdialog.ShowDialog() == DialogResult.OK)
                changepaper(getpicdialog.FileName);            
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

        private Bitmap rotateImage(Bitmap b, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Height, b.Width);
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(returnBitmap);
            //move rotation point to center of image
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            //rotate
            g.RotateTransform(angle);
            //move image back
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }

        /// <summary>
        /// puts picture in correct placement on total screen bitmap
        /// </summary>
        /// <param name="b">the bitmap representing the entire wallpaper</param>
        /// <param name="picin">the picture to add to the wallpaper</param>
        /// <param name="screen">the screen to start adding to</param>
        /// <returns>a bitmap with new picture added into it</returns>
        private void makepicture(ref Bitmap b, ref Bitmap picin, int screen)
        {
            int action = 0;
            int workingwidth = desktops[screen].Bounds.Width;
            int workingheight = desktops[screen].Bounds.Height;

            if (usedwidth >= widthofscreens(0, screen))  //if the screen has already been filled over, return the bitmap back unchanged
            {
                usedpic = false;
                return;
            }
            // if picture is sufficiently larger than the working screen, find how many more screens to go out to
            if (picin.Width > desktops[screen].Bounds.Width && !sameratio(ref picin, screen) && desktops.Length > 1)   
            {
                int i = screen;

                while (i < desktops.Length - 1 && picin.Width > workingwidth && !sameratio(ref picin, workingwidth, workingheight))
                {
                    i++;
                    workingwidth = widthofscreens(screen, i);
                }
                if (!sameratio(ref picin, workingwidth, workingheight))
                    if(picin.Width >= workingwidth)
                        action = 0;
                    else
                        workingwidth -= desktops[i].Bounds.Width;
                else
                    action = 1;
            }

            if(sameratio(ref picin, workingwidth, workingheight))
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
                Bitmap todraw = new Bitmap(1,1);
                scale(ref picin, ref todraw, realwidth,realheight);
                g.DrawImage(todraw, xpad, ypad, realwidth, realheight);
                todraw.Dispose();
                g.Save();
                g.Dispose();
            }

            else if (action == 1)    //stretched (stretched slightly to fill the entire screen) 
            {                       //tiled (no change in size, picture just added in)
                Graphics g;
                g = Graphics.FromImage(temp);
                Bitmap todraw = new Bitmap(1,1);
                scale(ref picin,ref todraw, workingwidth,workingheight);
                g.DrawImage(todraw, 0, 0, workingwidth, workingheight);
                todraw.Dispose();
                g.Save();
                g.Dispose();
            }

            Graphics h;
            h = Graphics.FromImage(b);
            h.DrawImage(temp, usedwidth, desktops[screen].WorkingArea.Y, workingwidth, workingheight);
            //h.DrawImage(temp, usedwidth, heightfromtop[screen], workingwidth, workingheight);
            h.Save();
            h.Dispose();

            temp.Dispose();
            usedwidth += workingwidth;
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
            timer.Interval = decimal.ToInt32(timernum.Value * milidenoms[denombox.SelectedIndex]);
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
            this.Text = "Desktop Switcher";
            this.ShowInTaskbar = true;
            trayicon.Visible = false;

        }

        private void dualmon_Click(object sender, EventArgs e)
        {
            if (desktops.Length < 2)
                dualmon.Checked = false;
        }

        private void diagnosticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diagnostic(true);
        }
        #endregion

        private void displaySettingsChanged(object sender, EventArgs e)
        {
            diagnostic(false);
            changepaper(lastpic);
        }

        private void currentPicturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(pics, "Current Backgrounds");
        }

        private void currentPicturesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(pics, "Current Pictures");            
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] dropped = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (exts.Contains(dropped[0].Substring(dropped[0].Length - 4, 4).ToLower()))
                changepaper(dropped[0]);
        }
    }
}
