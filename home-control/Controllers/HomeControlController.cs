using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace home_control;

[Authorize]
[Route("api/[controller]")]
public class HomeControlController : ControllerBase
{
    public HomeControlController() : base()
    {
    }

    [Authorize]
    [HttpGet("items")]
    public async Task<List<string>> GetItems()
    {
        return new List<string>(){
            "aaa", "bbb"
        };
    }


}
