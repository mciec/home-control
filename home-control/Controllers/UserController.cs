using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace home_control;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    //private readonly SignInManager<IdentityUser> _signInManager;

    public UserController(IUserService userService /*, SignInManager<IdentityUser> signInManager*/) : base()
    {
        this._userService = userService;
        //this._signInManager = signInManager;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        var response = _userService.TryLogin(request);

        if (response.Success == false)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, request.UserName),
            new Claim(ClaimTypes.Role, "Administrator"),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(300),
            IsPersistent = true,
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return Ok();

    }

    [HttpPost]
    [AllowAnonymous]
    [Route("loginExternal")]
    public IActionResult LoginExternal()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = @"https://localhost:7001/api/user/loginexternalcallback",
            Items =
                {
                    { "LoginProvider", "Google" },
                },
        };
        var x = Challenge(properties, GoogleDefaults.AuthenticationScheme);
        return x;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("loginexternalcallback")]
    public async Task<IActionResult> LoginExternalCallbackGet()
    {
        //var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
        //var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var authenticateResult = await HttpContext.AuthenticateAsync("External");

        if (!authenticateResult.Succeeded || authenticateResult?.Principal?.Identities is null)
        {
            return Unauthorized();
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticateResult.Principal);
        return Redirect("/");
    }

    [HttpGet]
    [Route("items")]
    public async Task<IEnumerable<int>> GetItems()
    {
        return new[] { 1, 2, 3 };
    }
}
