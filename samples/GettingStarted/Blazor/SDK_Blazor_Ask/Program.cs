// 1. Install the Nuclia SDK package
using Progress.Nuclia;
using Progress.Nuclia.Extensions;
using SDK_Blazor_Ask.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// 2. Add your secrets
// dotnet user-secrets set "ApiKey" "your-api-key-value"
// dotnet user-secrets set "ZoneId" "your-zone-id-value"
// dotnet user-secrets set "KnowledgeBoxId" "your-knowledge-box-id-value"

// 3. Create a NucliaDbConfig instance with your credentials
NucliaDbConfig config = new(ZoneId: builder.Configuration["ZoneId"]!,
	KnowledgeBoxId: builder.Configuration["KnowledgeBoxId"]!,
	ApiKey: builder.Configuration["ApiKey"]!);

// 4. Register the NucliaDbClient with DI
builder.Services.AddNucliaDb(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
