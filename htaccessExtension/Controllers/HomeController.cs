using htaccessExtension.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.IO;

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

            string htaccess = convert.GenerateOrUpdateWebConfig(
                System.IO.File.OpenRead(Server.MapPath("~/TestFiles/Web.config")),
                model.File.InputStream);

            return View(new HTAccessModel(htaccess));
        }
	}
}