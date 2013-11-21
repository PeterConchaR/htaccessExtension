namespace htaccessExtension.Models
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Xml;
    using System.Xml.Linq;

    public class ConversionManager
    {
        protected const string _defaultWebConfig = "<?xml version=\"1.0\"?><configuration><system.webServer></system.webServer></configuration>";

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

            webServerSection.Add(XElement.Parse(GenerateRewriteSectionFromHTAccess(htaccessFileContent)));

            if (addWebServerSection)
            {
                doc.Root.Add(webServerSection);
            }

            return doc.ToString();
        }

        private string GenerateRewriteSectionFromHTAccess(string fileContents)
        {
            var iisRewriteClient = Assembly.Load("Microsoft.Web.Management.Rewrite.Client, Version=7.2.2.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

            Type translator = iisRewriteClient.GetType("Microsoft.Web.Management.Iis.Rewrite.Translation.Translator");
            MethodInfo translate = translator.GetMethod("Translate");
            var result = translate.Invoke(null, new object[] { fileContents, true, 0 });

            Type rewriteEntry = iisRewriteClient.GetType("Microsoft.Web.Management.Iis.Rewrite.Translation.RewriteEntry");
            MethodInfo writeTo = rewriteEntry.GetMethod("WriteTo");

            string config = string.Empty;

            using (var xmlString = new StringWriter())
            {
                using (var xw = XmlWriter.Create(xmlString, new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    ConformanceLevel = ConformanceLevel.Fragment,
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    IndentChars = "\t"
                }))
                {
                    writeTo.Invoke(result, new object[] { xw });
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
    }
}