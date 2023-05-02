Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.HttpsPolicy
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
Imports Microsoft.AspNetCore.Routing
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.FileProviders
Imports Microsoft.Extensions.Hosting

Public Class Startup
    Public ReadOnly Property Configuration As IConfiguration
    Public Shared GetEnv As IWebHostEnvironment

    Public Sub New(configuration As IConfiguration, env As IWebHostEnvironment)
        Me.Configuration = configuration
        GetEnv = env
    End Sub

    Public Sub New(env As IWebHostEnvironment)
        Dim build As New ConfigurationBuilder()
        build.SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json")
        Configuration = build.Build()
    End Sub

    ' This method gets called by the runtime. Use this method to add services to the container.
    Public Sub ConfigureServices(services As IServiceCollection)
        services.AddDistributedMemoryCache()
        services.AddSession()
        services.AddMvc()
        services.AddControllersWithViews(). ' Enable Vazor
            AddRazorRuntimeCompilation(
                 Sub(options) options.FileProviders.Add(New Vazor.VazorViewProvider())
            )
        services.Add(New ServiceDescriptor(GetType(SparePartsContext),
                                           New SparePartsContext(Configuration.GetConnectionString("DefaultConnection"))))
    End Sub

    ' This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    Public Sub Configure(app As IApplicationBuilder, env As IWebHostEnvironment)

        ' Let Vazor Compile the Shared Views
        Vazor.VazorSharedView.CreateAll()

        If (env.IsDevelopment()) Then
            app.UseDeveloperExceptionPage()
        Else
            app.UseExceptionHandler("/Home/Error")
            ' The default HSTS value Is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts()
        End If

        app.UseSession()

        app.UseHttpsRedirection()
        app.UseStaticFiles()

        app.UseRouting()
        app.UseAuthorization()

        app.UseEndpoints(
             Sub(routes As IEndpointRouteBuilder)
                 REM routes.MapControllers()
                 routes.MapControllerRoute(
                 name:="Accounts", pattern:="{controller=Accounts}/{action=Login}/{id?}"
                 )
                 routes.MapControllerRoute(
                 name:="Default", pattern:="{controller=Home}/{action=Index}/{id?}")
             End Sub)
    End Sub
End Class