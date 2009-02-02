using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace DesktopSwitcher
{
    class directory
    {
        List<picture> pics = new List<picture>();
        string file;
        string dir;

        /// <summary>
        /// sets the file for the index of the pictures in the directory and imports the index
        /// </summary>
        /// <param name="path"></param>
        public directory(string path)
        {
            dir = path;
            file = path + "\\pics.txt";
            import();
        }

        public void add(picture pic)
        {
            pics.Add(pic);
        }

        /// <summary>
        /// imports picture data from a text file with each line in the format: filename,height,width,colorindex
        /// </summary>
        public void import()
        {
            if (!File.Exists(file))
            {
                parsedirectory(dir);
                return;
            }
                
            string pictures;
            try
            {
                TextReader r = new StreamReader(file);
                pictures = r.ReadToEnd();
                r.Close();
            }
            catch (Exception x)
            {
                if (x.Message.Contains("used by another process"))
                    if (MessageBox.Show("Please close any programs using pics.txt and press OK, or else your picture data cache will be erased", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        directory temp = new directory(dir);
                        this.pics = temp.pics;
                        this.file = temp.file;
                    }
                return;
            }
            string[] picarr = pictures.Split('\n');
            foreach (string s in picarr)
            {                
                string[] temp = s.Split(',');
                if (temp.Length > 3)
                    pics.Add(new picture(temp[0], int.Parse(temp[1]), int.Parse(temp[2]), temp[3]));
            }
        }

        /// <summary>
        /// gets data from all pictures in given directory, and puts it in "pics.txt"
        /// </summary>
        /// <param name="path">the directory to parse</param>
        public void parsedirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] all = di.GetFiles();
            DateTime start = DateTime.Now;
            TimeSpan duration;

            TextWriter w = new StreamWriter(path + "\\pics.txt");
            pics = new List<picture>();
            for (int i = 0; i < all.Length; i++)
            {
                if (i % 100 == 0)
                    Application.DoEvents();
                if (Form1.exts.Contains(all[i].Extension.ToLower()) && all[i].Extension != "" && all[i].FullName != path + "\\Background.bmp")
                {
                    Bitmap b = new Bitmap(all[i].FullName);
                    string indx = getcolorindex(ref b);
                    add(new picture(all[i].FullName,b.Height,b.Width,indx));
                    w.WriteLine(all[i].FullName + "," + b.Height.ToString() + "," + b.Width.ToString() + "," + indx);
                    b.Dispose();
                }
            }
            w.Close();
            duration = DateTime.Now - start;
            MessageBox.Show("Color indexing complete\nduration: " + duration.Minutes.ToString() + ":" + duration.Seconds.ToString());
        }

        /// <summary>
        /// appends a new picture to "pics.txt" as well as memory
        /// </summary>
        /// <param name="pic">picture to add</param>
        public void update(picture pic)
        {
            add(pic);
        }

        /// <summary>
        /// given a list of filenames, if name doesn't already exist, add it
        /// </summary>
        /// <param name="file">list of filenames to add</param>
        public void updatelist(List<string> file)
        {
            List<string> names = new List<string>();
            foreach (picture p in pics)
                names.Add(p.getfilename());
            foreach (string s in file)
                if (!names.Contains(s))
                    add(new picture(s));
        }

        public static string getcolorindex(ref Bitmap pic)
        {
            string index = "";
            int r = 0;
            int g = 0;
            int b = 0;
            //int sample = 2096;
            int sample = (pic.Height * pic.Width / 200);
            for (int i = 0; i < sample; i++)
            {
                Color test = pic.GetPixel(new Random().Next(pic.Width), new Random().Next(pic.Height));
                r += (int)test.R;
                g += (int)test.G;
                b += (int)test.B;
            }
            r /= sample;
            g /= sample;
            b /= sample;

            index += r.ToString() + "|";
            index += g.ToString() + "|";
            index += b;

            return index;
        }

        /// <summary>
        /// finds average value of rgb values, given a random sampling of pixels in the image
        /// </summary>
        /// <param name="file">picture to index</param>
        /// <returns>returns index in the format of red x 1,000,000 + green x 1,000 + blue, or 0 if file is invalid</returns>
        public static string getcolorindex(string file)
        {
            Bitmap pic;
            pic = new Bitmap(file);
            string index = getcolorindex(ref pic);
            pic.Dispose();
            return index;
        }

        /// <summary>
        /// compares two pictures' color indexes
        /// </summary>
        /// <param name="pic1">original picture to be tested against</param>
        /// <param name="pic2">picture to test for equality</param>
        /// <param name="ratio">allowable ratio of color indexes</param>
        /// <returns>returns whether or not two pictures have equal-ish color indexes</returns>
        public static bool colorsmatch(string pic1, string pic2, int ratio)
        {
            string[] p1 = pic1.Split('|');
            string[] p2 = pic2.Split('|');
            float[] one = new float[3];
            float[] two = new float[3];
            for (int i = 0; i < 3; i++)
            {
                one[i] = float.Parse(p1[i]);
                two[i] = float.Parse(p2[i]);

                //if (getratio(ref b) >= getratio(screen) - ((double)ratiobox.Value / 100 * getratio(screen)) && getratio(ref b) <= getratio(screen) + ((double)ratiobox.Value / 100 * getratio(screen))) { }
                //if ((float)one[i] / (float)two[i] < (float)one[i] * (float)colorratio.Value && (float)one[i] / (float)two[i] > (float)one[i] * (float)colorratio.Value /100 > (float)two[i])
                if (two[i] >= one[i] - ((float)ratio / 100 * one[i]) && two[i] <= one[i] + ((float)ratio / 100 * one[i]))
                { }
                else
                    return false;
            }

            return true;
        }

        public void removepic(int index)
        {
            pics.RemoveAt(index);
        }

        public void removepic(picture toremove)
        {
            pics.RemoveAt(pics.IndexOf(toremove));
        }

        public ArrayList getlist()
        {
            ArrayList temp = new ArrayList();
            for (int i = 0; i < pics.Count; i++)
                temp.Add(pics[i]);
            return temp;
        }

        public void savefile()
        {
            TextWriter w = new StreamWriter(file);
            foreach (picture pic in pics)
                w.WriteLine(pic.getfilename() + "," + pic.getheight().ToString() + "," + pic.getwidth().ToString() + "," + pic.getcolorindex().ToString());
            w.Close();
        }

        public picture getpic(int i)
        {
            return pics[i];
        }
    }
}
