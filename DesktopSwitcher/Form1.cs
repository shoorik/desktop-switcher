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
using System.Xml.Serialization;
using System.Diagnostics;

namespace DesktopSwitcher
{
    public partial class Form1 : Form
    {
        #region activedesktop
        //taken from http://groups.google.com/group/microsoft.public.dotnet.framework.interop/msg/9b42ff75078b71c0?pli=1
        enum WPSTYLE
        {
            CENTER = 0,
            TILE = 1,
            STRETCH = 2,
            MAX = 3
        }

        struct WALLPAPEROPT
        {
            public int dwSize;
            public WPSTYLE dwStyle;
        }

        struct COMPONENTSOPT
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fEnableComponents;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fActiveDesktop;
        }

        struct COMPPOS
        {
            public const int COMPONENT_TOP = 0x3FFFFFFF;
            public const int COMPONENT_DEFAULT_LEFT = 0xFFFF;
            public const int COMPONENT_DEFAULT_TOP = 0xFFFF;

            public int dwSize;
            public int iLeft;
            public int iTop;
            public int dwWidth;
            public int dwHeight;
            public int izIndex;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fCanResize;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fCanResizeX;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fCanResizeY;
            public int iPreferredLeftPercent;
            public int iPreferredTopPercent;
        }

        [Flags]
        enum ITEMSTATE
        {
            NORMAL = 0x00000001,
            FULLSCREEN = 00000002,
            SPLIT = 0x00000004,
            VALIDSIZESTATEBITS =
            NORMAL | SPLIT | FULLSCREEN,
            VALIDSTATEBITS =
            NORMAL | SPLIT | FULLSCREEN |
            unchecked((int)0x80000000) | 0x40000000
        }

        struct COMPSTATEINFO
        {
            public int dwSize;
            public int iLeft;
            public int iTop;
            public int dwWidth;
            public int dwHeight;
            public int dwItemState;
        }

        enum COMP_TYPE
        {
            HTMLDOC = 0,
            PICTURE = 1,
            WEBSITE = 2,
            CONTROL = 3,
            CFHTML = 4,
            MAX = 4
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct COMPONENT
        {
            private const int INTERNET_MAX_URL_LENGTH = 2084;
            public int dwSize;
            public int dwID;
            public COMP_TYPE iComponentType;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fChecked;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fDirty;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fNoScroll;
            public COMPPOS cpPos;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string wszFriendlyName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = INTERNET_MAX_URL_LENGTH)]
            public string wszSource;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = INTERNET_MAX_URL_LENGTH)]
            public string wszSubscribedURL;

