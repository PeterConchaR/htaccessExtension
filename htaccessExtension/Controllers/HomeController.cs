namespace htaccessExtension.Controllers
{
    using htaccessExtension.Models;
    using System;
    using System.IO;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        private readonly string WebConfigPath;

        public HomeController()
        {
            WebConfigPath = Environment.ExpandEnvironmentVariables(@"%HOME%\site\wwwroot\web.config");
        }

        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HTAccessModel model)
        {
            var convert = new ConversionManager();
            string exstWebConfig = WebConfigPath;
            string webconfig = string.Empty;

            if (System.IO.File.Exists(exstWebConfig))
            {
                using (var reader = new StreamReader(exstWebConfig))
                {
                    webconfig = reader.ReadToEnd();
                }
            }

            string htaccess = string.Empty;
            using (var reader = new StreamReader(model.UploadedFile.InputStream))
            {
                htaccess = reader.ReadToEnd();
            }

            string output = convert.GenerateOrUpdateWebConfig(webconfig, htaccess);

            return View(new HTAccessModel { HTAccessFile = htaccess, WebConfigFile = output, Path = exstWebConfig });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(HTAccessModel model)
        {
            if (model.AcceptTerms)
            {
                using (var stream = System.IO.File.CreateText(WebConfigPath))
                {
                    stream.Write(model.WebConfigFile);
                }
            }

            return RedirectToAction("Index");
        }
    }
}