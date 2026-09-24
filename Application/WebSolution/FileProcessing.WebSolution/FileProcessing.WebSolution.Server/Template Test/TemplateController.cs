using FileProcessing.Infrastructure.Email.Abstraction.EmailTemplateAbstraction;
using FileProcessing.Infrastructure.Email.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Razor.Templating.Core;
using static System.Net.Mime.MediaTypeNames;

namespace FileProcessing.WebSolution.Server.Template_Test
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController(ISignUpTemplateService TemplateService, IAppLogoProvider _AppLogoProvider) : ControllerBase
    {
        [HttpGet("testTemplate/")]
        [EnableRateLimiting("IP_Fixed")]
        public async Task<IActionResult> GetTemplate([FromBody] UserSignUpModel model)
        {
            byte[] LightLogo = await _AppLogoProvider.GetAppLightLogoAsync();
            byte[] DarkLogo = await _AppLogoProvider.GetAppDarkLogoAsync();

            model.LogoBaseLightString = $"data:image/png;base64,{Convert.ToBase64String(LightLogo)}";
            model.LogoBaseDarkString = $"data:image/png;base64,{Convert.ToBase64String(DarkLogo)}";


            var html = await TemplateService.RenderSignUpTemplateAsync(model);
            //var scheme = HttpContext.Request.Scheme;
            //var host = HttpContext.Request.Host;
            //https://localhost:5001/account/verify-email?token=...
            return Content(html, "text/html");
        }

        [HttpGet("toy/{toynumber:int}")]
        public IActionResult GetToys(int toynumber)
        {
            return Ok($"toy number {toynumber}");
        }
    }
}