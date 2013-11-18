
namespace htaccessExtension.Models
{
    using System;
    using System.Reflection;
    using System.Text;
    using System.Web;

    public class ConversionManager
    {
        public ConversionManager()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("RewriteEngine On");
            builder.AppendLine("RewriteBase /myapp/");
            builder.AppendLine(@"RewriteRule ^index\.html$  welcome.html");

            Assembly iisRewriteClient = Assembly.Load("Microsoft.Web.Management.Rewrite.Client, Version=7.2.2.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            Type type = iisRewriteClient.GetType("Microsoft.Web.Management.Iis.Rewrite.Translation.Translator");

            MethodInfo translate = type.GetMethod("Translate");
            translate.Invoke(null, new object[] {builder.ToString(), false, 0});
        }


    }
}