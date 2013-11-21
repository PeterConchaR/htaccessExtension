using htaccessExtension.Models;
using System.IO;
using System.Web.Mvc;

namespace htaccessExtension.Controllers
{
    public class HomeController : Controller
    {
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
            string exstWebConfig = "%Home%/site/wwwroot/Web.config";
            string webconfig = string.Empty;

            if(System.IO.File.Exists(exstWebConfig))
            {
                using (var asdf = new StreamReader(exstWebConfig))
                {
                    webconfig = asdf.ReadToEnd();
                }
            }

            string htaccess = new StreamReader(model.File.InputStream).ReadToEnd();

            string output = convert.GenerateOrUpdateWebConfig(
                webconfig,
                htaccess);

            return View(new HTAccessModel { HTAccessFile = htaccess, WebConfigFile = output });
        }
	}
}