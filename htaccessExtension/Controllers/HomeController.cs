using htaccessExtension.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace htaccessExtension.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("RewriteEngine On");
            builder.AppendLine("RewriteBase /myapp/");
            builder.AppendLine(@"RewriteRule ^index\.html$  welcome.html");

            using (XmlWriter writer = XmlWriter.Create(@"C:\temp\web.config"))
            {
                Assembly iisRewriteClient = Assembly.Load("Microsoft.Web.Management.Rewrite.Client, Version=7.2.2.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                Type translator = iisRewriteClient.GetType("Microsoft.Web.Management.Iis.Rewrite.Translation.Translator");
                MethodInfo translate = translator.GetMethod("Translate");
                var result = translate.Invoke(null, new object[] { builder.ToString(), true, 0 });

                Type rewriteEntry = iisRewriteClient.GetType("Microsoft.Web.Management.Iis.Rewrite.Translation.RewriteEntry");
                MethodInfo writeTo = rewriteEntry.GetMethod("WriteTo");
                writeTo.Invoke(result, new object[] { writer });
            }

            return View();
        }
	}
}