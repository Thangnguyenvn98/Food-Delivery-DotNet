using System.Security.Claims;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdminAuthentication.DependencyInjection.Extensions;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using storecategoryapi;

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

app.MapPost("/categories", async (Category categoryrequest) => {
    var result = await StoreCategoryService.Add(categoryrequest);
    if (result == null) return Results.Problem("An error occured while trying to add the category to the store");
    return Results.Ok(result);

});

app.MapGet("/categories/{categoryId}", async (string categoryId) =>
{
    var result = await StoreCategoryService.GetById(categoryId);
    if (result == null) return Results.NotFound("category Not Found");
    return Results.Ok(result);
});

app.MapGet("/categories", async () =>
{
    var result = await StoreCategoryService.GetAll();
    return Results.Ok(result);
});

app.MapDelete("/categories/{categoryId}", async (string categoryId) =>
{
    var result = await StoreCategoryService.DeleteById(categoryId);
    if (result == null) return Results.Problem("An error as occured while trying to add the category");
    return Results.Ok(result);
});

app.MapPut("/categories/{categoryId}", async ([FromRoute]string categoryId, 
                        [FromBody]Category category) =>
{
    var result = await StoreCategoryService.Update(category, categoryId);
    if (result == null) return Results.Problem("An error as occured while trying to add the category");
    return Results.Ok(result);
});

app.Run();
