using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using System.Web;
using System.Web.Script.Serialization;
using HtmlAgilityPack;

namespace Sitecore.SharedSource.GoogleTranslate
{
    public static class GoogleTranslate
    {

        #region Google Translate
        public static string Translate(string fromLanguage, string toLanguage, string content, string proxy = null, int? port = null, string User = null, string Domain = null)
        {

            if (fromLanguage == toLanguage)
                return content;

            try
            {
                // Create a Language mapping
                var languageMap = new Dictionary<string, string>();
                InitLanguageMap(languageMap);

                // Create an instance of WebClient in order to make the language translation
                Uri address = new Uri("https://translate.google.com/");
                WebClient wc = new WebClient();
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                //wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted);

                // Proxy and Credentials 
                //NetworkCredential nc = new NetworkCredential();
                //nc.UserName = "allec";
                //nc.Password = "Poi!1202";
                //nc.Domain = "sagefr";
                //WebProxy wp = new WebProxy("proxysage.sagefr.adinternal.com", 8080);
                //wp.Credentials = nc;
                //wc.Proxy = wp;
                //wc.Proxy = GetGoogleTranslateProxy();
                if (ProxySettings.IsSet)
                    wc.Proxy = ProxySettings.Settings;

                // Async Upload to the specified source i.e http://translate.google.com/ for handling the translation.
                //wc.UploadStringAsync(address, GetPostData(languageMap[fromLanguage], languageMap[toLanguage], content));
                // Sync ...
                string result = wc.UploadString(address, GetPostData(languageMap[fromLanguage], languageMap[toLanguage], content));
                var doc = new HtmlDocument();
                doc.LoadHtml(result);
                var node = doc.DocumentNode.SelectSingleNode("//span[@id='result_box']");
                var output = node != null ? WebUtility.HtmlDecode(node.InnerText) : null;
                return (string)output;
            }

            catch (WebException exw)
            {
                Sitecore.Diagnostics.Log.Error("WebError : /n{0}", exw.ToString());
                throw exw;
                //return content;
            }

            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error : /n{0}", ex.ToString());
                throw ex;
                //return content;
            }
        }

        /// <summary>
        /// Construct the Post data required for Google Translation
        /// </summary>
        /// <param name="fromLanguage"></param>
        /// <param name="toLanguage"></param>
        /// <returns></returns>
        static string GetPostData(string fromLanguage, string toLanguage, string content)
        {
            // Set the language translation. All we need is the language pair, from and to.
            string strPostData = string.Format("hl=en&ie=UTF8&oe=UTF8submit=Translate&langpair={0}|{1}",
                                                 fromLanguage,
                                                 toLanguage);

            // Encode the content and set the text query string param
            return strPostData += "&text=" + HttpUtility.UrlEncode(content);
        }

        #region Proxy
        public static class ProxySettings
        {
            static private WebProxy _settings;
            public static WebProxy Settings
            {
                get
                {
                    if (_settings == null)
                        _settings = GetGoogleTranslateProxy();
                    return _settings;
                }
            }


            public static bool IsSet
            {
                get { return (_settings != null); }
            }

