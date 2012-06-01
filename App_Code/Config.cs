using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Schema;
using System.IO;
using System.Configuration;
using System.Windows.Forms;

namespace NzbSearcher
{
    public delegate void ConfigEvent();

    public static class Config<T>
    {
        public static T Value { get { return Config.GetValue<T>(); } }
    }

    public static class Config
    {
        static string _FileNameWithoutPath = "\\AppConfig.xml"; 
        
        static Dictionary<string, object> _ConfigDict = new Dictionary<string, object>();
        
        static bool _NoSave = false;

        /// <summary>
        /// executed when config is about to be saved to disk
        /// giving external classes a chance to store variables
        /// </summary>
        public static event ConfigEvent Saving;

        /// <summary>
        /// Indicates if config has been saved to disk yet (in other words, if this is a first-time launch)
        /// </summary>
        public static bool ConfigFileExists 
        { 
            get 
            {
                return File.Exists(Global.GetStorageDirectory(true) + _FileNameWithoutPath) ||
                        File.Exists(Global.GetStorageDirectory(false) + _FileNameWithoutPath);
            } 
        }

        /// <summary>
        /// loads config
        /// </summary>
        public static void Load()
        {
            string FileName = Global.GetStorageDirectory(true) + _FileNameWithoutPath;
            if (!File.Exists(FileName))
                FileName = Global.GetStorageDirectory(false) + _FileNameWithoutPath;
            if (!File.Exists(FileName))
                return;

            XmlDocument X = new XmlDocument();
            try 
            {
                X.Load(FileName); 
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Unable to load settings.\r\n\r\n" + e.Message, "NzbSearcher");
            }

#if !DEBUG
            bool bIncompatible = false;
#endif
            
            _ConfigDict.Clear();
            XmlNodeList Items = X.SelectNodes("/ConfigItems/item");
            foreach(XmlNode Item in Items)
            {
                string type = string.Empty;

                try
                {
                    object value = null;
                    type = Item.Attributes["type"].Value;

                    if (Item.Attributes.Count != 1) //only "type" attributes are allowed in the new config
                        throw new Exception("Incompatible Config");

                    Type SerializedType = Type.GetType(type);
                    XmlSerializer ValueSerializer = new XmlSerializer(SerializedType);
                    using (StringReader InnerStream = new StringReader(Item.InnerXml))
                        value = ValueSerializer.Deserialize(InnerStream);

                    Type ValueType = value.GetType();
                    if (!ValueType.FullName.StartsWith("NzbSearcher."))
                        throw new Exception("Incompatible Config");

                    _ConfigDict.Add(value.GetType().Name, value); 
                }
                catch (Exception)
                {
#if DEBUG
                    MessageBox.Show("Unable to load setting '" + type + "'");
#else
                    if (!bIncompatible)
                    {
                        bIncompatible = true;

                        if (Program.SplashForm != null)
                            Program.SplashForm.StopCloseTimer();

                        DialogResult res = MessageBox.Show("One or more settings loaded are incompatible with the current version of NzbSearcher. " +
                                                            "Do you want to continue loading, losing partially (or all) of your previously saved settings?", 
                                                            "NzbSearcher - WARNING",
                                                            MessageBoxButtons.YesNo,
                                                            MessageBoxIcon.Exclamation,
                                                            MessageBoxDefaultButton.Button2);
                        if (res != DialogResult.Yes)
                        {
                            _NoSave = true;
                            Global.ApplicationIsTerminating = true;
                            if (Program.SplashForm != null)
                                Program.SplashForm.Close();

                            return; //exit application
                        }
                        else if (Program.SplashForm != null)
                            Program.SplashForm.StartCloseTimer();
                    }
#endif
                }
            }
        }

