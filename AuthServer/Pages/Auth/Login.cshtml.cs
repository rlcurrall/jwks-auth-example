using System.ComponentModel.DataAnnotations;
using System.Web;
using AuthServer.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServer.Pages.Auth;

public class LoginModel(
    IAuthenticationService authService,
    IRedirectUriValidator redirectUriValidator,
    IAuthCodeRepository authCodeRepository,
    ILogger<LoginModel> logger
) : PageModel
{
    [BindProperty]
    [Required]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Tenant { get; set; } = string.Empty;

    public string? ValidationError { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        var redirectUri = HttpContext.Session.GetString("RedirectUri");
        if (string.IsNullOrEmpty(redirectUri))
        {
            ErrorMessage =
                "No active authentication request. Please return to the application and try again.";
            return;
        }

        Tenant = HttpContext.Session.GetString("Tenant") ?? string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var state = HttpContext.Session.GetString("State");
        var redirectUri = HttpContext.Session.GetString("RedirectUri");
        var scopesValue = HttpContext.Session.GetString("Scopes");
        _ = HttpContext.Session.GetString("ClientId") ?? "default-client";
        var codeChallenge = HttpContext.Session.GetString("CodeChallenge");
        var codeChallengeMethod = HttpContext.Session.GetString("CodeChallengeMethod");

        if (string.IsNullOrEmpty(redirectUri))
        {
            ValidationError = "Missing redirect URI";
            return Page();
        }

        // Validate the redirect URI again (in case it was tampered with)
        if (!redirectUriValidator.Validate(redirectUri))
        {
            logger.LogWarning(
                "Invalid redirect URI detected in session during login: {RedirectUri}",
                redirectUri
            );
            ValidationError = "Invalid redirect URI";
            return Page();
        }

        var user = await authService.AttemptLogin(Username, Password, Tenant);
        if (user is null)
        {
            ValidationError = "Invalid username or password";
            return Page();
        }

        // Parse scopes from session, default to user_info if not provided
        var scopesList = string.IsNullOrEmpty(scopesValue)
            ? ["user_info"]
            : scopesValue.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        // Generate an authorization code
        var authCode = authCodeRepository.Create(
            user.Id,
            redirectUri,
            scopesList,
            codeChallenge,
            codeChallengeMethod
        );

        var uriBuilder = new UriBuilder(redirectUri);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        // Add the authorization code to the redirect URL
        query["code"] = authCode;

        if (!string.IsNullOrEmpty(state))
        {
            query["state"] = state;
        }

        uriBuilder.Query = query.ToString();

        // Clear session data
        HttpContext.Session.Remove("RedirectUri");
        HttpContext.Session.Remove("State");
        HttpContext.Session.Remove("Tenant");
        HttpContext.Session.Remove("Scopes");
        HttpContext.Session.Remove("ClientId");
        HttpContext.Session.Remove("CodeChallenge");
        HttpContext.Session.Remove("CodeChallengeMethod");

        logger.LogInformation(
            "User {Username} authenticated successfully. Redirecting to {RedirectUri} with authorization code",
            user.Username,
            redirectUri
        );

        return Redirect(uriBuilder.ToString());
    }
}
