# SimpleCqrsMediator
This is a simple implementation of the CqrsMediator patern. It is designed to be easy to use and understand, while still providing the basic functionality needed for a CQRS application.

This project is inspired by project [MediatR](https://github.com/jbogard/MediatR). The main differance is thats this project is making a distinction between commands and queries. Commands are used to change the state of the system, while queries are used to retrieve data from the system.

# features
- Interface based commands and queries.
- Automatic ID container registration of command-/ query handlers.
- Injectable Exception handler.

# Usage
Below is a simple example of how to use the CqrsMediator. 

First Add the CqrsMediator to your service collection in the Startup.cs file:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    var assemblies = new[]
    {
        Assembly.GetExecutingAssembly(),
        Assembly.Load("App.Business"),
        Assembly.Load("App.Data"),
    };

    services.AddCqrsMediator(assemblies);
}
```


Then you can create a query or command by implementing the `IQuery<TResult>` or `ICommand` interface. For example:

```csharp
public class TestQuery : IQuery<TestResultDto>
{
}

public class TestResultDto
{
    public string Result { get; set; }
}
```

Next, you need to create a handler for the query or command by implementing the `IQueryHandler<TQuery, TResult>` or `ICommandHandler<TCommand>` interface:
```csharp
public class TestQueryHandler(
    HttpClient HttpClient
    ) : IQueryHandler<TestQuery, TestResultDto>
{
    public async Task<TestResultDto> HandleAsync(TestQuery query)
    {
        var result = await ...// Call your API or perform some logic here
        return new TestResultDto { Result = result };
    }
}
```

Finally, you can use the `CqrsMediator` to send the query or command and get the result:
```csharp
[ApiController]
[Route("[controller]")]
public class RestFullTestController(
    IQueryProcessor QueryProcessor
    ) : ControllerBase
{
    [HttpGet]
    [Route("/Test")]
    public async Task<ActionResult<TestResultDto>> Test(TestQuery query)
    {
        var result = await QueryProcessor.ProcessAsync(query);
        return Ok(result);
    }
}
```

# Exception Handling
If you want to handle exceptions that occur during the processing of commands or queries, you can implement the `IProcessorExceptionHandler` interface:
```csharp
public class CustomProcessorExceptionHandler : IProcessorExceptionHandler
{
    public void HandleAsync(Exception exception)
    {
        // Log the exception or perform some custom logic
    }
}
```

Then, register the exception handler in your service collection:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    var assemblies = new[]
    {
        Assembly.GetExecutingAssembly(),
        Assembly.Load("App.Business"),
        Assembly.Load("App.Data"),
    };

    services.AddCqrsMediator(assemblies);
    services.AddCqrsMediatorExceptionHandler<CustomProcessorExceptionHandler>();
}
```

# Notes
This is an expirimental project; use in production at your own risk.