        /// <summary>
        /// Save the configuration to disk
        /// </summary>
        public static void Save()
        {
            if (_NoSave)
                return;
            if (Saving != null)
                Saving();

            XmlDocument X = new XmlDocument();
            X.AppendChild(X.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlNode ConfigElm = X.CreateElement("ConfigItems");

            foreach (KeyValuePair<string, object> KeyValue in _ConfigDict)
            {
                try
                {
                    XmlNode ItemElm = X.CreateElement("item");
                    //XmlNode NameAttr = X.CreateAttribute("name");
                    XmlNode TypeAttr = X.CreateAttribute("type");

                    //NameAttr.Value = KeyValue.Key;
                    TypeAttr.Value = KeyValue.Value.GetType().FullName;
                    if (Type.GetType(TypeAttr.Value) == null) // if GetType can't resolve just by FullName, use AssemblyQualifiedName
                        TypeAttr.Value = KeyValue.Value.GetType().AssemblyQualifiedName;

                    //ItemElm.Attributes.SetNamedItem(NameAttr);
                    ItemElm.Attributes.SetNamedItem(TypeAttr);

                    XmlSerializer ValueSerializer = new XmlSerializer(KeyValue.Value.GetType());
                    StringBuilder SB = new StringBuilder();
                    using (XmlWriter xw = XmlWriter.Create(SB, new XmlWriterSettings() { OmitXmlDeclaration = true }))
                        ValueSerializer.Serialize(xw, KeyValue.Value);
                    ItemElm.InnerXml = SB.ToString();
                    ItemElm.FirstChild.Attributes.RemoveAll();

                    ConfigElm.AppendChild(ItemElm);
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("Unable to save setting '" + KeyValue.Key + "'.");
                }
            }

            X.AppendChild(ConfigElm);

            try
            {
                //delete from all possible storage locations
                string GlobalFileName = Global.GetStorageDirectory(true) + _FileNameWithoutPath;
                if (File.Exists(GlobalFileName))
                    File.Delete(GlobalFileName); 
                string LocalFileName = Global.GetStorageDirectory(false) + _FileNameWithoutPath;
                if (File.Exists(LocalFileName))
                    File.Delete(LocalFileName);

                //now actually save the file 
                Directory.CreateDirectory(Global.GetStorageDirectory());
                X.Save(Global.GetStorageDirectory() + _FileNameWithoutPath);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable to save settings to '" + Global.GetStorageDirectory() + _FileNameWithoutPath + "'.");
            }
        }

        /// <summary>
        /// Determines if a specified configuration key is found in the current config
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasValue(string key)
        {
            return _ConfigDict.ContainsKey(key);
        }

        /*
        /// <summary>
        /// Obtain configuration value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public object GetValue(string key, object DefaultValue)
        {
            if (HasValue(key))
            {
                object value = _ConfigDict[key];
                if (DefaultValue == null || value.GetType() == DefaultValue.GetType())
                    return value;
            }
            if (DefaultValue != null) //Doesn't exist yet, so create it
                SetValue(key, DefaultValue);
            return DefaultValue;
        }
        */

        public static T GetValue<T>()
        {
            return GetValue<T>(typeof(T).Name);
        }

        public static T GetValue<T>(string key)
        {
            if (HasValue(key))
            {
                object value = _ConfigDict[key];
                if (value is T)
                    return (T)value;
            }
            //Doesn't exist yet, so create it
            T NewValue = Activator.CreateInstance<T>();
            SetValue(key, NewValue);
            return NewValue;
        }

        /*
        public void SetValue(object Value)
        {
            if (Value != null)
                SetValue(Value.GetType().Name, Value);
        }
        */

        /// <summary>
        /// Set value in config data
        /// </summary>
        /// <param name="key"></param>
        /// <param name="Value"></param>
        private static void SetValue(string key, object Value)
        {
            _ConfigDict[key] = Value;
        }

        /// <summary>
        /// Remove a value from the configuration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="UserData"></param>
        public static void RemoveValue(string key)
        {
            if (_ConfigDict.ContainsKey(key))
                _ConfigDict.Remove(key);
        }
    }

    [XmlRoot("serializable_list")]
    public class SerializableList<T> : List<T>, IXmlSerializable
    {
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                string type = reader.GetAttribute("type");

                XmlSerializer TypeSerializer = new XmlSerializer(Type.GetType(type));

                reader.ReadStartElement("value");
                T value = (T)TypeSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (T value in this)
            {
                writer.WriteStartElement("item");
                writer.WriteAttributeString("type", value.GetType().FullName);

                XmlSerializer TypeSerializer = new XmlSerializer(value.GetType());

                writer.WriteStartElement("value");
                TypeSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion
    }

    public class StringIntDictionary : Dictionary<string, int>, IXmlSerializable
    {
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            do
            {
                string key = reader.GetAttribute("key");
                int value = int.Parse(reader.GetAttribute("value"));
                this.Add(key, value);
            }
            while (reader.Read() && reader.NodeType != System.Xml.XmlNodeType.EndElement);

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach(KeyValuePair<string, int> KeyValue in this)
            {
                if (KeyValue.Key != null)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("key", KeyValue.Key);
                    writer.WriteAttributeString("value", KeyValue.Value.ToString());
                    writer.WriteEndElement();
                }
            }
        }

        #endregion
    }


    [XmlRoot("serializable_dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion
    }




}