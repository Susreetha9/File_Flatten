using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Forms;

namespace FileFlatten.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlattenController : ControllerBase
    {
        [HttpPost("flatten")]
        public IActionResult FlattenPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                using (var inputMemoryStream = new MemoryStream())
                {
                    file.CopyTo(inputMemoryStream);
                    inputMemoryStream.Position = 0; // Reset the position to the beginning of the stream

                    using (var outputMemoryStream = new MemoryStream())
                    {
                        using (var reader = new PdfReader(inputMemoryStream))
                        using (var writer = new PdfWriter(outputMemoryStream))
                        {
                            PdfDocument pdfDoc = new PdfDocument(reader, writer);
                            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);

                            form.FlattenFields();
                            pdfDoc.Close(); // This will write the content to outputMemoryStream
                        }

                        // Convert the outputMemoryStream to an array and return the file
                        var outputBytes = outputMemoryStream.ToArray();
                        return File(outputBytes, "application/pdf", "flattened.pdf");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    
        }
}