#if AD_IE5
public int dwCurItemState;
public COMPSTATEINFO csiOriginal;
public COMPSTATEINFO csiRestored;
#endif
        }

        enum DTI_ADTIWUI
        {
            DTI_ADDUI_DEFAULT = 0x00000000,
            DTI_ADDUI_DISPSUBWIZARD = 0x00000001,
            DTI_ADDUI_POSITIONITEM = 0x00000002,
        }

        [Flags]
        enum AD_APPLY
        {
            SAVE = 0x00000001,
            HTMLGEN = 0x00000002,
            REFRESH = 0x00000004,
            ALL = SAVE | HTMLGEN | REFRESH,
            FORCE = 0x00000008,
            BUFFERED_REFRESH = 0x00000010,
            DYNAMICREFRESH = 0x00000020
        }

        [Flags]
        enum COMP_ELEM
        {
            TYPE = 0x00000001,
            CHECKED = 0x00000002,
            DIRTY = 0x00000004,
            NOSCROLL = 0x00000008,
            POS_LEFT = 0x00000010,
            POS_TOP = 0x00000020,
            SIZE_WIDTH = 0x00000040,
            SIZE_HEIGHT = 0x00000080,
            POS_ZINDEX = 0x00000100,
            SOURCE = 0x00000200,
            FRIENDLYNAME = 0x00000400,
            SUBSCRIBEDURL = 0x00000800,
            ORIGINAL_CSI = 0x00001000,
            RESTORED_CSI = 0x00002000,
            CURITEMSTATE = 0x00004000,
            ALL = TYPE | CHECKED | DIRTY | NOSCROLL | POS_LEFT |
            SIZE_WIDTH | SIZE_HEIGHT | POS_ZINDEX | SOURCE |
            FRIENDLYNAME | POS_TOP | SUBSCRIBEDURL | ORIGINAL_CSI |
            RESTORED_CSI | CURITEMSTATE
        }

        [Flags]
        enum ADDURL
        {
            SILENT = 0x0001
        }

        [
        ComImport(),
        Guid("F490EB00-1240-11D1-9888-006097DEACF9"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        interface IActiveDesktop
        {
            void ApplyChanges(AD_APPLY dwFlags);
            void GetWallpaper([MarshalAs(UnmanagedType.LPWStr)]
System.Text.StringBuilder pwszWallpaper, int cchWallpaper, int
            dwReserved);
            void SetWallpaper([MarshalAs(UnmanagedType.LPWStr)]
string pwszWallpaper, int dwReserved);
            void GetWallpaperOptions(ref WALLPAPEROPT pwpo, int dwReserved);
            void SetWallpaperOptions([In] ref WALLPAPEROPT pwpo, int
            dwReserved);
            void GetPattern([MarshalAs(UnmanagedType.LPWStr)]
System.Text.StringBuilder pwszPattern, int cchPattern, int
            dwReserved);
            void SetPattern([MarshalAs(UnmanagedType.LPWStr)] string
            pwszPattern, int dwReserved);
            void GetDesktopItemOptions(ref COMPONENTSOPT pco, int dwReserved);
            void SetDesktopItemOptions([In] ref COMPONENTSOPT pco, int
            dwReserved);
            void AddDesktopItem([In] ref COMPONENT pcomp, int dwReserved);
            void AddDesktopItemWithUI(IntPtr hwnd, [In] ref COMPONENT pcomp,
            DTI_ADTIWUI dwFlags);
            void ModifyDesktopItem([In] ref COMPONENT pcomp, COMP_ELEM
            dwFlags);
            void RemoveDesktopItem([In] ref COMPONENT pcomp, int dwReserved);
            void GetDesktopItemCount(out int lpiCount, int dwReserved);
            void GetDesktopItem(int nComponent, ref COMPONENT pcomp, int
            dwReserved);
            void GetDesktopItemByID(IntPtr dwID, ref COMPONENT pcomp, int
            dwReserved);
            void GenerateDesktopItemHtml([MarshalAs(UnmanagedType.LPWStr)]
string pwszFileName, [In] ref COMPONENT pcomp, int dwReserved);
            void AddUrl(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)]
string pszSource, [In] ref COMPONENT pcomp, ADDURL dwFlags);
            void GetDesktopItemBySource([MarshalAs(UnmanagedType.LPWStr)]
string pwszSource, ref COMPONENT pcomp, int dwReserved);
        }

        [
        ComImport(),
        Guid("75048700-EF1F-11D0-9888-006097DEACF9")
        ]
        class ActiveDesktop /* : IActiveDesktop */ { }
        #endregion

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        public const string exts = ".jpg.jpeg.bmp.png";
        string[] denoms = new string[] { "Seconds", "Minutes", "Hours", "Days" };   //for interval settings
        int[] milidenoms = new int[] { 1000, 60000, 3600000, 8640000 };             //                                                        //
        Screen[] desktops = Screen.AllScreens; //array of all screens on system
        int totalwidth; //total width of all screens
        int allheight = 0;
        int usedwidth = 0;
        string pics = "";
        bool usedpic = true;
        int farthestleft = 0;
        bool selecting = false;
        stats stat;
        bool randompicking = false;
        List<picture> pictures = new List<picture>();
        List<string> dirpics;        
        ArrayList log = new ArrayList();
        bool useParse = false;
        string updateStats = "";
        bool userClose = false;
        public string progfilter = "";
        InputForm inputfrm;
        //directory dir;
        //picture lastpic;

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
                RegistryKey ourkey = Registry.CurrentUser;
                ourkey = ourkey.OpenSubKey(@"Software\Schraitle\Desktop");
                dirtb.Text = (string)ourkey.GetValue("dir");
                timernum.Value = decimal.Parse((string)ourkey.GetValue("interval"));
                startmintool.Checked = bool.Parse((string)ourkey.GetValue("startmin"));
                denombox.SelectedIndex = (int)ourkey.GetValue("denomindex");
                dualmon.Checked = bool.Parse((string)ourkey.GetValue("dualmon"));
                ratiobox.Value = decimal.Parse((string)ourkey.GetValue("ratio"));
                usebox.Value = decimal.Parse((string)ourkey.GetValue("use"));
                autostart.Checked = bool.Parse((string)ourkey.GetValue("autostart"));
                subdirs.Checked = bool.Parse((string)ourkey.GetValue("subdirs"));
                showtips.Checked = bool.Parse((string)ourkey.GetValue("balloon"));
                alwaysparse.Checked = bool.Parse((string)ourkey.GetValue("parse"));
                autoparse.Checked = bool.Parse((string)ourkey.GetValue("autoparse"));
                fade7.Checked = bool.Parse((string)ourkey.GetValue("fade7"));
                progfilter += (string)ourkey.GetValue("progfilter");
                ourkey.Close();
                RegistryKey startup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (startup.GetValue("SchDesktopSwitcher") != null)
                    winstart.Checked = true;
                startup.Close();
            }
            catch (Exception x) { MessageBox.Show("Error with the registry, either this is the first time you've run this program, or the program can't access your registry(UAC)\n\n" + x.ToString()); }
            log.Capacity = 50;
            if (progfilter == null)
                progfilter = "";
            if (startmintool.Checked)
                this.WindowState = FormWindowState.Minimized;
            if (useParse = alwaysparse.Checked)
                parseParse();
            else
            {
                autoparse.Checked = false;
                getdirpics();
            }
            if (autoparse.Checked)
                parsepics(false);
            getscreens();
            if (autostart.Checked)
                start_timer();
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new System.EventHandler(displaySettingsChanged);
            stat = new stats(dirtb.Text);
            inputfrm = new InputForm(this);
            //dir = new directory(dirtb.Text);
            //lastpic = dir.getpic(0);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            regsave();
            //dir.savefile();
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
            int wide = farthestleft;
            for (int i = 0; i < desktops.Length; i++)
            {
                desktops[i] = Screen.FromPoint(new Point(wide, 500));
                wide += desktops[i].Bounds.Width;
            }

            screenslist.DropDownItems.Clear();
            screenslist.DropDownItems.AddRange(new ToolStripItemCollection(menuStrip1, t));
        }

        private void regsave()
        {
            try
            {
                RegistryKey ourkey = Registry.CurrentUser;
                ourkey = ourkey.CreateSubKey(@"Software\Schraitle\Desktop");
                ourkey.OpenSubKey(@"Software\Schraitle\Desktop", true);
                ourkey.SetValue("dir", dirtb.Text);
                ourkey.SetValue("interval", timernum.Value);
                ourkey.SetValue("startmin", startmintool.Checked);
                ourkey.SetValue("denomindex", denombox.SelectedIndex);
                ourkey.SetValue("dualmon", dualmon.Checked);
                ourkey.SetValue("ratio", ratiobox.Value);
                ourkey.SetValue("use", usebox.Value);
                ourkey.SetValue("autostart", autostart.Checked);
                ourkey.SetValue("subdirs", subdirs.Checked);
                ourkey.SetValue("balloon", showtips.Checked);
                ourkey.SetValue("parse", alwaysparse.Checked);
                ourkey.SetValue("autoparse", autoparse.Checked);
                ourkey.SetValue("fade7", fade7.Checked);
                ourkey.SetValue("progfilter", progfilter);
                ourkey.Close();
            }
            catch (Exception x) { x.ToString(); }
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
        private bool sameratio(ref Bitmap b, int screen, double value)
        {
            double screenRatio = getratio(screen);
            double imgRatio = getratio(ref b);
            if (screenRatio < imgRatio)
                return ((screenRatio / imgRatio) + (double)(value / 100) >= 1);
            else
                return ((imgRatio / screenRatio) + (double)(value / 100) >= 1);
            //return (getratio(ref b) >= getratio(screen) - (value / 100 * getratio(screen)) && getratio(ref b) <= getratio(screen) + (value / 100 * getratio(screen)));
        }

        /// <summary>
        /// determines if ratio of bitmap is close enough to dimensions given
        /// </summary>
        /// <param name="b">bitmap to test</param>
        /// <param name="x">width dimension</param>
        /// <param name="y">height dimension</param>
        /// <returns></returns>
        private bool sameratio(ref Bitmap b, double x, double y, double value)
        {
            double screenRatio = x / y;
            double imgRatio = getratio(ref b);
            if (screenRatio < imgRatio)
                return ((screenRatio / imgRatio) + (double)(value / 100) >= 1);
            else
                return ((imgRatio / screenRatio) + (double)(value / 100) >= 1);
            //return (getratio(ref b) >= x/y - (value / 100 * x/y) && getratio(ref b) <= x/y + (value / 100 * x/y));
        }

        /// <summary>
        /// determines whether or not the ratio of the picture is close enough to the ratio of the screen
        /// </summary>
        /// <param name="b">bitmap to test</param>
        /// <param name="screen">index of the screen in the desktop array</param>
        /// <returns></returns>
        private bool sameratio(picture b, int screen, double value)
        {
            double screenRatio = getratio(screen);
            double imgRatio = (double)b.getwidth()/(double)b.getheight();
            if (screenRatio < imgRatio)
                return ((screenRatio / imgRatio) + (value / 100) >= 1);
            else
                return ((imgRatio / screenRatio) + (value / 100) >= 1);
            //return (getratio(ref b) >= getratio(screen) - (value / 100 * getratio(screen)) && getratio(ref b) <= getratio(screen) + (value / 100 * getratio(screen)));
        }

        /// <summary>
        /// determines if ratio of bitmap is close enough to dimensions given
        /// </summary>
        /// <param name="b">bitmap to test</param>
        /// <param name="x">width dimension</param>
        /// <param name="y">height dimension</param>
        /// <returns></returns>
        private bool sameratio(picture b, double x, double y, double value)
        {
            double screenRatio = x / y;
            double imgRatio = (double)b.getwidth() / (double)b.getheight();
            if (screenRatio < imgRatio)
                return ((screenRatio / imgRatio) + (value / 100) >= 1);
            else
                return ((imgRatio / screenRatio) + (value / 100) >= 1);
            //return (getratio(ref b) >= x/y - (value / 100 * x/y) && getratio(ref b) <= x/y + (value / 100 * x/y));
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

        private int getwidth(int given)
        {
            int screen = 0;
            int width = 0;

            if(given != 0)
                while (width < totalwidth - given)
                {
                    width += desktops[screen].Bounds.Width;
                    screen++;
                }
            width = desktops[screen].Bounds.Width;
            int height = desktops[screen].Bounds.Height;
            screen++;
            while (screen < desktops.Length)
            {
                if (desktops[screen].Bounds.Height == height)
                    width += desktops[screen].Bounds.Width;
                else
                    screen = desktops.Length;
                screen++;
            }
            return width;
        }

        /// <summary>
        /// changes wallpaper to random image
        /// </summary>
        /// <param name="use">file name and path to use for wallpaper, for random image, use ""</param>
        private void changepaper(string use)
        {
            addToLog("===========================================================");
            addToLog("Start: " + DateTime.Now.ToString());
            addToLog("===========================================================");
            string path = dirtb.Text + "\\Background.bmp";
            File.Delete(dirtb.Text + "\\Background.bmp");
            Bitmap final = new Bitmap(totalwidth, allheight);
            string file = use;
            //pictures.Clear();
            if (use == "")
            {
                file = getrandompic(getwidth(0), 0);
                randompicking = true;
            }
            //pictures.Add(file);
            pics = "Screen 1: " + file;
            if (randompicking && statenable.Checked)
                stat.addpicture(file);
            Bitmap b = new Bitmap(file);
            int i = 0;
            i += makepicture(ref final, ref b, i);
            while (i < desktops.Length)
            {
                string touse = "";
                if (selecting)
                    if (MessageBox.Show(pics + "\n\nChoose for screen " + (i+1).ToString(), "Custom", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        getpicdialog.InitialDirectory = dirtb.Text;
                        if (getpicdialog.ShowDialog() == DialogResult.OK)
                            touse = getpicdialog.FileName;
                        else
                            touse = use;
                    }
                    else
                        touse = use;
                else
                    if (dualmon.Checked && use == "")
                        touse = getrandompic(getwidth(totalwidth - usedwidth), i);
                    else
                        touse = file;

                Bitmap b2 = new Bitmap(touse);
                i += makepicture(ref final, ref b2, i);
                if (usedpic)
                {
                    pics += "\nScreen " + (i) + ": " + touse;
                    //pictures.Add(touse);
                    if(randompicking && statenable.Checked)
                        stat.addpicture(touse);
                }
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
                trayicon.ShowBalloonTip(10000,"Pictures", pics, ToolTipIcon.Info);
            final.Dispose();
            newfinal.Dispose();
            b.Dispose();
            if(randompicking && statenable.Checked)
                stat.savestats();
            randompicking = false;
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
        /// puts pictures from current directory into an arraylist
        /// </summary>
        private void getdirpics()
        {
            if (dirtb.Text.Length > 0)
            {
                dirpics = new List<string>();
                DirectoryInfo di = new DirectoryInfo(dirtb.Text);
                FileInfo[] all;
                if (subdirs.Checked)
                    all = di.GetFiles("*.*", SearchOption.AllDirectories);
                else
                    all = di.GetFiles();
                foreach (FileInfo f in all)
                    if (exts.Contains(f.Extension.ToLower()) && f.Extension != "" && f.FullName != (dirtb.Text + "\\Background.bmp"))
                        dirpics.Add(f.FullName);
            }
            else
                MessageBox.Show("There is no directory selected, please select one");
        }

        /// <summary>
        /// parses the parse.txt file in the selected directory, or goes to the regular system on failure
        /// </summary>
        private void parseParse()
        {
            pictures.Clear();
            try
            {
                TextReader r = new StreamReader(dirtb.Text + "\\parse.txt");
                string temp;
                while ((temp = r.ReadLine()) != null)
                    pictures.Add(new picture(temp, int.Parse(r.ReadLine()), int.Parse(r.ReadLine()), ""));
                r.Close();
                addToLog(pictures.Count.ToString() + " pictures in library");
            }
            catch (Exception x)
            {
                useParse = false;
                autoparse.Checked = false;
                getdirpics();
                MessageBox.Show("Error parsing parse.txt, using regular picture selection system\n" + x.Message);
            }
        }

        /// <summary>
        /// parses the height and width of each picture in current directory, as well as adding new pictures and deleting old ones
        /// </summary>
        /// <param name="complete">when true, will parse entire directory fresh and add everything</param>
        private void parsepics(bool complete)
        {
            if (complete)
                pictures.Clear();
            addToLog("Updating Library");
            getdirpics();
            List<string> name = new List<string>();
            foreach(string f in dirpics)
                name.Add(f);
            //search new names for old names and take them out of list
            //find old names that aren't in new names and delete them
            List<picture> toremove = new List<picture>();
            foreach(picture p in pictures)
            {
                int index;
                if ((index = name.IndexOf(p.getfilename())) == -1)
                    toremove.Add(p);
                else
                    name.RemoveAt(index);                
            }
            foreach (picture p in toremove)
                pictures.Remove(p);
            //add all remaining names to pictures
            Bitmap b = new Bitmap(1,1);
            pb1.Visible = true;
            pb1.Maximum = name.Count;
            pb1.Value = 0;
            foreach(string s in name)
            {
                b = new Bitmap(s);
                pictures.Add(new picture(s,b.Height, b.Width, ""));
                pb1.Value++;
                Application.DoEvents();
            }
            b.Dispose();
            TextWriter w = new StreamWriter(dirtb.Text + "\\parse.txt",false);
            foreach (picture p in pictures)
            {
                w.WriteLine(p.getfilename());
                w.WriteLine(p.getheight());
                w.WriteLine(p.getwidth());
            }
            w.Close();
            updateStats = name.Count.ToString() + " added\n";
            updateStats += toremove.Count.ToString() + " deleted";
            addToLog(updateStats);
            pb1.Visible = false;
        }

        /// <summary>
        /// goes through the steps to turn parsing off and use the regular system
        /// </summary>
        private void turnParseOff()
        {
            useParse = false;
            autoparse.Checked = false;
            alwaysparse.Checked = false;
            getdirpics();
        }

        /// <summary>
        /// gets random picture from directory based on given max width, if picture is close enough to max width that the scaling will be correct, picture is returned, even if the width is greater than maxwidth
        /// </summary>
        /// <param name="maxwidth">maximum picture width that is returned; any size = 0</param>
        private string getrandompic(int maxwidth, int screen)
        {
            bool ok = true;
            //ArrayList pics = dir.getlist();
            //picture temp;
            picture b;
            int fail = 0;
            int failpoint;
            if(useParse)
                failpoint = pictures.Count-1;
            else
                failpoint = dirpics.Count-1;
            //DateTime start = DateTime.Now;
            int c = 0;
            do
            {
                /* all of this is for color matching, to implement it, just uncomment and comment out the code below it
                c = new Random().Next(pics.Count);
                //temp = (FileInfo)pics[c];
                temp = (picture)pics[c];
                pics.RemoveAt(c);
               // MessageBox.Show(temp.FullName);
                label5.Text = pics.Count.ToString() + "  " + temp.getfilename();
                if (colormatching.Checked && temp.getcolorindex() == "0|0|0")
                    if (directory.getcolorindex(temp.getfilename()) != "0|0|0")
                    {
                        dir.removepic(temp);
                        dir.update(new picture(temp.getfilename()));
                    }
                Application.DoEvents();
                if (temp.getwidth() > maxwidth && !sameratio(temp.getwidth(), temp.getheight(), maxwidth, allheight))
                    ok = false;
                else if (pictures.Contains(temp.getfilename()))
                        ok = false;
                else if(colormatching.Checked && !directory.colorsmatch(lastpic.getcolorindex(), temp.getcolorindex(), (int)colorratio.Value))
                    ok = false;
                else
                    ok = true;
                //TimeSpan duration = start - DateTime.Now;
                if (maxwidth == 0 || fail == failpoint)// || duration.Seconds > 5)
                    ok = true;
                fail++;*/

                if (useParse)
                {
                    c = new Random().Next(pictures.Count);
                    b = (picture)pictures[c];
                }
                else
                {
                    c = new Random().Next(dirpics.Count);
                    b = new picture((string)dirpics[c]);
                }
                System.Threading.Thread.Sleep(1);
                int j = screen;
                int workingwidth = desktops[screen].Bounds.Width;
                int workingheight = desktops[screen].Bounds.Height;
                if (b.getwidth() > desktops[screen].Bounds.Width && !sameratio(b, screen, (double)usebox.Value) && desktops.Length > 1)
                {
                    int i = screen;
                    while (i < desktops.Length - 1 && b.getwidth() > workingwidth && !sameratio(b, workingwidth, workingheight, (double)usebox.Value))
                    {
                        i++;
                        j++;
                        workingwidth = widthofscreens(screen, i);
                    }
                    if (!sameratio(b, workingwidth, workingheight, (double)usebox.Value))
                        if (b.getwidth() >= workingwidth)
                        { }
                        else
                        {
                            workingwidth -= desktops[i].Bounds.Width;
                            j--;
                        }
                    else
                    { }
                    j = j - screen + 1;
                }
                else
                    j = 1;
                if(j == 1)
                    if (b.getwidth() > maxwidth && !sameratio(b, maxwidth, workingheight, (double)usebox.Value) || !sameratio(b, desktops[screen].Bounds.Width, desktops[screen].Bounds.Height, (double)usebox.Value))
                        ok = false;
                    else
                        ok = true;
                else
                    if (b.getwidth() > maxwidth && !sameratio(b, maxwidth, allheight, (double)usebox.Value))
                        ok = false;
                    else
                        ok = true;
                if (maxwidth == 0 && sameratio(b, maxwidth, allheight, (double)usebox.Value) || fail == failpoint)
                    ok = true;
                fail++;
                Application.DoEvents();
            } while (!ok);
            //addToLog(b..Name);
            addToLog("Screen " + screen + " Chose after " + fail.ToString() + " tries");

            //lastpic = temp;
            //return temp.getfilename();
            return b.getfilename();
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
        private int makepicture(ref Bitmap b, ref Bitmap picin, int screen)
        {
            int action = 0;
            int j = screen;
            int workingwidth = desktops[screen].Bounds.Width;
            int workingheight = desktops[screen].Bounds.Height;

            if (usedwidth >= widthofscreens(0, screen))  //if the screen has already been filled over, return the bitmap back unchanged
            {
                usedpic = false;
                return 1;
            }
            // if picture is sufficiently larger than the working screen, find how many more screens to go out to
            if (picin.Width > desktops[screen].Bounds.Width && !sameratio(ref picin, screen, (double)ratiobox.Value) && desktops.Length > 1)
            {
            int i = screen;
            while (i < desktops.Length - 1 && picin.Width > workingwidth && !sameratio(ref picin, workingwidth, workingheight, (double)ratiobox.Value))
                {
                    i++;
                    j++;
                    workingwidth = widthofscreens(screen, i);
                }
                if (!sameratio(ref picin, workingwidth, workingheight, (double)ratiobox.Value))
                    if (picin.Width >= workingwidth)
                        action = 0;
                    else
                    {
                        workingwidth -= desktops[i].Bounds.Width;
                        j--;
                    }
                else
                    action = 1;
                j = j - screen +1;
            }
            else
                j = 1;

            if (sameratio(ref picin, workingwidth, workingheight, (double)ratiobox.Value))
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
            return j;
        }

        /// <summary>
        /// performs events necessary to change wallpaper
        /// </summary>
        /// <param name="path">path of new wallpaper</param>
        /// <param name="tile">tile switch</param>
        /// <param name="style">style switch, 2 = centered</param>
        private void setwallpaper(string path, int tile, int style)
        {
            if(fade7.Checked)
            {
                IActiveDesktop ad = (IActiveDesktop)new ActiveDesktop();
                ad.SetWallpaper(path, 0);
                ad.ApplyChanges(AD_APPLY.ALL);
            }
            else
            {
                RegistryKey ourkey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
                ourkey.SetValue("Wallpaper", path);
                ourkey.SetValue("TileWallpaper", tile.ToString());
                ourkey.SetValue("WallpaperStyle", style.ToString());
                ourkey.Close();
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_SENDWININICHANGE);
            }

        }

        /// <summary>
        /// adds a new line to the output log
        /// </summary>
        /// <param name="toadd">text to add to log</param>
        private void addToLog(string toadd)
        {
            textlog.AppendText(toadd + "\n");
            textlog.ScrollToCaret();
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
            string[] progs = progfilter.Split(' ');
            bool allowgo = true;
            string beingused = "";
            foreach (string prog in progs)
            {
                Process[] pname = Process.GetProcessesByName(prog);
                if (pname.Length > 0)
                {
                    allowgo = false;
                    beingused = prog;
                    break;
                }
            }
            if(allowgo)
                changepaper("");
            else
                addToLog("didn't change because " + beingused + " is running");
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
                    //stat = new stats(dirtb.Text);
                    //label5.Text = "Parsing Directory (This could take a while)";
                    //dir.savefile();
                    //dir = new directory(dirtb.Text);
                    //label5.Text = "Messages";
                    if (System.IO.File.Exists(dirtb.Text + "\\parse.txt") && useParse)
                        parseParse();
                    else
                        turnParseOff();
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
            userClose = true;
            this.Close();
        }

        private void traychange_Click(object sender, EventArgs e)
        {
            changepaper("");
        }

        private void exit_Click(object sender, EventArgs e)
        {
            userClose = true;
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
            changepaper("");
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

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selecting = true;
            getpicdialog.InitialDirectory = dirtb.Text;
            if (getpicdialog.ShowDialog() == DialogResult.OK)
                changepaper(getpicdialog.FileName);
            selecting = false;
        }

        private void winstart_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey startup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (winstart.Checked)
                {
                    winstart.Checked = false;
                    startup.DeleteValue("SchDesktopSwitcher");
                }
                else
                {
                    winstart.Checked = true;
                    startup.SetValue("SchDesktopSwitcher", Application.ExecutablePath.ToString());
                }
                startup.Close();
            }
            catch (Exception x) { MessageBox.Show(x.ToString()); }
        }

        private void logshow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                textlog.Text = "";
            else
                if (logshow.Text == "Show Log")
                {
                    logshow.Text = "Hide Log";
                    this.Height = 386;
                    textlog.Visible = true;
                }
                else
                {
                    logshow.Text = "Show Log";
                    textlog.Visible = false;
                    this.Height = 170;                
                }
        }

        private void parseDirectoryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            parsepics(true);
        }

        private void updateLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parsepics(false);
        }

        private void alwaysparse_Click(object sender, EventArgs e)
        {
            if (alwaysparse.Checked)
            {
                useParse = false;
                getdirpics();
                alwaysparse.Checked = false;
                autoparse.Checked = false;
            }
            else
            {
                if (System.IO.File.Exists(dirtb.Text + "\\parse.txt"))
                    parseParse();
                parsepics(false);
                useParse = true;
                alwaysparse.Checked = true;
            }
        }

        private void updateLibraryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            parsepics(false);
            trayicon.ShowBalloonTip(10000, "", "Update Complete\n"+updateStats, ToolTipIcon.Info);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!userClose)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void programFilterTS_Click(object sender, EventArgs e)
        {
            inputfrm.progFilter();
        }
    }
}
