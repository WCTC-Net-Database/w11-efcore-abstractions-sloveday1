You're absolutely right, and it's a common concern when setting up projects like this. Duplicated code not only violates the DRY (Don't Repeat Yourself) principle but can also lead to maintenance issues down the line.

To address this, we can centralize the configuration logic into a shared class or method that both your runtime DI setup and the `GameContextFactory` can use. This way, you write the configuration code once and reuse it wherever needed.

---

### **Solution: Centralize Configuration Logic**

#### **1. Create a Shared Configuration Helper**

Let's create a static helper class that encapsulates the configuration-building logic. This class can reside in a common project or namespace that both the `ConsoleRpg` and `ConsoleRpgEntities` projects can access.

**ConfigurationHelper.cs:**

```csharp
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ConsoleRpgCommon // or another shared namespace
{
    public static class ConfigurationHelper
    {
        public static IConfigurationRoot GetConfiguration(string basePath = null, string environmentName = null)
        {
            basePath ??= Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Optionally add environment-specific configuration
            if (!string.IsNullOrEmpty(environmentName))
            {
                builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            }

            builder.AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
```

**Explanation:**

- **GetConfiguration Method:**
  - Accepts `basePath` and `environmentName` as optional parameters.
  - Builds the configuration by reading `appsettings.json`.
  - Optionally includes environment-specific configurations and environment variables.
- **Namespace:**
  - Place this helper in a shared namespace or project accessible to both `ConsoleRpg` and `ConsoleRpgEntities`.

#### **2. Modify `Program.cs` to Use the Helper**

In your `ConsoleRpg` project's `Program.cs`, modify the `ConfigureServices` method to use the `ConfigurationHelper`.

**Program.cs:**

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ConsoleRpgEntities;
using ConsoleRpgCommon; // Namespace where ConfigurationHelper is located

namespace ConsoleRpg
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var gameEngine = serviceProvider.GetService<GameEngine>();
            gameEngine?.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Use the ConfigurationHelper
            var configuration = ConfigurationHelper.GetConfiguration();

            // Add logging
            services.AddLogging(configure => configure.AddConsole());

            // Register DbContext with dependency injection
            services.AddDbContext<GameContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register your services
            services.AddTransient<GameEngine>();
            // Add other services as needed
        }
    }
}
```

**Explanation:**

- **Configuration:** Uses the shared `ConfigurationHelper` to build the configuration.
- **Consistency:** Both your application and the design-time factory will use the same configuration logic.

#### **3. Modify `GameContextFactory` to Use the Helper**

In your `ConsoleRpgEntities` project, update the `GameContextFactory` to use the `ConfigurationHelper`.

**GameContextFactory.cs:**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ConsoleRpgCommon; // Namespace where ConfigurationHelper is located

namespace ConsoleRpgEntities
{
    public class GameContextFactory : IDesignTimeDbContextFactory<GameContext>
    {
        public GameContext CreateDbContext(string[] args)
        {
            // Use the ConfigurationHelper
            var configuration = ConfigurationHelper.GetConfiguration(GetApplicationRoot());

            // Build DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            // Create and return the context
            return new GameContext(optionsBuilder.Options);
        }

        private string GetApplicationRoot()
        {
            // Adjust the path to your appsettings.json file
            var basePath = Directory.GetCurrentDirectory();
            return Path.Combine(basePath, "..", "ConsoleRpg");
        }
    }
}
```

**Explanation:**

- **Configuration:** Uses the same `ConfigurationHelper` to build the configuration.
- **Shared Logic:** Eliminates duplication by reusing the configuration-building code.

#### **4. Ensure Access to the Shared Helper**

- **Project References:**
  - If `ConsoleRpgEntities` is unable to reference `ConsoleRpg` directly (since it would create a circular dependency), you can create a new class library project (e.g., `ConsoleRpgCommon`) to hold shared code like `ConfigurationHelper`.
  - Both `ConsoleRpg` and `ConsoleRpgEntities` can reference `ConsoleRpgCommon`.

**Solution Structure:**

```
- Solution Folder
  - ConsoleRpg
    - Program.cs
    - appsettings.json
    - ConsoleRpg.csproj
  - ConsoleRpgEntities
    - GameContext.cs
    - GameContextFactory.cs
    - ConsoleRpgEntities.csproj
  - ConsoleRpgCommon
    - ConfigurationHelper.cs
    - ConsoleRpgCommon.csproj
```

- **Add Project References:**
  - `ConsoleRpg` references `ConsoleRpgEntities` and `ConsoleRpgCommon`.
  - `ConsoleRpgEntities` references `ConsoleRpgCommon`.

#### **5. Adjust the `GetApplicationRoot` Method**

Since `GameContextFactory` is now using `ConfigurationHelper`, you may not need `GetApplicationRoot`, or you can move it to `ConfigurationHelper` if necessary.

Alternatively, you can modify `ConfigurationHelper` to handle the base path appropriately.

**Updated ConfigurationHelper.cs:**

