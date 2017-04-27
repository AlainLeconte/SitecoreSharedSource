namespace Sitecore.SharedSource.SmartTools.Dialogs
{
    //using Microsoft.ApplicationBlocks.Data;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Pages;
    using Sitecore.Web.UI.Sheer;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using Sitecore.Collections;
    using Sitecore.Globalization;
    using Sitecore.SecurityModel;
    using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
    using System.Net;
    using System.Web;
    using Sitecore.SharedSource.GoogleTranslate;

    public class AddVersionAndCopyDialog : DialogForm
    {
        protected Language sourceLanguage;
        protected bool CopySubItems;

        protected bool bGoogleTranslate;
        protected bool bUseProxy;
        protected string szProxy;
        protected int iPort;
        protected string szUser;
        protected string szPassword;
        protected string szDomain;

        protected Dictionary<string, string> langNames;
        protected Combobox Source;
        protected Literal TargetLanguages;
        protected Literal Options;
        protected TreeList TreeListOfItems;
        protected List<Language> targetLanguagesList;

        private void fillLanguageDictionary()
        {
            this.langNames = new Dictionary<string, string>();

            LanguageCollection languages;
            Database database = Context.ContentDatabase;
            languages = LanguageManager.GetLanguages(database);

            foreach (Language language in languages)
            {
                this.langNames.Add(language.CultureInfo.EnglishName, language.Name);//Danish - Key, da - Value
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                Assert.ArgumentNotNull(e, "e");
                base.OnLoad(e);
                if (!Context.ClientPage.IsEvent)
                {
                    Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);
                    ListItem child = new ListItem();
                    this.Source.Controls.Add(child);
                    CultureInfo info = new CultureInfo(Context.Request.QueryString["ci"]);
                    child.Header = info.EnglishName;
                    child.Value = info.EnglishName;
                    child.ID = Control.GetUniqueID("I");

                    if (itemFromQueryString == null)
                        throw new Exception();

                    // Target languages  
                    string str = "<script type='text/javascript'>";
                    str += "function toggleChkBoxMethod2(formName){var form=$(formName);var i=form.getElements('checkbox'); i.each(function(item){if (item.className=='reviewerCheckbox'){item.checked = !item.checked};});$('togglechkbox').checked = !$('togglechkbox').checked;}";
                    str += "function toggleChkBoxGoogle(){$('proxy').style.display = ($('chk_GoogleTranslate').checked == true)? 'table-row':'none';}";
                    str += "function toggleChkBoxProxy(){$('tabProxy').style.display = ($('chk_UseProxy').checked == true)? 'table-row':'none';}";
                    str += "</script>";
                    str += "<table><tr><td>Toggle All Checkboxes:</td><td>&nbsp;</td><td><input type='checkbox' id='togglechkbox' onClick=toggleChkBoxMethod2(ctl16); /></td></tr><tr><td colspan=3>&nbsp;</td></tr>";
                    this.fillLanguageDictionary();
                    foreach (KeyValuePair<string, string> pair in this.langNames)
                    {
                        if (itemFromQueryString.Language.Name != pair.Value)
                        {
                            string str2 = string.Format("chk_{0}", pair.Value);
                            string str4 = str;
                            str = str4 + "<tr><td>" + pair.Key + "</td><td>" + pair.Value + "</td><td><input class='reviewerCheckbox' type='checkbox' value='1' name='" + str2 + "'/></td></tr>";
                        }
                    }
                    str = str + "</table>";
                    this.TargetLanguages.Text = str;

                    //Options
                    str = "";
                    str += "<table>";
                    str += "  <tr>";
                    str += "    <td><input class='optionsCheckbox' type='checkbox' value='1' name='chk_IncludeSubItems' id='chk_IncludeSubItems'/></td>";
                    str += "    <td>Include SubItems</td>";
                    str += "  </tr>";
                    str += "  <tr>";
                    str += "    <td><input class='optionsCheckbox' type='checkbox' value='1' name='chk_GoogleTranslate' id='chk_GoogleTranslate' onClick=toggleChkBoxGoogle(); /></td>";
                    str += "    <td>Translate with Google (Only not shared text field)</td>";
                    str += "  </tr>";
                    str += "  <tr name='proxy' id='proxy' style='display: none; '>";
                    str += "    <td>&nbsp;</td>";
                    str += "    <td>";
                    str += "      <table>";
                    str += "        <tr>";
                    str += "          <td><input class='optionsCheckbox' type='checkbox' value='1' name='chk_UseProxy' id='chk_UseProxy' onClick=toggleChkBoxProxy(); /></td>";
                    str += "          <td>Set Proxy</td>";
                    str += "        </tr>";
                    str += "        <tr>";
                    str += "          <td>&nbsp</td>";
                    str += "          <td>";
                    str += "            <table id='tabProxy' name ='tabProxy' style='display: none; '>";
                    str += "              <tr>";
                    str += "                <td>Proxy :</td>";
                    str += "                <td><input class='proxy' type='text' name='txtProxy' value='proxysage.sagefr.adinternal.com' size=32/></td>";
                    str += "              </tr>";
                    str += "              <tr>";
                    str += "                <td>Port :</td>";
                    str += "                <td><input class='proxy' type='text' name='txtPort' value='8080' size=6/></td>";
                    str += "              </tr>";
                    str += "              <tr><td>User :</td><td><input class='proxy' ' type='text' name='txtUser' value='allec' size=8/></td></tr>";
                    str += "              <tr><td>Password :</td><td><input class='proxy' type='password' name='txtPassword' size=8/></td></tr>";
                    str += "              <tr><td>Domain :</td><td><input class='proxy' type='text' name='txtDomain' value='sagefr' size=8/></td></tr>";
                    str += "            </table>";
                    str += "          </td>";
                    str += "        </tr>";
                    str += "      </table>";
                    str += "    </td>";
                    str += "  </tr>";
                    str += "</table>";

                    this.Options.Text = str;
                }
            }
            catch (Exception exception)
            {
                Sitecore.Diagnostics.Log.Error(exception.Message, this);
            }
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Exception exception;
            Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);
            this.fillLanguageDictionary();
            targetLanguagesList = new List<Language>();
            try
            {
                //Get the source language
                if (itemFromQueryString == null)
                    throw new Exception();
                sourceLanguage = itemFromQueryString.Language;
                Sitecore.Diagnostics.Log.Debug("Smart Tools: OnOK-sourceLanguage-" + sourceLanguage.Name, this);

                //Get the target languages
                foreach (KeyValuePair<string, string> pair in this.langNames)
                {
                    if (!string.IsNullOrEmpty(Context.ClientPage.Request.Params.Get("chk_" + pair.Value)))
                    {
                        targetLanguagesList.Add(Sitecore.Data.Managers.LanguageManager.GetLanguage(pair.Value));
                    }
                }

                //Include SubItems?
                if (!string.IsNullOrEmpty(Context.ClientPage.Request.Params.Get("chk_IncludeSubItems")))
                {
                    CopySubItems = true;
                }
                Sitecore.Diagnostics.Log.Debug("Smart Tools: OnOK-CopySubItems-" + CopySubItems.ToString(), this);

                //Google Translate?
                if (!string.IsNullOrEmpty(Context.ClientPage.Request.Params.Get("chk_googleTranslate")))
                {
                    bGoogleTranslate = true;

                    if (!string.IsNullOrEmpty(Context.ClientPage.Request.Params.Get("chk_UseProxy")))
                    {
                        bUseProxy = true;
                        szProxy = Context.ClientPage.Request.Params.Get("txtProxy");
                        int.TryParse(Context.ClientPage.Request.Params.Get("txtPort"), out iPort);
                        szUser = Context.ClientPage.Request.Params.Get("txtUser");
                        szPassword = Context.ClientPage.Request.Params.Get("txtPassword");
                        szDomain = Context.ClientPage.Request.Params.Get("txtDomain");
                    }
                }
                Sitecore.Diagnostics.Log.Debug("Smart Tools: OnOK-GoogleTranslate-" + bGoogleTranslate.ToString(), this);

                //Execute the process
                if (itemFromQueryString != null && targetLanguagesList.Count > 0 && sourceLanguage != null)
                {
                    //Execute the Job
                    Sitecore.Shell.Applications.Dialogs.ProgressBoxes.ProgressBox.Execute("Add Version and Copy", "Smart Tools", new ProgressBoxMethod(ExecuteOperation), itemFromQueryString);
                }
                else
                {
                    //Show the alert
                    Context.ClientPage.ClientResponse.Alert("Context Item and Target Languages are empty.");
                    Context.ClientPage.ClientResponse.CloseWindow();
                }

                Context.ClientPage.ClientResponse.Alert("Process has been completed.");
                Context.ClientPage.ClientResponse.CloseWindow();
            }
            catch (Exception exception8)
            {
                exception = exception8;
                Sitecore.Diagnostics.Log.Error(exception.Message, this);
                Context.ClientPage.ClientResponse.Alert("Exception Occured. Please check the logs.");
                Context.ClientPage.ClientResponse.CloseWindow();
            }
        }

        protected void ExecuteOperation(params object[] parameters)
        {
            Sitecore.Diagnostics.Log.Debug("Smart Tools: Job Executed.", this);

            if (parameters == null || parameters.Length == 0)
                return;

            Item item = (Item)parameters[0];
            IterateItems(item, targetLanguagesList, sourceLanguage);
        }

        private void IterateItems(Item item, List<Language> targetLanguages, Language sourceLang)
        {
            AddVersionAndCopyItems(item, targetLanguages, sourceLang);

            if (CopySubItems && item.HasChildren)
            {
                foreach (Item childItem in item.Children)
                {
                    IterateItems(childItem, targetLanguages, sourceLang);
                }
            }
        }

        private void AddVersionAndCopyItems(Item item, List<Language> targetLanguages, Language sourceLang)
        {
            string _sourceLanguage = sourceLang.CultureInfo.EnglishName.Split(' ')[0];
            string _targetLanguage = null;

            foreach (Language language in targetLanguages)
            {
                Item source = Context.ContentDatabase.GetItem(item.ID, sourceLang);
                Item target = Context.ContentDatabase.GetItem(item.ID, language);

                if (source == null || target == null) return;

                _targetLanguage = language.CultureInfo.EnglishName.Split(' ')[0];

                Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItems-SourcePath-" + source.Paths.Path, this);
                Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItemsSourceLanguage-" + sourceLang.Name, this);
                Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItems-TargetLanguage-" + language.Name, this);

                source = source.Versions.GetLatestVersion();

                // If needed test googleTranslate
                if (bGoogleTranslate && _sourceLanguage != _targetLanguage)
                {
                    if (bUseProxy)
                        GoogleTranslate.ProxySettings.SetProxy(szProxy, iPort, szUser, szPassword, szPassword);
                    bool bProxyIsSet = GoogleTranslate.ProxySettings.IsSet;
                    bGoogleTranslate = (GoogleTranslate.Translate("English", "French", "Thank You") == "Merci");
                }

                target.Versions.AddVersion();
                target.Editing.BeginEdit();
                source.Fields.ReadAll();
                foreach (Field field in source.Fields)
                {
                    if (!field.Name.StartsWith("_")) //(!field.Shared)
                    {
                        target[field.Name] = source[field.Name];
                        if (bGoogleTranslate)
                        {
                            if (FieldTypeManager.GetField(field) is TextField && !field.Shared)
                            {
                                //if (bUseProxy)
                                //    GoogleTranslate.ProxySettings.SetProxy(szProxy, iPort, szUser, szPassword, szPassword);

                                target[field.Name] = Sitecore.SharedSource.GoogleTranslate.GoogleTranslate.Translate(
                                    _sourceLanguage,
                                    _targetLanguage,
                                    target[field.Name]
                                );
                            }
                        }

                    }
                }
                target.Editing.EndEdit();

                Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItems-Completed.", this);
            }
        }
    }
}

