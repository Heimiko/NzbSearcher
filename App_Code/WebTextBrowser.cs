using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Web;
using System.Net;
using System.IO;

//Need addiotional references by project:
// System.Web

namespace Heimiko
{
    public class WebElement
    {
        protected string _Attributes = null;
        protected string _Content = null;

        public string Content { get { return _Content; } protected set { _Content = value; } }

        public WebElement GetElement(string ElementType, string AttributeCompare, string AttributeValue)
        {
            int StartIndex = 0;
            return GetElement(ElementType, AttributeCompare, AttributeValue, ref StartIndex);
        }

        /// <summary>
        /// WARNING: doesn't properly handle nested elements!
        /// </summary>
        public WebElement GetElement(string ElementType, string AttributeCompare, string AttributeValue, ref int StartIndex)
        {
            WebElement Elm = new WebElement();

            while(StartIndex >= 0 && StartIndex < _Content.Length)
            {
                StartIndex = _Content.IndexOf("<" + ElementType, StartIndex, StringComparison.OrdinalIgnoreCase);
                if (StartIndex < 0)
                    return null; // not found

                int StartOfAttrs = StartIndex = StartIndex + ElementType.Length + 1;
                char afterChar = _Content[StartOfAttrs];
                if (afterChar == ' ' || afterChar == '>') //we found a matching elemnt type
                {
                    int EndOfAttrs = StartIndex = _Content.IndexOf('>', StartIndex);
                    Elm._Attributes = _Content.Substring(StartOfAttrs, EndOfAttrs - StartOfAttrs);
                    string AttributeCompareValue = AttributeCompare != null ? Elm.GetAttributeValue(AttributeCompare) : null;
                    if (AttributeCompare == null || (AttributeValue == null ? AttributeCompareValue != null : AttributeValue == AttributeCompareValue))
                    {
                        if (Elm._Attributes.EndsWith("/"))
                        {
                            Elm._Content = string.Empty;
                        }
                        else
                        {
                            StartIndex = _Content.IndexOf("</" + ElementType + ">", EndOfAttrs);
                            if (StartIndex < 0)
                                StartIndex = _Content.Length - 1;

                            Elm._Content = _Content.Substring(EndOfAttrs + 1, StartIndex - EndOfAttrs - 1);
                        }
                        
                        return Elm; // we're done!
                    }
                }
            }
            
            return null;
        }

        public string GetAttributeValue(string Name)
        {
            if (_Attributes == null)
                return null;
            return GetAttributeValue(_Attributes, Name);
        }

        protected string GetAttributeValue(string ElementAttributes, string AttrName)
        {
            Match Attr = Regex.Match(ElementAttributes, " " + AttrName + " *= *\"([^\"]*)");
            if (Attr.Success)
                return Attr.Groups[1].Value;
            Attr = Regex.Match(ElementAttributes, " " + AttrName + " *= *([^ ]*)");
            return Attr.Success ? Attr.Groups[1].Value : null;
        }

        public Dictionary<string, string> GetComboValues(string ComboBoxName)
        {
            Dictionary<string, string> Values = new Dictionary<string, string>();
            Match ComboBox = Regex.Match(_Content.Replace('\n', ' '), "<select name=\"" + ComboBoxName + "\">(.*?)</select>", RegexOptions.IgnoreCase);
            if (ComboBox.Success)
            {
                Match ComboValue = Regex.Match(ComboBox.Groups[1].Value, "<option.*?value=\"([^\"]*)\">([^<]*)", RegexOptions.IgnoreCase);
                while (ComboValue.Success)
                {
                    Values.Add(ComboValue.Groups[2].Value.Trim(), ComboValue.Groups[1].Value);
                    ComboValue = ComboValue.NextMatch();
                }
            }

            return Values;
        }
    }


    public class WebTextBrowser : WebElement
    {
        CookieContainer _Cookies = new CookieContainer();

        //gets default timeout in milli-seconds
        public static int Timeout { get { return 30000; } }

        public CookieContainer Cookies { get { return _Cookies; } }
        public Uri CurrentUri { get; private set; }
        public string DocumentHTML { get { return _Content; } private set { _Content = value; } }

        public string Title
        {
            get
            {
                Match TitleMatch = Regex.Match(DocumentHTML, "<title>([^<]*)</title>");
                return TitleMatch.Success ? TitleMatch.Groups[1].Value : string.Empty;
            }
        }

        internal HttpWebRequest CreateRequest(string URL)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(URL);
            req.Timeout = Timeout;
            req.CookieContainer = _Cookies;
            return req;
        }

