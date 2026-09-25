# How to Use LeetKeen.Results

## Overview

LeetKeen.Results is a .NET library that provides a robust, type-safe way to handle operation results and errors in your applications. It implements the Result pattern, which allows you to explicitly handle success and failure cases without relying on exceptions for control flow.

## Core Concepts

### IResult and IResult&lt;T&gt;

The library provides two main interfaces:

- **`IResult`**: Non-generic interface representing any operation result
- **`IResult<T>`**: Generic interface representing a result with a typed value

Both interfaces provide:
- `IsSuccess()`: Returns `true` if the operation succeeded (no errors)
- `Errors`: Collection of errors that occurred during the operation
- `Value`: The result value (typed in `IResult<T>`)

### IError

Represents an error with the following properties:

- **`Code`**: Integer error code (0 by default)
- **`Message`**: Human-readable error message
- **`IsSecret`**: Indicates if the error should be hidden from public visibility
- **`Data`**: Dictionary for additional error metadata
- **`Reasons`**: Collection of sub-errors that caused this error

## Basic Usage

### Creating Successful Results

Use the `Result.Ok<T>()` static method or the `AsOkResult()` extension method:

```csharp
using LeetKeen.Results;

// Using static method
IResult<int> result = Result.Ok(42);

// Using extension method
IResult<string> result2 = "Hello World".AsOkResult();

// Using Result<T>.Ok() method
IResult<bool> result3 = Result<bool>.Ok(true);
```

### Creating Failed Results

Use the `Result.Fail<T>()` static method with one or more errors:

```csharp
// Single error
var error = new Error(100, "Something went wrong");
IResult<int> result1a = Result.Fail<int>(error);
IResult<int> result1b = Result<int>.Fail(error);

// Multiple errors
var error1 = new Error(101, "First error");
var error2 = new Error(102, "Second error");
IResult<string> result2a = Result.Fail<string>(error1, error2);
IResult<string> result2b = Result<string>.Fail(error1, error2);

// Using Error.New() factory method
IResult<bool> result3a = Result.Fail<bool>(Error.New(103, "Error message"));
IResult<bool> result3b = Result<bool>.Fail(Error.New(103, "Error message"));

// Converting an error directly to a result
IResult<int> result4 = error.ToResult<int>();
```

### Failed Results with Values

You can create a failed result that still contains a value:

```csharp
var error = new Error(200, "Validation failed");
IResult<User> result = Result.Fail(defaultUser, error);
```

### Checking Result Status

```csharp
IResult<int> result = GetSomeResult();

// Check if successful
if (result.IsSuccess())
{
    var value = result.Value;
    // Process the value
}

// Check if failed (using extension method)
if (result.IsFailure())
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"Error {error.Code}: {error.Message}");
    }
}
```

## Working with Errors

### Creating Errors

```csharp
// Basic error with code and message
var error = new Error(100, "Invalid input");

// Using factory method
var error2 = Error.New(200, "Database error");

// Empty error (code defaults to 0)
var error3 = new Error();
error3.Code = 300;
error3.Message = "Custom error";
```

### Adding Metadata to Errors

Use the `WithData()` extension method to add contextual information:

```csharp
var error = new Error(100, "Invalid user")
    .WithData("userId", 12345)
    .WithData("attemptedAction", "login")
    .WithData("timestamp", DateTime.UtcNow);
```

### Adding Error Reasons (Sub-Errors)

Build hierarchical error structures using the `WithReason()` extension method:

```csharp
var rootCause = new Error(101, "Network timeout");
var secondaryError = new Error(102, "Retry failed");
var mainError = new Error(100, "Operation failed")
    .WithReason(rootCause)
    .WithReason(secondaryError);

IResult<string> result = Result.Fail<string>(mainError);

// Access nested errors
foreach (var reason in mainError.Reasons)
{
    Console.WriteLine($"Caused by: {reason.Message}");
}
```

### Standard Error Codes

The library provides predefined error codes in the `StandardErrorCodes` class:

