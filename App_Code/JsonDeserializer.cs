using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heimiko
{
    // IMDB lists JSON example
    // {"lists":[{"name":"List Test","list_id":"C-ORlHoPdvk","state":"INPROGRESS"}],"status":200}

    /// <summary>
    /// Generic JSON deserializer, made by Heimiko
    /// </summary>
    public class JsonObject
    {
        private JsonObject() { /* no public constructor, call Deserialize() */ } 

        Dictionary<string, JsonObject> _Childs = new Dictionary<string,JsonObject>();

        public string Name { get; set; }
        public string Value { get; set; }
        public ICollection<JsonObject> Childs { get { return _Childs.Values; } }
        public JsonObject this[string name] { get { return _Childs[name]; } }

        enum DeserializeMode
        {
            None,
            ReadName,
            ReadValue
        }

        /// <summary>
        /// the one deserialize function you need to call
        /// </summary>
        /// <param name="JSON"></param>
        /// <returns></returns>
        public static JsonObject Deserialize(string JSON)
        {
            int pos = 0;
            return Deserialize(JSON, pos);
        }

        public static JsonObject Deserialize(string JSON, int pos)
        {
            JsonObject RootItem = new JsonObject();
            RootItem.Deserialize(JSON, ref pos);
            if (RootItem.Childs.Count == 1) //if rootitem has a single child, return this!
                return RootItem.Childs.ElementAt(0);
            return RootItem; //more than 1 root element? then return them as a collection
        }

        private void Deserialize(string JSON, ref int pos)
        {
            JsonObject NewChild = null;
            DeserializeMode Mode = DeserializeMode.ReadName;

            while (pos < JSON.Length)
            {
                char c = JSON[pos++]; 
                switch (c)
                {
                    case '[':
                    case '{': //starting new array or element
                        if (NewChild == null)
                        {
                            NewChild = new JsonObject();
                            NewChild.Name = Guid.NewGuid().ToString(); //create unique name
                        }
                        NewChild.Value = c == '[' ? "JsonArray" : "JsonObject"; //no real value here
                        NewChild.Deserialize(JSON, ref pos);
                        _Childs.Add(NewChild.Name, NewChild);
                        NewChild = null; //done with child
                        Mode = DeserializeMode.None;
                        break;
                    case ',': //about to have name
                        Mode = DeserializeMode.ReadName;
                        break;
                    case ':': //about to have the value
                        Mode = DeserializeMode.ReadValue;
                        break;

                    case '\r':
                    case '\n':
                    case ' ': // eat up these characters (spaces, enter, linefeeds, etc)
                        break;

                    case '}':
                    case ']':
                        return; // we're done with this array or item

                    default:
                        //read value, then store it in either the name or value
                        bool bHadQuote = false;
                        string ReadString = string.Empty;
                        pos--; //take 1 step back, start at the beginning
                        while (pos < JSON.Length)
                        {
                            char ch = JSON[pos++];
                            if (ch == ' ' && ReadString.Length == 0)
                            {
                                //eat up leading spaces (do nothing with this)
                            }
                            else if (ch == '\"')
                            {
                                bHadQuote = !bHadQuote;
                                if (!bHadQuote)
                                    break; //we're done (just closed quoted string)
                            }
                            else if (ch == '\\') //escape character?
                            {
                                if (char.TryParse("\\" + JSON[pos++], out ch))
                                    ReadString += ch;
                            }
                            else if (!bHadQuote && !char.IsLetterOrDigit(ch))
                            {
                                pos--; //take one step back
                                break; //we're done with this, since this appeared to be something without quotes
                            }
                            else
                                ReadString += ch;
                        }

                        //now we have the name/value string, store in whatever we're reading (mode)
                        switch(Mode)
                        {
                            case DeserializeMode.ReadName:
                                if (ReadString.Length > 0)
                                {
                                    NewChild = new JsonObject();
                                    NewChild.Name = ReadString;
                                }
                                break;
                            case DeserializeMode.ReadValue:
                                if (NewChild != null)
                                {
                                    NewChild.Value = ReadString;
                                    _Childs.Add(NewChild.Name, NewChild);
                                    NewChild = null; //done with current child
                                }
                                break;
                            }

                        Mode = DeserializeMode.None; // done with current mode, switch back to none
                        break;
                }
            }
        }


    }
}
