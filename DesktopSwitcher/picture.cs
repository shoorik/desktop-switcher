using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DesktopSwitcher
{
    class picture
    {
        string filename;
        int height;
        int width;
        string colorindex;

        public picture(string path,int h, int w, string index)
        {
            filename = path;
            height = h;
            width = w;
            colorindex = index;
        }

        public picture(string file)
        {
            Bitmap b = new Bitmap(file);
            filename = file;
            height = b.Height;
            width = b.Width;
            colorindex = directory.getcolorindex(ref b);
            b.Dispose();
        }

        public string getfilename()
        {
            return filename;
        }

        public int getheight()
        {
            return height;
        }

        public int getwidth()
        {
            return width;
        }

        public string getcolorindex()
        {
            return colorindex;
        }
    }
}
