using Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using ClientLibrary.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using ClientLibrary.Services.Interfaces;
using ClientLibrary.Services.Implementations;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor;
using Client.ApplicationStates;
using BaseLibrary.Entities;
using Syncfusion.Licensing;



var builder = WebAssemblyHostBuilder.CreateDefault(args);

SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["SyncfusionLicenseKey"]);
bool isValid = SyncfusionLicenseProvider.ValidateLicense(Platform.Blazor);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CustomHttpHandler>();
builder.Services.AddHttpClient("SystemApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7098");
}).AddHttpMessageHandler<CustomHttpHandler>();
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7098") });
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<GetHttpClient>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();

builder.Services.AddScoped<IGenericServiceInterface<GeneralDepartment>, GenericServiceImplementation<GeneralDepartment>>();
builder.Services.AddScoped<IGenericServiceInterface<Department>, GenericServiceImplementation<Department>>();
builder.Services.AddScoped<IGenericServiceInterface<Branch>, GenericServiceImplementation<Branch>>();

builder.Services.AddScoped<IGenericServiceInterface<Country>, GenericServiceImplementation<Country>>();
builder.Services.AddScoped<IGenericServiceInterface<City>, GenericServiceImplementation<City>>();

builder.Services.AddScoped<IGenericServiceInterface<Employee>, GenericServiceImplementation<Employee>>();

builder.Services.AddScoped<IGenericServiceInterface<MedicalLeave>, GenericServiceImplementation<MedicalLeave>>();

builder.Services.AddScoped<IGenericServiceInterface<Overtime>, GenericServiceImplementation<Overtime>>();
builder.Services.AddScoped<IGenericServiceInterface<OvertimeType>, GenericServiceImplementation<OvertimeType>>();

builder.Services.AddScoped<IGenericServiceInterface<Vacation>, GenericServiceImplementation<Vacation>>();
builder.Services.AddScoped<IGenericServiceInterface<VacationType>, GenericServiceImplementation<VacationType>>();


builder.Services.AddScoped<AppState>();

builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped<SfDialogService>();



await builder.Build().RunAsync();