```csharp
public static class ConfigurationHelper
{
    public static IConfigurationRoot GetConfiguration(string basePath = null, string environmentName = null)
    {
        basePath ??= GetApplicationRoot();

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        if (!string.IsNullOrEmpty(environmentName))
        {
            builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
        }

        builder.AddEnvironmentVariables();

        return builder.Build();
    }

    private static string GetApplicationRoot()
    {
        var basePath = Directory.GetCurrentDirectory();
        var projectRoot = Path.Combine(basePath, "..", "ConsoleRpg");
        return projectRoot;
    }
}
```

**Explanation:**

- **GetApplicationRoot:** Moved into `ConfigurationHelper` to centralize path resolution.
- **BasePath Handling:** Adjusts the base path to point to the location of `appsettings.json`.

#### **6. Update `Program.cs` and `GameContextFactory` Accordingly**

In both `Program.cs` and `GameContextFactory`, you can now call `ConfigurationHelper.GetConfiguration()` without worrying about the base path.

---

### **Benefits of This Approach**

- **Eliminates Code Duplication:**
  - Centralizes configuration logic in one place.
  - Both runtime and design-time code use the same method to build the configuration.

- **Easier Maintenance:**
  - Changes to configuration logic (e.g., adding new configuration sources) only need to be made in one place.

- **Consistency:**
  - Ensures that both your application and EF Core tools are using the exact same configuration settings.

---

### **Example Usage**

**Program.cs:**

```csharp
// Inside ConfigureServices method
var configuration = ConfigurationHelper.GetConfiguration();
```

**GameContextFactory.cs:**

```csharp
// Inside CreateDbContext method
var configuration = ConfigurationHelper.GetConfiguration();
```

---

### **Additional Considerations**

#### **1. Handling Environment-Specific Configurations**

If you have environment-specific configuration files (e.g., `appsettings.Development.json`), you can modify `ConfigurationHelper` to handle this.

```csharp
public static IConfigurationRoot GetConfiguration(string basePath = null)
{
    basePath ??= GetApplicationRoot();

    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

    var builder = new ConfigurationBuilder()
        .SetBasePath(basePath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
        .AddEnvironmentVariables();

    return builder.Build();
}
```

- **Explanation:**
  - Reads the environment name from the `ASPNETCORE_ENVIRONMENT` variable.
  - Includes environment-specific configuration files if they exist.

#### **2. Ensuring Correct Paths**

- **GetApplicationRoot:**
  - Adjust the `GetApplicationRoot` method in `ConfigurationHelper` to accurately locate `appsettings.json`.
  - This is crucial for the `GameContextFactory` to find the configuration file during design-time operations.

#### **3. Circular Dependencies**

- **Avoid Circular References:**
  - By placing `ConfigurationHelper` in a separate project (`ConsoleRpgCommon`), you avoid circular dependencies between `ConsoleRpg` and `ConsoleRpgEntities`.

#### **4. Dependency Injection and Service Registration**

- **Registering Other Services:**
  - You can also centralize the registration of services if needed.
  - Create a `ServiceRegistration` class in `ConsoleRpgCommon` to handle service registrations.

---

### **Example of Centralizing Service Registration (Optional)**

**ServiceRegistration.cs in ConsoleRpgCommon:**

```csharp
using Microsoft.Extensions.DependencyInjection;
using ConsoleRpgEntities;

namespace ConsoleRpgCommon
{
    public static class ServiceRegistration
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add logging
            services.AddLogging(configure => configure.AddConsole());

            // Register DbContext
            services.AddDbContext<GameContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register other services
            services.AddTransient<GameEngine>();
            // Add more services as needed
        }
    }
}
```

**Modify Program.cs:**

```csharp
private static void ConfigureServices(IServiceCollection services)
{
    var configuration = ConfigurationHelper.GetConfiguration();
    ServiceRegistration.ConfigureServices(services, configuration);
}
```

**Explanation:**

- **Centralizes Service Registration:**
  - Moves service registrations into a shared method.
  - Both `Program.cs` and other projects can reuse this method if necessary.

---

### **Final Thoughts**

By centralizing the configuration logic into a shared helper class, you eliminate code duplication and ensure consistency across your application. This approach adheres to SOLID principles, particularly the **Single Responsibility Principle**, as the `ConfigurationHelper` class is solely responsible for building the configuration.

**Key Benefits:**

- **DRY Principle:** Avoids code duplication.
- **Maintainability:** Easier to maintain and update configuration logic.
- **Consistency:** Both runtime and design-time code use the same configuration source.

**Action Items:**

1. **Create the `ConfigurationHelper` class in a shared project or namespace.
2. **Reference the shared project in both `ConsoleRpg` and `ConsoleRpgEntities`.
3. **Update `Program.cs` and `GameContextFactory` to use `ConfigurationHelper`.
4. **Ensure that all paths and references are correctly set up.

**Let me know if you have any questions or need further assistance implementing this solution!**