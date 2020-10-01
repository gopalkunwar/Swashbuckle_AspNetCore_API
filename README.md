# Asp.Net Core 3 Web API & Swashbuckle Implementation
There are three main components to Swashbuckle:

* Swashbuckle.AspNetCore.Swagger: a Swagger object model and middleware to expose SwaggerDocument objects as JSON endpoints.

* Swashbuckle.AspNetCore.SwaggerGen: a Swagger generator that builds SwaggerDocument objects directly from your routes, controllers, and models. It's typically combined with the Swagger endpoint middleware to automatically expose Swagger JSON.

* Swashbuckle.AspNetCore.SwaggerUI: an embedded version of the Swagger UI tool. It interprets Swagger JSON to build a rich, customizable experience for describing the web API functionality. It includes built-in test harnesses for the public methods. Swashbuckle includes Swagger UI and provides methods to configure and customize the web interface. 

### Swagger/OpenAPI
Library SwashBuckle.AspNetCore, which is an implementation of Swagger/OpenAPI.

OpenAPI refers to the OpenAPI Specification (OAS) developed and supported by the Open API Initiative (OAI). The OAS describes the capabilities of API endpoints, and the OAS is language-agnostic and framework-agnostic. By default, the specification is presented in a document named swagger.json.

Swagger refers to the tools for implementing the OAS, which for example, is described in the swagger.json file. The Swagger tooling ecosystem, including Swagger Editor, Swagger UI, Swagger Codegen, etc., helps developers generate useful documentation and interactive pages for Web APIs. Different programming languages and frameworks have their own implementations of OAS.


### Package installation

    Install-Package Swashbuckle.AspNetCore
  
### Add and configure Swagger middleware
    
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
The code above contains three major parts:
* The method services.AddSwaggerGen() registers services for generating Swagger/OpenAPI documents and configures options for the generators.
* The middleware app.UseSwagger() generates the OpenAPI document and responds to the client if an HTTP request hits the configured route (e.g., /swagger/v1/swagger.json).
* The middleware app.UseSwaggerUI() serves the Swagger UI web page at a specified route, and configures the presentation and optional customization of the web interface.

### OpenAPI Document
One of the critical things Swashbuckle does is generating the OpenAPI document, swagger.json. Based on the configurations in the code above, we can view the JSON document by visiting the route path /swagger/v1/swagger.json. 

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

## XML Documentation
Much of the groundwork has been done. We can focus on documenting our Web API endpoints. The goal is to document our APIs using XML comments and let Swagger UI generate human-friendly descriptions for Operations, Parameters and Schemas based on XML comment files.
To follow along, please make sure the Web API project has the following PropertyGroup section in the csproj file.

XML comments can be enabled with the following approaches:

      <PropertyGroup>
        <GenerateDocumentationFile>true</GenerateDocumentationFile>
        <NoWarn>$(NoWarn);1591</NoWarn>
      </PropertyGroup>

This PropertyGroup section in the csproj file instructs the compiler to generate an XML document file based on XML comments and ignore the warning due to undocumented public types and members. In order to configure Swagger to use the generated XML file, we need to specify the XML file path in the Swagger options

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

### <summary></summary>
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
      
### <remarks></remarks>
Add a <remarks> element to the Create action method documentation. It supplements information specified in the <summary> element and provides a more robust Swagger UI. The <remarks> element content can consist of text, JSON, or XML.

        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TodoItem> Create(TodoItem item)
        {
            _context.TodoItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
        }

### Data annotations
Mark the model with attributes, found in the System.ComponentModel.DataAnnotations namespace, to help drive the Swagger UI components.

Add the [Required] attribute to the Name property of the TodoItem class:

        using System.ComponentModel;
        using System.ComponentModel.DataAnnotations;

        namespace TodoApi.Models
        {
            public class TodoItem
            {
                public long Id { get; set; }

                [Required]
                public string Name { get; set; }

                [DefaultValue(false)]
                public bool IsComplete { get; set; }
            }
        }

Add the [Produces("application/json")] attribute to the API controller. Its purpose is to declare that the controller's actions support a response content type of application/json:

        [Produces("application/json")]
        [Route("api/[controller]")]
        [ApiController]
        public class TodoController : ControllerBase
        {
            private readonly TodoContext _context;
            
### Describe response types
Developers consuming a web API are most concerned with what's returned—specifically response types and error codes (if not standard). The response types and error codes are denoted in the XML comments and data annotations.

The Create action returns an HTTP 201 status code on success. An HTTP 400 status code is returned when the posted request body is null. Without proper documentation in the Swagger UI, the consumer lacks knowledge of these expected outcomes. Fix that problem by adding the highlighted lines in the following example:

        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TodoItem> Create(TodoItem item)
        {
            _context.TodoItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
        }
        
        
 ### Customize the UI
The default UI is both functional and presentable. However, API documentation pages should represent your brand or theme. Branding the Swashbuckle components requires adding the resources to serve static files and building the folder structure to host those files.

If targeting .NET Framework or .NET Core 1.x, add the Microsoft.AspNetCore.StaticFiles NuGet package to the project:

        <PackageReference Include="Microsoft.AspNetCore.StaticFiles" Version="2.0.0" />
        
The preceding NuGet package is already installed if targeting .NET Core 2.x and using the metapackage.
Enable Static File Middleware:

        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();

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
        
 To inject additional CSS stylesheets, add them to the project's wwwroot folder and specify the relative path in the middleware options:
 
    app.UseSwaggerUI(c =>
    {
         c.InjectStylesheet("/swagger-ui/custom.css");
    }
