using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace htaccessExtension.Models
{
    public class HTAccessModel
    {
        public HTAccessModel() : this("")
        {

        }
        public HTAccessModel(string content)
        {
            FileContents = content;
        }
        public string FileContents { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}