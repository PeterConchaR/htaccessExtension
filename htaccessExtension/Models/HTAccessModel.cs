namespace htaccessExtension.Models
{
    using System.ComponentModel;
    using System.Web;

    public class HTAccessModel
    {
        public HTAccessModel()
        {
            WebConfigFile = string.Empty;
            HTAccessFile = string.Empty;
            Path = string.Empty;
            AcceptTerms = false;
        }
        public string WebConfigFile { get; set; }
        public string HTAccessFile { get; set; }
        public string Path { get; set; }
        [DisplayName("Overwrite my web.config")]
        public bool AcceptTerms { get; set; }

        [DisplayName("Configuration file:")]
        public HttpPostedFileBase UploadedFile { get; set; }
    }
}