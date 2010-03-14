using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Xml.Serialization;
using System.Windows.Forms;


//last modified: 3/13/10 7:13PM
namespace Utilities
{
    public class utilities
    {
        /*
         * this class can be transferred from program to program
         * just change the namespace at the top
         * give the constructor a program name which will be the folder inside the "Schraitle" appdata folder
         * */

        string appData;
        string name;
        string settingsFile;

        [Serializable]
        public struct KeyValuePair<K, V>
        {
            public K Key { get; set; }
            public V Value { get; set; }
        }

        /// <summary>
        /// sets up appdata saving
        /// </summary>
        /// <param name="progName">the name of the folder that this programs settings will be stored in</param>
        public utilities(string progName)
        {
            name = progName;
            getAppDataDir(progName);
            settingsFile = appData + "settings";
        }

        /// <summary>
        /// sets the appdata string and creates folders if necessary
        /// </summary>
        public string getAppDataDir(string progName)
        {
            if (!Directory.Exists(Environment.GetEnvironmentVariable("appdata") + "\\Schraitle"))
                Directory.CreateDirectory(Environment.GetEnvironmentVariable("appdata") + "\\Schraitle");
            if (!Directory.Exists(Environment.GetEnvironmentVariable("appdata") + "\\Schraitle\\" + progName))
                Directory.CreateDirectory(Environment.GetEnvironmentVariable("appdata") + "\\Schraitle\\" + progName);
            return appData = Environment.GetEnvironmentVariable("appdata") + "\\Schraitle\\" + progName + "\\";
        }

        /// <summary>
        /// gets settings from appdata folder and returs them in a hashtable(string,object)
        /// </summary>
        /// <returns>hashtable containing settings</returns>
        public Hashtable getSettings()
        {
            Hashtable toreturn = new Hashtable();
            List<KeyValuePair<string, object>> tempList = new List<KeyValuePair<string, object>>();
            if (File.Exists(settingsFile))
            {
                TextReader settingsReader = new StreamReader(settingsFile);
                XmlSerializer s = new XmlSerializer(typeof(List<KeyValuePair<string, object>>));
                tempList = (List<KeyValuePair<string, object>>)s.Deserialize(settingsReader);
                settingsReader.Close();
            }
            else
                File.Create(settingsFile).Close();
            foreach (KeyValuePair<string, object> k in tempList)
                toreturn[k.Key] = k.Value;
            return toreturn;
        }

        /// <summary>
        /// saves settings to appdata folder from a hashtable
        /// </summary>
        /// <param name="settings">hashtable containing settings in form(string, object)</param>
        public void saveSettings(Hashtable settings)
        {
            List<KeyValuePair<string, object>> sets = new List<KeyValuePair<string, object>>();
            foreach (string s in settings.Keys)
            {
                KeyValuePair<string, object> temp = new KeyValuePair<string,object>();
                temp.Key = s;
                temp.Value = settings[s];
                sets.Add(temp);
            }
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(List<KeyValuePair<string, object>>));
                TextWriter settingsWriter = new StreamWriter(settingsFile);
                s.Serialize(settingsWriter, sets);
                settingsWriter.Close();
            }
            catch (Exception x) { MessageBox.Show(x.ToString()); }
        }        
    }
}
