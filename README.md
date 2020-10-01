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
    
To support backwards compatibility, exposing JSON in the 2.0 format
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = true;
    });

## API info and description
Using the OpenApiInfo class, modify the information displayed in the UI:
      // Register the Swagger generator, defining 1 or more Swagger documents
      services.AddSwaggerGen(c =>
      {
          c.SwaggerDoc("v1", new OpenApiInfo
          {
              Version = "v1",
              Title = "ToDo API",
              Description = "A simple example ASP.NET Core Web API",
              TermsOfService = new Uri("https://example.com/terms"),
              Contact = new OpenApiContact
              {
                  Name = "Shayne Boyer",
                  Email = string.Empty,
                  Url = new Uri("https://twitter.com/spboyer"),
              },
              License = new OpenApiLicense
              {
                  Name = "Use under LICX",
                  Url = new Uri("https://example.com/license"),
              }
          });
      });

## XML comments
XML comments can be enabled with the following approaches:

      <PropertyGroup>
        <GenerateDocumentationFile>true</GenerateDocumentationFile>
        <NoWarn>$(NoWarn);1591</NoWarn>
      </PropertyGroup>

Enabling XML comments provides debug information for undocumented public types and members. Undocumented types and members are indicated by the warning message. For example, the following message indicates a violation of warning code 1591:

      warning CS1591: Missing XML comment for publicly visible type or member 'TodoController.GetAll()'

To suppress warnings project-wide, define a semicolon-delimited list of warning codes to ignore in the project file. Appending the warning codes to $(NoWarn); applies the C# default values too.

      <PropertyGroup>
        <GenerateDocumentationFile>true</GenerateDocumentationFile>
        <NoWarn>$(NoWarn);1591</NoWarn>
      </PropertyGroup>

To suppress warnings only for specific members, enclose the code in #pragma warning preprocessor directives. This approach is useful for code that shouldn't be exposed via the API docs. In the following example, warning code CS1591 is ignored for the entire Program class. Enforcement of the warning code is restored at the close of the class definition. Specify multiple warning codes with a comma-delimited list.

      namespace TodoApi
      {
      #pragma warning disable CS1591
          public class Program
          {
              public static void Main(string[] args) =>
                  BuildWebHost(args).Run();

              public static IWebHost BuildWebHost(string[] args) =>
                  WebHost.CreateDefaultBuilder(args)
                      .UseStartup<Startup>()
                      .Build();
          }
      #pragma warning restore CS1591
      }

Configure Swagger to use the XML file that's generated with the preceding instructions. For Linux or non-Windows operating systems, file names and paths can be case-sensitive. For example, a TodoApi.XML file is valid on Windows but not CentOS.

      public void ConfigureServices(IServiceCollection services)
      {
          services.AddDbContext<TodoContext>(opt =>
              opt.UseInMemoryDatabase("TodoList"));
          services.AddControllers();

          // Register the Swagger generator, defining 1 or more Swagger documents
          services.AddSwaggerGen(c =>
          {
              c.SwaggerDoc("v1", new OpenApiInfo
              {
                  Version = "v1",
                  Title = "ToDo API",
                  Description = "A simple example ASP.NET Core Web API",
                  TermsOfService = new Uri("https://example.com/terms"),
                  Contact = new OpenApiContact
                  {
                      Name = "Shayne Boyer",
                      Email = string.Empty,
                      Url = new Uri("https://twitter.com/spboyer"),
                  },
                  License = new OpenApiLicense
                  {
                      Name = "Use under LICX",
                      Url = new Uri("https://example.com/license"),
                  }
              });

              // Set the comments path for the Swagger JSON and UI.
              var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
              var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
              c.IncludeXmlComments(xmlPath);
          });
      }
In the preceding code, Reflection is used to build an XML file name matching that of the web API project. The AppContext.BaseDirectory property is used to construct a path to the XML file. Some Swagger features (for example, schemata of input parameters or HTTP methods and response codes from the respective attributes) work without the use of an XML documentation file. For most features, namely method summaries and the descriptions of parameters and response codes, the use of an XML file is mandatory.


Adding triple-slash comments to an action enhances the Swagger UI by adding the description to the section header. Add a <summary> element above the Delete action:
  
        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>        
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
          var todo = _context.TodoItems.Find(id);

          if (todo == null)
          {
              return NotFound();
          }

          _context.TodoItems.Remove(todo);
          _context.SaveChanges();

          return NoContent();
      }