        internal void ReadRequestResponse(HttpWebRequest req)
        {
            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    using (Stream respStream = resp.GetResponseStream())
                        using (StreamReader reader = new StreamReader(respStream))
                            DocumentHTML = reader.ReadToEnd();

                    CurrentUri = resp.ResponseUri; //set URI which is currently assigned to the response
                }
            }
            catch (WebException)
            {
                //Page could not be loaded
                DocumentHTML = string.Empty;
                CurrentUri = req.RequestUri;
            }
        }

        public void Navigate(string URL)
        {
            ReadRequestResponse(CreateRequest(URL));
        }

        public WebForm[] GetWebForms()
        {
            List<WebForm> forms = new List<WebForm>();

            int FormStartPos = DocumentHTML.IndexOf("<form", StringComparison.OrdinalIgnoreCase);
            while (FormStartPos > 0)
            {
                int FormEndPos = DocumentHTML.IndexOf("form>", FormStartPos + 6, StringComparison.OrdinalIgnoreCase);
                if (FormEndPos < FormStartPos)
                    break;

                forms.Add(new WebForm(CurrentUri, DocumentHTML.Substring(FormStartPos, FormEndPos - FormStartPos)));
                FormStartPos = DocumentHTML.IndexOf("<form", FormEndPos + 6, StringComparison.OrdinalIgnoreCase);
            }

            return forms.ToArray();
        }

        public WebForm GetWebFormByAction(string Action)
        {
            foreach (WebForm form in GetWebForms())
            {
                if (form.Action.Contains(Action))
                    return form;
            }
            return null;
        }

        public WebForm GetWebFormByName(string Name)
        {
            foreach (WebForm form in GetWebForms())
            {
                if (form.Name == Name)
                    return form;
            }
            return null;
        }

        public string GetFullURL(string Relative)
        {
            return GetFullURL(CurrentUri, Relative);
        }

        public static string GetFullURL(Uri CurrentUri, string Relative)
        {
            string PortString = CurrentUri.IsDefaultPort ? string.Empty : ":" + CurrentUri.Port;

            if (Relative == null || Relative.Length == 0)
                return CurrentUri.ToString(); //no relative path, simply return full URL of current URI
            if (Relative.StartsWith("/"))
                return CurrentUri.Scheme + "://" + CurrentUri.Host + PortString + Relative;
            if (Relative.StartsWith("http"))
                return Relative; //already is a full path

            int Pos = CurrentUri.AbsolutePath.LastIndexOf('/');
            return CurrentUri.Scheme + "://" + CurrentUri.Host + PortString + CurrentUri.AbsolutePath.Substring(0, Pos + 1) + Relative;
        }
    }

    public class WebForm : WebElement
    {
        public string Name { get; private set; }
        public string Action { get; private set; }
        public string Method { get; private set; }

        public string FormHTML { get { return _Content; } private set { _Content = value; } }

        string _ActionFullURL;

        public Dictionary<string, string> Values { get; private set; }

        internal WebForm(string Method, string Action)
        {
            this.Action = _ActionFullURL = Action;
            this.Method = Method;
            this.Values = new Dictionary<string, string>();
        }

        internal WebForm(Uri CurrentUri, string FormHtml)
        {
            this.Values = new Dictionary<string, string>();
            this.FormHTML = FormHtml;

            Match Attr = Regex.Match(FormHtml, "<(form|input|select)( [^>]*)>", RegexOptions.IgnoreCase);
            while(Attr != null && Attr.Success)
            {
                string ElementType = Attr.Groups[1].Value;
                string ElementAttributes = Attr.Groups[2].Value;

                switch (Attr.Groups[1].Value) // element type
                {
                    case "form":
                        this.Name = GetAttributeValue(ElementAttributes, "name");
                        this.Action = GetAttributeValue(ElementAttributes, "action");
                        this.Method = GetAttributeValue(ElementAttributes, "method");
                        break;
                    case "input":
                    case "select":
                        string name = GetAttributeValue(ElementAttributes, "name");
                        if (name != null && name.Length > 0 && !Values.ContainsKey(name)) //only add variable if name not already present
                            Values.Add(name, GetAttributeValue(ElementAttributes, "value"));
                        break;
                }

                Attr = Attr.NextMatch();
            }

            _ActionFullURL = WebTextBrowser.GetFullURL(CurrentUri, Action); //resolve full URL now, because NOW we have the correct CurrentURI
        }

        string GetPostData()
        {
            string PostData = string.Empty;
            foreach (KeyValuePair<string, string> KeyValue in Values)
            {
                if (PostData.Length > 0)
                    PostData += "&";
                PostData += KeyValue.Key + "=" + HttpUtility.UrlEncode(KeyValue.Value);
            }
            return PostData;
        }

        public void Submit(WebTextBrowser browser)
        {
            //POST is the default method
            if (this.Method == null || this.Method.Length == 0 || this.Method.ToUpper() == "POST")
            {
                HttpWebRequest req = browser.CreateRequest(_ActionFullURL);

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                string postData = GetPostData();
                req.ContentLength = postData.Length;

                using (Stream ReqStream = req.GetRequestStream())
                    using (StreamWriter stOut = new StreamWriter(ReqStream, System.Text.Encoding.ASCII))
                        stOut.Write(postData);

                browser.ReadRequestResponse(req);
            }
            else //use GET method
            {
                browser.ReadRequestResponse(browser.CreateRequest(_ActionFullURL + "?" + GetPostData()));
            }
        }
    }

}