```csharp
// Transport/Infrastructure errors
StandardErrorCodes.GeneralError          // -1: Generic error
StandardErrorCodes.TransportError        // -2: Network/transport error
StandardErrorCodes.MalformedRequest      // -3: Invalid request data
StandardErrorCodes.RemoteInternalError   // -4: Remote system error
StandardErrorCodes.FeatureUnavailable    // -5: Feature not available
StandardErrorCodes.NoIdentity            // -6: Authentication required
StandardErrorCodes.NoPermission          // -7: Authorization failed
StandardErrorCodes.BusinessError         // -8: Generic business error
StandardErrorCodes.RepositoryError       // -9: Repository layer error

// Business errors
StandardErrorCodes.Business.EntityNotFound         // -1001
StandardErrorCodes.Business.InvalidAuthCredentials // -1002
StandardErrorCodes.Business.CantCreateExists       // -1003

// Usage
var error = new Error(StandardErrorCodes.Business.EntityNotFound, "User not found");
```

### EntityNotFound Helper

For common "entity not found" scenarios:

```csharp
// Basic usage
var error = Error.EntityNotFound(userId);

// With entity kind
var error2 = Error.EntityNotFound(productId, "Product");

// The error automatically includes the ID in the Data dictionary
IResult<User> result = Result.Fail<User>(error);
```

## Recommended Patterns

### 1. Return Results from Methods

Replace exceptions with results for expected failure cases:

```csharp
public IResult<User> GetUserById(int userId)
{
    var user = _database.FindUser(userId);
    
    if (user == null)
    {
        return Result.Fail<User>(Error.EntityNotFound(userId, "User"));
    }
    
    return Result.Ok(user);
}
```

### 2. Validate Input

```csharp
public IResult<Order> CreateOrder(OrderRequest request)
{
    if (request == null)
    {
        return Result.Fail<Order>(
            new Error(StandardErrorCodes.MalformedRequest, "Request cannot be null"));
    }
    
    if (request.Items.Count == 0)
    {
        var error = new Error(100, "Order must contain at least one item")
            .WithData("orderId", request.Id);
        return Result.Fail<Order>(error);
    }
    
    var order = ProcessOrder(request);
    return Result.Ok(order);
}
```

### 3. Chain Operations

```csharp
public IResult<Invoice> ProcessPayment(int orderId)
{
    var orderResult = GetOrderById(orderId);
    if (orderResult.IsFailure())
    {
        return Result.Fail<Invoice>(orderResult.Errors);
    }
    
    var paymentResult = ChargeCustomer(orderResult.Value);
    if (paymentResult.IsFailure())
    {
        return Result.Fail<Invoice>(paymentResult.Errors);
    }
    
    var invoice = GenerateInvoice(paymentResult.Value);
    return Result.Ok(invoice);
}
```

### 4. Aggregate Multiple Errors

```csharp
public IResult<User> ValidateUser(User user)
{
    var errors = new List<IError>();
    
    if (string.IsNullOrEmpty(user.Email))
    {
        errors.Add(new Error(200, "Email is required")
            .WithData("field", "Email"));
    }
    
    if (user.Age < 18)
    {
        errors.Add(new Error(201, "User must be 18 or older")
            .WithData("field", "Age")
            .WithData("providedAge", user.Age));
    }
    
    if (errors.Any())
    {
        return Result.Fail<User>(errors);
    }
    
    return Result.Ok(user);
}
```

### 5. Service Layer Pattern

```csharp
public class UserService
{
    private readonly IUserRepository _repository;
    
    public IResult<User> CreateUser(string email, string password)
    {
        // Validation
        if (string.IsNullOrEmpty(email))
        {
            return Result.Fail<User>(new Error(100, "Email is required"));
        }
        
        // Check existence
        var existing = _repository.FindByEmail(email);
        if (existing != null)
        {
            return Result.Fail<User>(
                new Error(StandardErrorCodes.Business.CantCreateExists, 
                    "User with this email already exists")
                .WithData("email", email));
        }
        
        // Create
        try
        {
            var user = new User { Email = email, Password = password };
            _repository.Add(user);
            return Result.Ok(user);
        }
        catch (Exception ex)
        {
            var error = new Error(StandardErrorCodes.RemoteInternalError, 
                "Failed to create user")
                .WithReason(new Error(500, ex.Message));
            return Result.Fail<User>(error);
        }
    }
}
```

