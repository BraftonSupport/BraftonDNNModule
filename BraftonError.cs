using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using Brafton.Modules.Globals;
namespace Brafton.BraftonError
{
    public class ErrorReporting
    {
        string DestURL;
        string key = "311rg6nunsim4rjjstsu5p3a";
        public Hashtable Settings;
        public static int Loop = 1;

        public ErrorReporting(Hashtable ModuleSettings)
        {
            Settings = ModuleSettings;
            DestURL = "http://updater.brafton.com/errorlog/dotnetnuke/error/" + key;
        }
        public void RemotePost(string error)
        {
            
            string url = string.Empty;
            MyGlobals.LogMessage("the dest url is " + DestURL, 1);
            HttpWebRequest report = (HttpWebRequest)WebRequest.Create(DestURL);
            report.Method = "POST";
            report.ContentType = "application/x-www-form-urlencoded";
            string jsonData = "error=" + EncodeError(error);
            string postData = jsonData;
            MyGlobals.LogMessage(" the error is " + postData, 1);
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            report.ContentLength = bytes.Length;

            Stream reportStream = report.GetRequestStream();
            reportStream.Write(bytes, 0, bytes.Length);
            WebResponse reportResponse = report.GetResponse();
            Stream reportResponseStream = reportResponse.GetResponseStream();
            StreamReader response = new StreamReader(reportResponseStream);
            var result = response.ReadToEnd();

            reportResponseStream.Dispose();
            response.Dispose();
            
        }
        private string EncodeError(string error)
        {
            var serializer = new JavaScriptSerializer();
            Dictionary<string, string> errorArray = new Dictionary<string, string>();
            errorArray.Add("Domain", Settings["clientDomain"].ToString());
            errorArray.Add("API", Settings["APIKey"].ToString());
            errorArray.Add("Brand", Settings["BrandUrl"].ToString());
            errorArray.Add("client_sys_time", DateTime.Now.ToString());
            errorArray.Add("error", error);
            var json = serializer.Serialize(errorArray);
            return json;
        }

        public static bool CheckErrors(string type)
        {
            bool value = true;
            Dictionary<string, string> acceptable = new Dictionary<string, string>();
            acceptable.Add("FormatException", "Format Issue most likely steming from an incorrect API, or Private Key");
            if (acceptable.ContainsKey(type))
            {
                MyGlobals.LogMessage(acceptable[type]);
                return false;
            }

            return value;
        }
    }
}