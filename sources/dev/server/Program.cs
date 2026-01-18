using Jarvis;

var builder = WebApplication.CreateBuilder(args);

// bind KeyVault and Foundry settings from configuration with validation
builder.Services
    .AddOptions<KeyVaultOptions>()
    .Bind(builder.Configuration.GetSection("KeyVault"))
    .Validate(o => !string.IsNullOrEmpty(o.VaultName), "KeyVault:VaultName is required")
    .Validate(o => !string.IsNullOrEmpty(o.KeyName), "KeyVault:KeyName is required")
    .ValidateOnStart();

builder.Services
    .AddOptions<FoundryOptions>()
    .Bind(builder.Configuration.GetSection("Foundry"))
    .Validate(o => !string.IsNullOrEmpty(o.Endpoint), "Foundry:Endpoint is required")
    .Validate(o => !string.IsNullOrEmpty(o.DeploymentName) || !string.IsNullOrEmpty(o.Model), "Foundry:DeploymentName or Model is required")
    .ValidateOnStart();

// add controllers support
builder.Services.AddControllers();

// register LLMService
builder.Services.AddSingleton<Jarvis.Services.LLMService>();

// register Azure Key Vault secret client using DefaultAzureCredential
var keyVaultName = builder.Configuration["KeyVault:VaultName"];
if (!string.IsNullOrEmpty(keyVaultName))
{
    var kvUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    builder.Services.AddSingleton(new Azure.Security.KeyVault.Secrets.SecretClient(kvUri, new Azure.Identity.DefaultAzureCredential()));
}

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// map controllers
app.MapControllers();

// test endpoint implemented in Controllers/TestController.cs as GET api/test/key

app.Run();
