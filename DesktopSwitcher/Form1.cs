using System;
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

        public Form1()
        {
            InitializeComponent();
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
            }
        }

        /// <summary>
        /// checks given list of files for picture images (BMP, JPG, JPEG)
        /// </summary>
        /// <param name="files">the file list to check</param>
        private bool checkforpics(FileInfo[] files)
        {
            foreach(FileInfo f in files)
            {
                if (f.Extension == ".jpg" || f.Extension == ".JPG" || f.Extension == ".jpeg" || f.Extension == ".JPEG")
                    return true;
                if (f.Extension == ".bmp" || f.Extension == ".BMP")
                    return true;
            }
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
            string path;
            if(use != "")
                path = use;
            else
                path = 
            Bitmap b = new Bitmap()
            RegistryKey ourkey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            ourkey.SetValue("Wallpaper", "C:\\Documents and Settings\\David\\My Documents\\My Pictures\\Backgrounds\\Octopus.bmp");
            ourkey.Close();
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, "C:\\Documents and Settings\\David\\My Documents\\My Pictures\\Backgrounds\\Octopus.bmp", SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        /// <summary>
        /// gets random picture from directory
        /// </summary>
        private void getrandompic()
        {
        }
    }
}
