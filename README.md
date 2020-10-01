# Asp.Net Core 3 Web API & Swashbuckle Implementation
There are three main components to Swashbuckle:

* Swashbuckle.AspNetCore.Swagger: a Swagger object model and middleware to expose SwaggerDocument objects as JSON endpoints.

* Swashbuckle.AspNetCore.SwaggerGen: a Swagger generator that builds SwaggerDocument objects directly from your routes, controllers, and models. It's typically combined with the Swagger endpoint middleware to automatically expose Swagger JSON.

* Swashbuckle.AspNetCore.SwaggerUI: an embedded version of the Swagger UI tool. It interprets Swagger JSON to build a rich, customizable experience for describing the web API functionality. It includes built-in test harnesses for the public methods.

## Package installation
  Install-Package Swashbuckle.AspNetCore
  
## Add and configure Swagger middleware
    
    public void ConfigureServices(IServiceCollection services)
    {
      // Register the Swagger generator, defining 1 or more Swagger documents
      services.AddSwaggerGen();
    }
    public void Configure(IApplicationBuilder app)
    {
      // Enable middleware to serve generated Swagger as a JSON endpoint.
      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
      // specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
      {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
      });

      app.UseRouting();
      app.UseEndpoints(endpoints =>
      {
          endpoints.MapControllers();
      });
    }
    
To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
