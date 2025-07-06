using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace alquimia.Services.Models
{
    public class SendPDFToProvider
    {
        [FromForm]
        public string Email { get; set; } = string.Empty;

        /*= "Diseño de etiqueta.pdf";*/
        [FromForm]
        public string FileName { get; set; }

        [FromForm]
        public string CreatorName { get; set; }

        [FromForm]
        public string ProviderName { get; set; }

        [FromForm]
        public IFormFile File { get; set; }
    }
}
