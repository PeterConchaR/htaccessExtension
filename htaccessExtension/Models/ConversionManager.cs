namespace htaccessExtension.Models
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Xml;
    using System.Xml.Linq;

    public class ConversionManager
    {
        protected const string _defaultWebConfig = "<?xml version=\"1.0\"?><configuration><system.webServer></system.webServer></configuration>";
        private Action<XmlWriter, object> WriteTo;
        private Func<string, bool, int, object> Translate;

        public ConversionManager()
        {
        }

        public string GenerateOrUpdateWebConfig(Stream webconfig, Stream htaccess)
        {
            string config = ReadFullFile(webconfig);
            string apache = ReadFullFile(htaccess);

            return GenerateOrUpdateWebConfig(config, apache);
        }

        public string GenerateOrUpdateWebConfig(string config, string htaccessFileContent)
        {
            bool addWebServerSection = false;
            if (String.IsNullOrWhiteSpace(config))
            {
                config = _defaultWebConfig;
            }

            var doc = XDocument.Parse(config);

            var webServerSection = FindSingle(doc, "system.webServer");
            if (webServerSection == default(XElement))
            {
                webServerSection = new XElement("system.webServer");
                addWebServerSection = true;
            }

            var rewriteSection = FindSingle(webServerSection, "rewrite");
            if (rewriteSection != default(XElement))
            {
                rewriteSection.Remove();
            }

            webServerSection.Add(GenerateRewriteSectionFromHTAccess(htaccessFileContent));

            if (addWebServerSection)
            {
                doc.Root.Add(webServerSection);
            }

            return doc.ToString();
        }

        private string GenerateRewriteSectionFromHTAccess(string fileContents)
        {
            var iisRewriteClient = Assembly.Load("Microsoft.Web.Management.Rewrite.Client, Version=7.2.2.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            CreateTranslateDelegate(iisRewriteClient);
            CreateRewriteDelegate(iisRewriteClient);

            string config = string.Empty;
            var result = Translate(fileContents, true, 0);

            using (var xmlString = new StringWriter())
            {
                using (var xw = XmlWriter.Create(xmlString))
                {
                    WriteTo(xw, result);
                }
                config = xmlString.ToString();
            }
            return config;
        }

        private string ReadFullFile(Stream file)
        {
            string contents = String.Empty;
            using (StreamReader sr = new StreamReader(file))
            {
                contents = sr.ReadToEnd();
            }
            return contents;
        }

        private static XElement FindSingle(XContainer container, string elementName)
        {
            return container.Descendants().SingleOrDefault(e => e.Name.LocalName == elementName);
        }
        //[ReflectionPermission(SecurityAction.Assert, Flags = ReflectionPermissionFlag.RestrictedMemberAccess)]
        private void CreateTranslateDelegate(Assembly client)
        {
            Type translator = client.GetType("Microsoft.Web.Management.Iis.Rewrite.Translation.Translator");
            MethodInfo translate = translator.GetMethod("Translate");
            Translate = (Func<string, bool, int, object>)Delegate.CreateDelegate(typeof(Func<string, bool, int>), translate);

        }

        //[ReflectionPermission(SecurityAction.Assert, Flags = ReflectionPermissionFlag.RestrictedMemberAccess)]
        private void CreateRewriteDelegate(Assembly client)
        {
            Type rewriteEntry = client.GetType("Microsoft.Web.Management.Iis.Rewrite.Translation.RewriteEntry");
            MethodInfo writeTo = rewriteEntry.GetMethod("WriteTo");
            WriteTo = (Action<XmlWriter, object>)Delegate.CreateDelegate(typeof(Action<XmlWriter, object>), writeTo);
        }
    }
}