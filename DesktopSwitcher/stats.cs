using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace DesktopSwitcher
{
    class stats
    {
        List<string> pics = new List<string>();
        string file;

        public stats(string path)
        {
            file = path + "\\stats.csv";
            string stats = "";
            try
            {
                TextReader r = new StreamReader(file);
                stats = r.ReadToEnd();
                r.Close();
            }
            catch (Exception x) {
                if(x.Message.Contains("used by another process"))
                    if (MessageBox.Show("Please close any programs using stats.csv and press OK, or else your stats will be erased","Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        stats temp = new stats(path);
                        this.pics = temp.pics;
                        this.file = temp.file;
                    }
                return;
            }
            string[] statarr = stats.Split('\n');
            foreach (string s in statarr)
            {
                string[] temp = s.Split(',');
                if (temp.Length > 1)
                {
                    pics.Add(temp[0]);
                    pics.Add(temp[1]);
                }
            }
        }

        public void addpicture(string pic)
        {
            int index;
            pic = pic.Replace(",","");
            if ((index = pics.IndexOf(pic)) != -1)
                pics[index + 1] = (Int32.Parse(pics[index + 1]) + 1).ToString();
            else
            {
                pics.Add(pic);
                pics.Add("1");
            }
        }

        public void savestats()
        {
            try
            {
                TextWriter w = new StreamWriter(file);
                for (int i = 0; i < pics.Count; i++)
                {
                    w.Write(pics[i] + ",");
                    w.Write(pics[++i] + '\n');
                }
                w.Close();
            }
            catch (Exception x) { x.ToString(); }
        }

    }
}
