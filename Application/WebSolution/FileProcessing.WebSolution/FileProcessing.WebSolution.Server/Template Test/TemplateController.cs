using FileProcessing.Core.Email.Abstraction.EmailTemplateAbstraction;
using FileProcessing.Core.Email.Models;
using Microsoft.AspNetCore.Mvc;
using Razor.Templating.Core;
using static System.Net.Mime.MediaTypeNames;

namespace FileProcessing.WebSolution.Server.Template_Test
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController(ISignUpTemplateService TemplateService, IAppLogoProvider _AppLogoProvider) : ControllerBase
    {
        [HttpGet("testTemplate/")]
        public async Task<IActionResult> GetTemplate([FromBody]UserSignUpModel model)
        {
            byte[] LightLogo = await _AppLogoProvider.GetAppLightLogoAsync();
            byte[] DarkLogo = await _AppLogoProvider.GetAppDarkLogoAsync();
            
            model.LogoBaseLightString = $"data:image/png;base64,{Convert.ToBase64String(LightLogo)}";
            model.LogoBaseDarkString = $"data:image/png;base64,{Convert.ToBase64String(DarkLogo)}";

            
            var html = await TemplateService.RenderSignUpTemplateAsync(model);
            return Content(html, "text/html");
        }

    }
}