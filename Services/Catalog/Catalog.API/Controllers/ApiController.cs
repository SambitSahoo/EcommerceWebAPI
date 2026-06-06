using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{    
    [Asp.Versioning.ApiVersion("1")]
    [Route("api/v{version:apiversion}/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        
    }
}