### 6. API Controller Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var result = _userService.GetUserById(id);
        
        if (result.IsFailure())
        {
            // Map error codes to HTTP status codes
            var firstError = result.Errors.First();
            
            return firstError.Code switch
            {
                StandardErrorCodes.Business.EntityNotFound => NotFound(result.Errors),
                StandardErrorCodes.NoPermission => Forbid(),
                StandardErrorCodes.NoIdentity => Unauthorized(),
                _ => BadRequest(result.Errors)
            };
        }
        
        return Ok(result.Value);
    }
    
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        var result = _userService.CreateUser(request.Email, request.Password);
        
        if (result.IsFailure())
        {
            return BadRequest(new 
            {
                errors = result.Errors.Select(e => new
                {
                    code = e.Code,
                    message = e.Message,
                    data = e.Data
                })
            });
        }
        
        return CreatedAtAction(nameof(GetUser), 
            new { id = result.Value.Id }, 
            result.Value);
    }
}
```

### 7. Hide Sensitive Information

Use the `IsSecret` property to mark errors that shouldn't be exposed:

```csharp
public class SecretError : Error
{
    public SecretError(int code, string message) : base(code, message)
    {
    }
    
    public bool IsSecret => true; // Override to return true
}

// In your API response builder
var publicErrors = result.Errors
    .Where(e => !e.IsSecret)
    .Select(e => new { e.Code, e.Message });
```

## Best Practices

### DO ✅

1. **Use results for expected failures** (validation errors, not found, etc.)
2. **Provide meaningful error codes** to help clients handle errors programmatically
3. **Add contextual data** to errors using `WithData()` for debugging
4. **Use hierarchical errors** with `WithReason()` to trace error chains
5. **Check `IsSuccess()` or `IsFailure()`** before accessing `Value`
6. **Return specific error codes** for different failure scenarios
7. **Use standard error codes** when applicable for consistency

### DON'T ❌

1. **Don't use results for exceptional/unexpected failures** (use exceptions for those)
2. **Don't access `Value` without checking success** first
3. **Don't create failed results without errors** (Fail methods require at least one error)
4. **Don't expose secret errors** in public APIs (check `IsSecret` property)
5. **Don't reuse error instances** across multiple results (create new instances)

## Custom Error Codes

Define your own error code constants:

```csharp
public static class MyAppErrorCodes
{
    public const int InvalidEmailFormat = 1001;
    public const int PasswordTooWeak = 1002;
    public const int AccountLocked = 1003;
    public const int QuotaExceeded = 1004;
}

// Usage
var error = new Error(MyAppErrorCodes.PasswordTooWeak, "Password must be at least 8 characters")
    .WithData("minLength", 8)
    .WithData("providedLength", password.Length);
```

## Advanced Scenarios

### Type Introspection

Get the value type even when the value is null:

```csharp
IResult result = GetSomeResult(); // Non-generic reference
Type valueType = result.GetValueType(); // Returns typeof(T)
```

### Adding Errors After Creation

```csharp
var result = new Result<User>(user);
result.AddError(new Error(100, "Warning: User has incomplete profile"));

// Result now has a value AND errors (unusual but supported)
bool isSuccess = result.IsSuccess(); // false (has errors)
var value = result.Value; // Still contains the user
```

## Summary

LeetKeen.Results provides a clean, type-safe way to handle success and failure in your .NET applications. By using the Result pattern:

- **Explicit error handling**: Callers must check for success/failure
- **Type safety**: Compile-time guarantees about result types
- **Rich error information**: Codes, messages, metadata, and hierarchical reasons
- **Better API design**: Clear contracts about what can fail and why
- **No exception abuse**: Reserve exceptions for truly exceptional cases

Start by returning `IResult<T>` from methods that can fail in expected ways, check `IsSuccess()` or `IsFailure()`, and build rich error information using the `Error` class and its extensions.