            public static bool SetProxy(string proxy = null, int? port = null, string user = null, string password = null, string domain = null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(proxy) && port > 0)
                    {
                        _settings = new WebProxy(proxy, (int)port);

                        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
                        {
                            NetworkCredential nc = new NetworkCredential();
                            nc.UserName = user;
                            nc.Password = password;
                            nc.Domain = domain;
                            _settings.Credentials = nc;
                        }
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        // Use to deserialize sitecore item param...
        private class ProxyClass
        {
            public string Proxy { get; set; }
            public int Port { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
            public string Domain { get; set; }
        }

        // Get Google Translate proxy settings from Sitecore 
        private static WebProxy GetGoogleTranslateProxy()
        {
            var settingsItem = Sitecore.Context.ContentDatabase.GetItem("/sitecore/Shared/Common/Common Settings/Google Translate Proxy");
            string proxySettings = settingsItem != null ? settingsItem["Text"] : string.Empty;
            if (string.IsNullOrEmpty(proxySettings))
                return null;

            try
            {
                ProxyClass settings = new JavaScriptSerializer().Deserialize<ProxyClass>(proxySettings);
                if (settings != null)
                {
                    // Proxy and Credentials 
                    NetworkCredential nc = new NetworkCredential();
                    nc.UserName = settings.User;
                    nc.Password = settings.Password;
                    nc.Domain = settings.Domain;
                    WebProxy wp = new WebProxy(settings.Proxy, settings.Port);
                    wp.Credentials = nc;

                    return wp;
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("No proxy setting found: ", ex);
                return null;
            }
            return null;
        }
        #endregion Proxy


        static void InitLanguageMap(Dictionary<string, string> languageMap)
        {
            languageMap.Add("Afrikaans", "af");
            languageMap.Add("Albanian", "sq");
            languageMap.Add("Arabic", "ar");
            languageMap.Add("Armenian", "hy");
            languageMap.Add("Azerbaijani", "az");
            languageMap.Add("Basque", "eu");
            languageMap.Add("Belarusian", "be");
            languageMap.Add("Bengali", "bn");
            languageMap.Add("Bulgarian", "bg");
            languageMap.Add("Catalan", "ca");
            languageMap.Add("Chinese", "zh-CN");
            languageMap.Add("Croatian", "hr");
            languageMap.Add("Czech", "cs");
            languageMap.Add("Danish", "da");
            languageMap.Add("Dutch", "nl");
            languageMap.Add("English", "en");
            languageMap.Add("Esperanto", "eo");
            languageMap.Add("Estonian", "et");
            languageMap.Add("Filipino", "tl");
            languageMap.Add("Finnish", "fi");
            languageMap.Add("French", "fr");
            languageMap.Add("Galician", "gl");
            languageMap.Add("German", "de");
            languageMap.Add("Georgian", "ka");
            languageMap.Add("Greek", "el");
            languageMap.Add("Haitian Creole", "ht");
            languageMap.Add("Hebrew", "iw");
            languageMap.Add("Hindi", "hi");
            languageMap.Add("Hungarian", "hu");
            languageMap.Add("Icelandic", "is");
            languageMap.Add("Indonesian", "id");
            languageMap.Add("Irish", "ga");
            languageMap.Add("Italian", "it");
            languageMap.Add("Japanese", "ja");
            languageMap.Add("Korean", "ko");
            languageMap.Add("Lao", "lo");
            languageMap.Add("Latin", "la");
            languageMap.Add("Latvian", "lv");
            languageMap.Add("Lithuanian", "lt");
            languageMap.Add("Macedonian", "mk");
            languageMap.Add("Malay", "ms");
            languageMap.Add("Maltese", "mt");
            languageMap.Add("Norwegian", "no");
            languageMap.Add("Persian", "fa");
            languageMap.Add("Polish", "pl");
            languageMap.Add("Portuguese", "pt");
            languageMap.Add("Romanian", "ro");
            languageMap.Add("Russian", "ru");
            languageMap.Add("Serbian", "sr");
            languageMap.Add("Slovak", "sk");
            languageMap.Add("Slovenian", "sl");
            languageMap.Add("Spanish", "es");
            languageMap.Add("Swahili", "sw");
            languageMap.Add("Swedish", "sv");
            languageMap.Add("Tamil", "ta");
            languageMap.Add("Telugu", "te");
            languageMap.Add("Thai", "th");
            languageMap.Add("Turkish", "tr");
            languageMap.Add("Ukrainian", "uk");
            languageMap.Add("Urdu", "ur");
            languageMap.Add("Vietnamese", "vi");
            languageMap.Add("Welsh", "cy");
            languageMap.Add("Yiddish", "yi");
        }

        #endregion Google Translate
    }
}