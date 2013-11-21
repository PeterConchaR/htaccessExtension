namespace htaccessExtension.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web;

    public class HTAccessModel
    {
        public HTAccessModel()
        { }
        public string WebConfigFile { get; set; }
        public string HTAccessFile { get; set; }

        [DisplayName("Configuration file:")]
        public HttpPostedFileBase File { get; set; }
    }
}