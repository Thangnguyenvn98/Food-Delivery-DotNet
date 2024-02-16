using System.Security.Claims;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdminAuthentication.DependencyInjection.Extensions;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(builder.Configuration.GetValue<string>("FIREBASE_CONFIG"))
}
));

builder.Services.AddFirebaseAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (ClaimsPrincipal user) => "Hello World!");

app.MapPost("/verifyToken", async (string idToken) =>
{
    try
    {
        FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        var uid = decodedToken.Uid;
        // Optionally, retrieve additional user information or perform authorization checks
        return Results.Ok(new { UID = uid });
    }
    catch (FirebaseAuthException)
    {
        return Results.Unauthorized();
    }
});

app.Run();
