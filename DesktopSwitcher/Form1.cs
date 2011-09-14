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
using Utilities;
using System.Net;
using System.Text.RegularExpressions;

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
        int[] milidenoms = new int[] { 1000, 60000, 3600000, 8640000 };             //
        List<Screen[]> desktops = new List<Screen[]>();// = Screen.AllScreens; //array of all screens on system
        int totalwidth; //total width of all screens
        int allheight = 0;
        int highest = 0;    //highest coordinate of all the screens
        int lowest = 0; //lowest coordinate of all the screens
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
        utilities Utilities;
      //  Bitmap overflow;    //used for the second half of pictures when they go over their screen
        //bool redo = false;
        //directory dir;
        //picture lastpic;

        public Form1()
        {
            try
            {
                desktops.Add(Screen.AllScreens);
                InitializeComponent();
            }
            catch (Exception x) { MessageBox.Show(x.ToString()); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            menuStrip1.Visible = true;
            dualmon.Checked = desktops[0].Length > 1;
            timer.Tick += new EventHandler(timer_Tick);
            trayicon.Icon = this.Icon;
            denombox.SelectedIndex = 1;
            Utilities = new utilities("DesktopSwitcher");
            getSettings();
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
            if(update.Checked)
                checkForUpdate();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            saveSettings();
        }

        /// <summary>
        /// counts number of screens and their sizes, displays them in menu bar, saves them in desktops array
        /// puts screens in array in physical order
        /// </summary>
        private void getscreens()
        {
            desktops[0] = (Screen[])Screen.AllScreens.Clone();
            farthestleft = 0;
            totalwidth = 0;
            allheight = 0;
            highest = 0;
            lowest = 0;
            ToolStripMenuItem[] t = new ToolStripMenuItem[desktops[0].Length];
            addToLog("getting::");
            for (int i = 0; i < desktops[0].Length; i++)
            {
                if (desktops[0][i].WorkingArea.X < farthestleft)
                    farthestleft = desktops[0][i].Bounds.X;
                totalwidth += desktops[0][i].Bounds.Width;
                if (desktops[0][i].WorkingArea.Bottom > lowest)
                    lowest = desktops[0][i].Bounds.Bottom;
                if (desktops[0][i].WorkingArea.Top < highest)
                    highest = desktops[0][i].Bounds.Top;
                addToLog("screen: " + i + "\n\tbottom: " + desktops[0][i].Bounds.Bottom + "\n\ttop: " + desktops[0][i].Bounds.Top);
            }
            //finds order of screens from left to right by finding lowest, adding it to desktops array and then discarding it to search for the rest
            int prevlow = farthestleft - 1;
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                int low = totalwidth;//x value of farthest left screen
                int y = 0;  //y value of farthest left screen
                for (int j = 0; j < Screen.AllScreens.Length; j++) //goes through all screens to find the lowest, ignoring any that have already been added
                {
                    int temp = Screen.AllScreens[j].WorkingArea.X;
                    if (temp < low && temp > prevlow)
                    {
                        low = temp;
                        y = Screen.AllScreens[j].WorkingArea.Y;
                    }
                }
                desktops[0][i] = Screen.FromPoint(new Point(low, y));
                prevlow = low;
            }
            for (int i = 0; i < desktops[0].Length; i++)
            {
                t[i] = new ToolStripMenuItem();
                ToolStripMenuItem[] props = new ToolStripMenuItem[3];
                for (int j = 0; j < props.Length; j++)
                    props[j] = new ToolStripMenuItem();
                t[i].Text = "Screen " + (i + 1).ToString();
                if (desktops[0][i].Primary)
                    t[i].Text += "(P)";
                props[0].Text = "Width:  " + desktops[0][i].Bounds.Width.ToString();
                props[1].Text = "Height: " + desktops[0][i].Bounds.Height.ToString();
                props[2].Text = "Ratio:  " + getratio(i).ToString();
                t[i].DropDownItems.AddRange(new ToolStripItemCollection(menuStrip1, props));
            }
            allheight = lowest - highest;

            screenslist.DropDownItems.Clear();
            screenslist.DropDownItems.AddRange(new ToolStripItemCollection(menuStrip1, t));
        }

        void getSettings()
        {
            Hashtable settings = Utilities.getSettings();
            try
            {
                if (settings.Contains("dir"))
                    dirtb.Text = (string)settings["dir"];
                if (settings.Contains("interval"))
                    timernum.Value = (decimal)settings["interval"];
                if (settings.Contains("startmin"))
                    startmintool.Checked = (bool)settings["startmin"];
                if (settings.Contains("denomindex"))
                    denombox.SelectedIndex = (int)settings["denomindex"];
                if (settings.Contains("dualmon"))
                    dualmon.Checked = (bool)settings["dualmon"];
                if (settings.Contains("ratio"))
                    ratiobox.Value = (decimal)settings["ratio"];
                if (settings.Contains("use"))
                    usebox.Value = (decimal)settings["use"];
                if (settings.Contains("autostart"))
                    autostart.Checked = (bool)settings["autostart"];
                if (settings.Contains("subdirs"))
                    subdirs.Checked = (bool)settings["subdirs"];
                if (settings.Contains("balloon"))
                    showtips.Checked = (bool)settings["balloon"];
                if (settings.Contains("parse"))
                    alwaysparse.Checked = (bool)settings["parse"];
                if (settings.Contains("autoparse"))
                    autoparse.Checked = (bool)settings["autoparse"];
                if (settings.Contains("fade7"))
                    fade7.Checked = (bool)settings["fade7"];
                if (settings.Contains("progfilter"))
                    progfilter += (string)settings["progfilter"];
                if (settings.Contains("stats"))
                    statenable.Checked = (bool)settings["stats"];
                if (settings.Contains("update"))
                    update.Checked = (bool)settings["update"];
                RegistryKey startup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (startup.GetValue("SchDesktopSwitcher") != null)
                    winstart.Checked = true;
                startup.Close();
            }
            catch (Exception x) { MessageBox.Show("Error with the save data, either this is the first time you've run this program, or the program can't access your apptata(UAC)\n\n" + x.ToString()); }         
        }

        private void saveSettings()
        {
            Hashtable settings = new Hashtable();
            settings["dir"] = dirtb.Text;
            settings["interval"] = timernum.Value;
            settings["startmin"] = startmintool.Checked;
            settings["denomindex"] = denombox.SelectedIndex;
            settings["dualmon"] = dualmon.Checked;
            settings["ratio"] = ratiobox.Value;
            settings["use"] = usebox.Value;
            settings["autostart"] = autostart.Checked;
            settings["subdirs"] = subdirs.Checked;
            settings["balloon"] = showtips.Checked;
            settings["parse"] = alwaysparse.Checked;
            settings["autoparse"] = autoparse.Checked;
            settings["fade7"] = fade7.Checked;
            settings["progfilter"] = progfilter;
            settings["stats"] = statenable.Checked;
            settings["update"] = update.Checked;
            Utilities.saveSettings(settings);
        }

        /// <summary>
        /// gets information about the screens
        /// </summary>
        /// <param name="popup">whether or not to display screen information after diagnostic</param>
        private void diagnostic(bool popup)
        {
            desktops[0] = Screen.AllScreens;
            getscreens();
            saveSettings();
            if (popup)
            {
                string done = " Screen(s)\n\nOrder:\n";
                //for (int i = 0; i < desktops[0].Length; i++)
                //    done += " | " + (i+1).ToString(); ;
                //done += " |";
                diagnosticForm diag = new diagnosticForm();
                diag.label1.Text = ("Done!\n\n" + desktops[0].Length.ToString() + done);
                diag.pictureBox1.Image = showVisual();
                diag.pictureBox1.Size = diag.pictureBox1.Image.Size;
                diag.Height = diag.label1.Height + 60 + diag.pictureBox1.Height;
                diag.Width = diag.pictureBox1.Width + 20;
                if (diag.Width < 250)
                    diag.Width = 250;
                diag.Show();
                //MessageBox.Show("Done!\n\n" + desktops[0].Length.ToString() + done);
            }
            //showVisual();
        }

        /// <summary>
        /// creates an image representing the screens and their size and position in relation to each other
        /// </summary>
        /// <returns>image showing where the screens are</returns>
        private Image showVisual()
        {
            float scale = 15;
            float offset = 5;
            float farleft = (float)farthestleft/scale*-1;
            float high = (float)highest/scale*-1;
            Bitmap world = new Bitmap((int)(((float)totalwidth / scale)+2*offset), (int)(((float)allheight / scale)+2*offset));
            Graphics g = Graphics.FromImage(world);
            Pen pen = new Pen(Color.White, 4);
            Brush pen2 = Brushes.Black;
            g.FillRectangle(pen2, 0, 0, (float)world.Width, (float)world.Height);
            Font font = new Font(FontFamily.GenericSansSerif, 15);
            for (int i = 0; i < desktops[0].Length; i++)
            {
                float width = (float)desktops[0][i].Bounds.Width / scale;
                float height = (float)desktops[0][i].Bounds.Height / scale;
                float x = farleft + ((float)desktops[0][i].WorkingArea.X / scale);
                float y = high +((float)desktops[0][i].WorkingArea.Y / scale);
                addToLog("screen: " + i + "\n\twidth: " + width + "\n\theight: " + height + "\n\tworkx: " + desktops[0][i].WorkingArea.X + "\n\tworky: " + desktops[0][i].WorkingArea.Y);
                g.DrawString((i+1).ToString(), font, Brushes.White, new PointF(x + width/2, y + height/2));
                g.DrawRectangle(pen, offset + x, offset + y, width, height);
            }
            g.Save();
            g.Dispose();
            return world;
            //world.Save("world.bmp");
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
            return (double)desktops[0][screen].Bounds.Width / (double)desktops[0][screen].Bounds.Height;
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
                    width += desktops[0][screen].Bounds.Width;
                    screen++;
                }
            width = desktops[0][screen].Bounds.Width;
            int height = desktops[0][screen].Bounds.Height;
            screen++;
            while (screen < desktops[0].Length)
            {
                if (desktops[0][screen].Bounds.Height == height)
                    width += desktops[0][screen].Bounds.Width;
                else
                    screen = desktops[0].Length;
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
            formWait(true);
            Application.DoEvents();
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
            if (!File.Exists(file))
            {
                final.Dispose();
                randompicking = false;
                addToLog("chosen picture doesn't exist, canceling operation (update library to weed out all missing files)");
                return;
            }
            //pictures.Add(file);
            pics = "Screen 1: " + file;
            if (randompicking && statenable.Checked)
                stat.addpicture(file);
            Bitmap b = new Bitmap(file);
            int i = 0;
            i += makepicture(ref final, ref b, i);
            while (i < desktops[0].Length)
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
                //if picture doesn't exist, get out of function, hopefully all things that should be set after function terminate are taken care of here
                if (!File.Exists(touse))
                {
                    final.Dispose();
                    b.Dispose();
                    usedwidth = 0;
                    randompicking = false;
                    addToLog("chosen picture doesn't exist, canceling operation (update library to weed out all missing files)");
                    return;
                }
                Bitmap b2 = new Bitmap(touse);
                //if (redo)
                //    b2 = overflow;
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
                    i = desktops[0].Length;
            }
            usedwidth = 0;

            //this section chops up image top and bottom and left and right for when primary screen isn't in a normal place
            Bitmap newfinal = new Bitmap(final.Width, final.Height);
            if (farthestleft < 0 || highest < 0)
            {
                if (highest < 0)
                {
                    Bitmap bottom = final.Clone(new Rectangle(0, (highest * -1), final.Width, final.Height - (highest * -1)), final.PixelFormat);
                    Bitmap top = final.Clone(new Rectangle(0, 0, final.Width, (highest * -1)), final.PixelFormat);

                    Graphics h;
                    h = Graphics.FromImage(newfinal);
                    h.DrawImage(bottom, 0, 0, bottom.Width, bottom.Height);
                    h.DrawImage(top, 0, bottom.Height, top.Width, top.Height);
                    h.Save();
                    h.Dispose();
                    bottom.Dispose();
                    top.Dispose();
                }
                if (farthestleft < 0)
                {
                    Bitmap cutUp = final;
                    if (highest < 0)
                        cutUp = newfinal;
                    Bitmap right = cutUp.Clone(new Rectangle((farthestleft * -1), 0, cutUp.Width - (farthestleft * -1), cutUp.Height), cutUp.PixelFormat);
                    Bitmap left = cutUp.Clone(new Rectangle(0, 0, (farthestleft * -1), cutUp.Height), cutUp.PixelFormat);

                    newfinal = new Bitmap(newfinal.Width, newfinal.Height);
                    Graphics h;
                    h = Graphics.FromImage(newfinal);
                    h.DrawImage(right, 0, 0, right.Width, right.Height);
                    h.DrawImage(left, right.Width, 0, left.Width, left.Height);
                    h.Save();
                    h.Dispose();
                    right.Dispose();
                    left.Dispose();
                    cutUp.Dispose();
                }
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
            formWait(false);
        }

        /// <summary>
        /// scales down bitmap to specified dimensions
        /// http://www.personalmicrocosms.com/Pages/dotnettips.aspx?c=24&t=50#tip;
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
                System.Threading.Thread.Sleep(10);
                int j = screen;
                int workingwidth = desktops[0][screen].Bounds.Width;
                int workingheight = desktops[0][screen].Bounds.Height;
                if (b.getwidth() > desktops[0][screen].Bounds.Width && !sameratio(b, screen, (double)usebox.Value) && desktops[0].Length > 1)
                {
                    int i = screen;
                    while (i < desktops[0].Length - 1 && b.getwidth() > workingwidth && !sameratio(b, workingwidth, workingheight, (double)usebox.Value))
                    {
                        i++;
                        j++;
                        workingwidth = widthofscreens(screen, i);
                    }
                    if (!sameratio(b, workingwidth, workingheight, (double)usebox.Value))
                        if (b.getwidth() > workingwidth)
                        { }
                        else
                        {
                            workingwidth -= desktops[0][i].Bounds.Width;
                            j--;
                        }
                    else
                    { }
                    j = j - screen + 1;
                }
                else
                    j = 1;
                if(j == 1)
                    if (b.getwidth() > maxwidth && !sameratio(b, maxwidth, workingheight, (double)usebox.Value) || !sameratio(b, desktops[0][screen].Bounds.Width, desktops[0][screen].Bounds.Height, (double)usebox.Value))
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
                temp += desktops[0][i].Bounds.Width;
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
            int workingwidth = desktops[0][screen].Bounds.Width;
            int workingheight = desktops[0][screen].Bounds.Height;

            if (usedwidth >= widthofscreens(0, screen))  //if the screen has already been filled over, return the bitmap back unchanged
            {
                usedpic = false;
                return 1;
            }
            // if picture is sufficiently larger than the working screen, find how many more screens to go out to
            if (picin.Width > desktops[0][screen].Bounds.Width && !sameratio(ref picin, screen, (double)ratiobox.Value) && desktops[0].Length > 1)
            {
                int i = screen;
                while (i < desktops[0].Length - 1 && picin.Width > workingwidth && !sameratio(ref picin, workingwidth, workingheight, (double)ratiobox.Value))
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
                        workingwidth -= desktops[0][i].Bounds.Width;
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
            int ypad = desktops[0][screen].WorkingArea.Y + (highest * -1);
            Bitmap temp = new Bitmap(workingwidth, allheight);
            //temp becomes image with dimensions of allheight x monitor width
            if (action == 0) //scaled (scaled up or down to fit in the closest screen bound)
            {
                if ((double)picin.Width / (double)workingwidth < (double)picin.Height / (double)workingheight)
                {
                    realheight = workingheight;
                    realwidth = (int)((double)picin.Width / ((double)picin.Height / (double)workingheight));
                    ypad += 0;
                    xpad = (workingwidth - realwidth) / 2;
                }
                else
                {
                    realheight = (int)((double)picin.Height / ((double)picin.Width / (double)workingwidth));
                    realwidth = workingwidth;
                    ypad += (workingheight - realheight) / 2;
                    xpad = 0;
                }
                drawPic(ref temp, ref picin, xpad, ypad, realwidth, realheight);
            }

            else if (action == 1)    //stretched (stretched slightly to fill the entire screen) 
            {                       //tiled (no change in size, picture just added in)
                drawPic(ref temp, ref picin, 0, ypad, workingwidth, workingheight);
            }

            Graphics h;
            h = Graphics.FromImage(b);
            h.DrawImage(temp, usedwidth, 0, temp.Width, temp.Height);
            //h.DrawImage(temp, usedwidth, heightfromtop[screen], workingwidth, workingheight);
            h.Save();
            h.Dispose();

            temp.Dispose();
            usedwidth += workingwidth;
            return j;
        }

        /// <summary>
        /// draws new picture onto original picture
        /// </summary>
        /// <param name="drawOn">picture that is being added to</param>
        /// <param name="image">picture that will be put on drawOn</param>
        /// <param name="xpad">x coordinate to start drawing</param>
        /// <param name="ypad">y coordinate to start drawing</param>
        /// <param name="width">width of picture to draw</param>
        /// <param name="height">height of picture to draw</param>
        private void drawPic(ref Bitmap drawOn, ref Bitmap image, int xpad, int ypad, int width, int height)
        {
            Graphics g;
            g = Graphics.FromImage(drawOn);
            Bitmap todraw = new Bitmap(1, 1);
            scale(ref image, ref todraw, width, height);
            g.DrawImage(todraw, xpad, ypad, width, height);
            todraw.Dispose();
            g.Save();
            g.Dispose();
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
                WALLPAPEROPT opts = new WALLPAPEROPT();
                opts.dwSize = System.Runtime.InteropServices.Marshal.SizeOf(opts);
                ad.GetWallpaperOptions(ref opts, 0);
                opts.dwStyle = WPSTYLE.TILE;
                ad.SetWallpaperOptions(ref opts, 0);
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
            textlog.SelectionStart = textlog.Text.Length;
            textlog.ScrollToCaret();
        }

        /// <summary>
        /// performs actions which make the window inactive and waiting
        /// </summary>
        /// <param name="waiting">true if waiting</param>
        private void formWait(bool waiting)
        {
            this.Enabled = !waiting;
            waitlbl.Visible = waiting;
        }

        //http://www.csharp-station.com/HowTo/HttpWebFetch.aspx
        /// <summary>
        /// attempts to connect to flat-line page and find out if there is a newer version or not
        /// </summary>
        private void checkForUpdate()
        {
            Stream resStream = Stream.Null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://schraitle.flat-line.net/programs/switcher/update.txt");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                resStream = response.GetResponseStream();
            }
            catch (Exception x)
            {
                x.ToString();
                return;
            }

            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            String[] update = sb.ToString().Split('\n');
            Regex finder = new Regex(@"(\d*)\.(\d*)\.(\d*)");
            MatchCollection matches = finder.Matches(update[0]);
            if (matches.Count > 0)
            {
                Version newOne = new Version(Int32.Parse(matches[0].Groups[1].Value), Int32.Parse(matches[0].Groups[2].Value), Int32.Parse(matches[0].Groups[3].Value));
                Version thisOne = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                if (thisOne.CompareTo(newOne) >= 0) { return; }
                if (MessageBox.Show(String.Format("There is an update available to version {0}.{1}.{2}\nWould you like to update?", new object[] { newOne.Major, newOne.Minor, newOne.Build }), "DesktopSwitcher Update!", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                Process.Start("http://schraitle.flat-line.net/programs/switcher/index.php");
            }
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
                saveSettings();
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
            if (desktops[0].Length < 2)
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
                    this.Height += (textlog.Height + 8);
                    textlog.Visible = true;
                }
                else
                {
                    logshow.Text = "Show Log";
                    textlog.Visible = false;
                    this.Height -= (textlog.Height + 8);
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
            if (!userClose && e.CloseReason != CloseReason.WindowsShutDown)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void programFilterTS_Click(object sender, EventArgs e)
        {
            inputfrm.progFilter();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Schraitle's Desktop Switcher\n" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "\n\nrubikscubist@gmail.com");
        }
    }
}
