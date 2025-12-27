# JsonBatch

JsonBatch is a .NET library designed to facilitate sending batch operations (Create, Update, Delete) in a single HTTP request. It abstracts the complexity of managing dependencies between entities (e.g., creating a parent and child record in the same batch) using temporary keys.

The library is designed with flexibility in mind, offering a base core library and implementations for both `System.Text.Json` and `Newtonsoft.Json`.

## Project Structure

*   **JsonBatchClient.Base**: Contains the core interfaces, models (`BatchRequest`, `BatchResponse`), and the abstract `DefaultClient`.
*   **JsonBatchClient.Json**: Implementation of the client using `System.Text.Json`, the new standard.
*   **JsonBatchClient.Newtonsoft**: Implementation of the client using `Newtonsoft.Json`.

## Key Features

*   **Batch Operations**: Combine multiple Create, Update, and Delete operations into a single request.
*   **Transaction Support**: Execute batches as transactions via the `ExecuteTransactionAsync` method.
*   **Automatic Temporary Key Management**: When adding `Create` operations, the library automatically handles temporary keys for related entities to ensure proper linking on the server side.
*   **Flexible Serialization**: Choose between `System.Text.Json` or `Newtonsoft.Json` depending on your project's needs.

## Usage

### 1. Define Your Entities

Entities must inherit from `BaseEntity` (which contains a `Guid Id`).

```csharp
using JsonBatchClient.Base;

public class User : BaseEntity
{
    public string Name { get; set; }
    public List<Post> Posts { get; set; } = new List<Post>();
}

public class Post : BaseEntity
{
    public string Content { get; set; }
    public User User { get; set; }
}
```

### 2. Create a Batch Request

Use `BatchRequest` to queue up your operations. The library will automatically detect relationships if you are creating related entities.

```csharp
using JsonBatchClient.Base;

var batchRequest = new BatchRequest();

var user = new User { Id = Guid.NewGuid(), Name = "John Doe" };
var post = new Post { Id = Guid.NewGuid(), Content = "Hello World", User = user };

// Add operations
// Note: The order of adding operations might matter depending on your server-side logic, 
// but the library handles the ID mapping.
batchRequest.AddCreateOperation(user);
batchRequest.AddCreateOperation(post);
```

### 3. Initialize the Client

Instantiate the client corresponding to your preferred JSON library. You'll need to provide an `HttpClient` and the appropriate serializer settings/options.

**Using System.Text.Json:**

```csharp
using JsonBatchClient.Json;
using System.Text.Json;

var httpClient = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

var client = new DefaultClient(httpClient, options);
```

**Using Newtonsoft.Json:**

```csharp
using JsonBatchClient.Newtonsoft;
using Newtonsoft.Json;

var httpClient = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
var settings = new JsonSerializerSettings();

var client = new DefaultClient(httpClient, settings);
```

### 4. Execute the Batch

```csharp
try
{
    // Execute as a standard batch
    var response = await client.ExecuteBatchAsync(batchRequest);

    if (response.IsSuccess)
    {
        Console.WriteLine("All operations succeeded.");
    }
    else
    {
        foreach (var failure in response.FailedResults)
        {
            Console.WriteLine($"Operation failed: {failure.Message}"); // Adjust based on actual OperationResult properties
        }
    }

    // Or execute as a transaction
    // var transactionResponse = await client.ExecuteTransactionAsync(batchRequest);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
```

## Dependencies

*   .NET 8.0
*   `Newtonsoft.Json` (for `JsonBatchClient.Newtonsoft`)
