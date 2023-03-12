using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MDP.Line.Services
{
    public class LineController : Controller
    {
        // Fields                
        private readonly LineContext _lineContext;


        // Constructors
        public LineController(LineContext lineContext)
        {
            #region Contracts

            if (lineContext == null) throw new ArgumentException($"{nameof(lineContext)}=null");

            #endregion

            // Default
            _lineContext = lineContext;
        }


        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/Hook-Line", Name = "Hook-Line")]
        public async Task<ActionResult> Hook()
        {
            try
            {
                // Content
                var content = string.Empty;
                using (var reader = new StreamReader(this.Request.Body))
                {
                    content = await reader.ReadToEndAsync();
                }
                if (string.IsNullOrEmpty(content) == true) return this.BadRequest();

                // Signature 
                var signature = string.Empty;
                if (this.Request.Headers.TryGetValue("X-Line-Signature", out var signatureHeader) == true)
                {
                    signature = signatureHeader.FirstOrDefault();
                }
                if (string.IsNullOrEmpty(signature) == true) return this.BadRequest();

                // HandleHook
                _lineContext.HandleHook(content, signature);
            }
            catch (Exception ex)
            {
                // Display
                Console.WriteLine(ex.ToString());
            }

            // Return
            return this.Ok();
        }
    }
}
