using System;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.SharedSource.LanguageMigrationTool.SitecoreProcessors
{
    public class CultureResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            try
            {
                if (Sitecore.Context.Language != null)
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture =
                        new System.Globalization.CultureInfo(Sitecore.Context.Language.Name);
                    System.Threading.Thread.CurrentThread.CurrentCulture =
                        System.Globalization.CultureInfo.CreateSpecificCulture(Sitecore.Context.Language.Name);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Sitecore.SharedSource.LanguageMigrationTool.SitecoreProcessors.CultureResolver. Details: " + ex.Message, this);
            }
        }
    }
}
