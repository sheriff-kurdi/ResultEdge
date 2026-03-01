# ResultEdge

**ResultEdge** is a lightweight, expressive Result pattern implementation for .NET that enables clean, explicit error handling without exceptions as control flow.

Model success and failure as first-class return values — improving readability, testability, and predictability across your application.

[![NuGet](https://img.shields.io/nuget/v/ResultEdge.svg)](https://www.nuget.org/packages/ResultEdge)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

---

## Table of Contents

- [Installation](#installation)
- [Target Frameworks](#target-frameworks)
- [Core Types](#core-types)
- [Result — void operations](#result--void-operations)
- [Result\<T\> — typed operations](#resultt--typed-operations)
- [PagedResult\<T\> — paginated data](#pagedresultt--paginated-data)
- [Factory Methods Reference](#factory-methods-reference)
- [Validation Errors](#validation-errors)
- [Functional Extensions](#functional-extensions)
  - [Map](#map)
  - [MapAsync](#mapasync)
  - [Bind](#bind)
  - [BindAsync](#bindasync)
  - [Match](#match)
- [Implicit Operators](#implicit-operators)
- [ResultStatus Enum](#resultstatus-enum)
- [Real-World Examples](#real-world-examples)

---

## Installation

```bash
dotnet add package ResultEdge
```

---

## Target Frameworks

ResultEdge targets multiple frameworks for maximum compatibility:

- `netstandard2.0` — compatible with .NET Framework 4.6.1+, .NET Core 2.0+, Unity, Xamarin
- `net6.0`
- `net8.0`

---

## Core Types

| Type | Use case |
|---|---|
| `Result` | Operations that return no value (commands) |
| `Result<T>` | Operations that return a typed value |
| `PagedResult<T>` | Paginated queries returning a typed list |

All types implement `IResult` and share the same properties and factory methods.

### Shared Properties

| Property | Type | Description |
|---|---|---|
| `IsSuccess` | `bool` | `true` when `Status == Ok` |
| `IsFailure` | `bool` | `true` when `Status != Ok` (inverse of `IsSuccess`) |
| `Status` | `ResultStatus` | The outcome status |
| `Data` | `T?` | The returned value (typed results only) |
| `DataType` | `Type` | The CLR type of `T` |
| `Errors` | `IEnumerable<string>` | Error messages |
| `ValidationErrors` | `IReadOnlyList<ValidationError>` | Structured validation failures (immutable after construction) |
| `SuccessMessage` | `string` | Optional message on success |
| `CorrelationId` | `string` | Optional correlation ID for distributed tracing |

---

## Result — void operations

Use `Result` when the operation does not return a value.

```csharp
// Success
Result result = Result.Success();

// Success with a message
Result result = Result.SuccessWithMessage("Order placed successfully.");

// Error
Result result = Result.Error("Something went wrong.");

// Error with a correlation ID for distributed tracing
Result result = Result.ErrorWithCorrelationId("req-abc-123", "Payment gateway timeout.");

// Not found
Result result = Result.NotFound();
Result result = Result.NotFound("Order not found.");

// Validation failure
Result result = Result.Invalid(new ValidationError("Email", "Invalid format.", "EMAIL_INVALID", ValidationSeverity.Error));

// Forbidden / Unauthorized
Result result = Result.Forbidden();
Result result = Result.Unauthorized();

// Conflict
Result result = Result.Conflict("A record with this name already exists.");

// Critical / Unavailable
Result result = Result.CriticalError("Unhandled exception occurred.");
Result result = Result.Unavailable("Database is temporarily unavailable.");
```

Checking the result:

```csharp
if (result.IsSuccess)
{
    Console.WriteLine(result.SuccessMessage);
}
else
{
    foreach (var error in result.Errors)
        Console.WriteLine(error);
}
```

---

## Result\<T\> — typed operations

Use `Result<T>` when the operation returns a value.

```csharp
// Success with data
Result<Order> result = Result<Order>.Success(order);

// Success with data and a message
Result<Order> result = Result<Order>.Success(order, "Order retrieved successfully.");

// Using the non-generic Result as a convenient entry point
Result<Order> result = Result.Success(order);
Result<Order> result = Result.Success(order, "Loaded.");

// Error states — identical factory methods as Result
Result<Order> result = Result<Order>.Error("Failed to retrieve order.");
Result<Order> result = Result<Order>.ErrorWithCorrelationId("req-xyz", "Database timeout.");
Result<Order> result = Result<Order>.NotFound("Order #42 does not exist.");
Result<Order> result = Result<Order>.Forbidden();
Result<Order> result = Result<Order>.Unauthorized();
Result<Order> result = Result<Order>.Conflict("Order is already closed.");
Result<Order> result = Result<Order>.CriticalError("Unexpected database error.");
Result<Order> result = Result<Order>.Unavailable("Service is down.");
```

Accessing the data:

```csharp
Result<Order> result = await orderService.GetOrderAsync(id);

if (result.IsSuccess)
{
    Console.WriteLine(result.Data!.Total);
}
else
{
    Console.WriteLine(string.Join(", ", result.Errors));

    // Access the correlation ID if set (useful for log correlation)
    if (!string.IsNullOrEmpty(result.CorrelationId))
        Console.WriteLine($"TraceId: {result.CorrelationId}");
}
```

---

## PagedResult\<T\> — paginated data

`PagedResult<T>` extends `Result<T>` with pagination metadata via `PagedInfo`.

> **Note:** `PagedInfo` is `null` on non-success results. Always check `IsSuccess` before accessing it.

### PagedInfo

```csharp
var pagedInfo = new PagedInfo(
    pageNumber:   1,
    pageSize:     10,
    totalPages:   5,
    totalRecords: 48
);
```

`PagedInfo` also supports a fluent builder style:

```csharp
var pagedInfo = new PagedInfo(1, 10, 5, 48)
    .SetPageNumber(2)
    .SetPageSize(20)
    .SetTotalPages(3)
    .SetTotalRecords(60);
```

### Creating a PagedResult

**Option 1 — static factory (recommended):**

```csharp
var result = PagedResult<List<Product>>.Success(pagedInfo, products);

// With a success message
var result = PagedResult<List<Product>>.Success(pagedInfo, products, "Products loaded.");
```

**Option 2 — direct constructor:**

```csharp
var result = new PagedResult<List<Product>>(pagedInfo, products);
```

**Option 3 — via `ToPagedResult()` on an existing `Result<T>`:**

```csharp
Result<List<Product>> result = Result.Success(products);
PagedResult<List<Product>> pagedResult = result.ToPagedResult(pagedInfo);
```

### Returning errors from a PagedResult

All factory methods are available directly on `PagedResult<T>`. `PagedInfo` is `null` for all error states.

```csharp
PagedResult<List<Product>> result = PagedResult<List<Product>>.Error("Failed to retrieve products.");
PagedResult<List<Product>> result = PagedResult<List<Product>>.ErrorWithCorrelationId("req-123", "Timeout.");
PagedResult<List<Product>> result = PagedResult<List<Product>>.NotFound("No products found.");
PagedResult<List<Product>> result = PagedResult<List<Product>>.Forbidden();
PagedResult<List<Product>> result = PagedResult<List<Product>>.Unauthorized();
PagedResult<List<Product>> result = PagedResult<List<Product>>.Conflict("Concurrent update detected.");
PagedResult<List<Product>> result = PagedResult<List<Product>>.CriticalError("Database error.");
PagedResult<List<Product>> result = PagedResult<List<Product>>.Unavailable("Service unavailable.");
PagedResult<List<Product>> result = PagedResult<List<Product>>.Invalid(
    new ValidationError("PageSize", "Must be between 1 and 100.", "INVALID_PAGE_SIZE", ValidationSeverity.Error)
);
```

Accessing paged data:

```csharp
PagedResult<List<Product>> result = await productService.GetProductsAsync(query);

if (result.IsSuccess)
{
    Console.WriteLine($"Page {result.PagedInfo!.PageNumber} of {result.PagedInfo.TotalPages}");
    Console.WriteLine($"Total records: {result.PagedInfo.TotalRecords}");

    foreach (var product in result.Data!)
        Console.WriteLine(product.Name);
}
```

---

## Factory Methods Reference

All methods are available on `Result`, `Result<T>`, and `PagedResult<T>`.

| Method | Status | HTTP equivalent |
|---|---|---|
| `Success()` | `Ok` | 200 |
| `SuccessWithMessage(message)` | `Ok` | 200 |
| `Success<T>(data)` | `Ok` | 200 |
| `Success<T>(data, message)` | `Ok` | 200 |
| `Error(messages)` | `Error` | 500 |
| `ErrorWithCorrelationId(id, messages)` | `Error` | 500 |
| `Invalid(validationError)` | `Invalid` | 422 |
| `Invalid(validationErrors[])` | `Invalid` | 422 |
| `Invalid(List<ValidationError>)` | `Invalid` | 422 |
| `NotFound()` | `NotFound` | 404 |
| `NotFound(messages)` | `NotFound` | 404 |
| `Forbidden()` | `Forbidden` | 403 |
| `Unauthorized()` | `Unauthorized` | 401 |
| `Conflict()` | `Conflict` | 409 |
| `Conflict(messages)` | `Conflict` | 409 |
| `CriticalError(messages)` | `CriticalError` | 500 |
| `Unavailable(messages)` | `Unavailable` | 503 |

---

## Validation Errors

`ValidationError` carries structured information about a validation failure. Properties are **init-only** — the object is immutable after construction.

```csharp
// Simple message (constructor shorthand)
var error = new ValidationError("Email is required.");

// Full detail
var error = new ValidationError(
    identifier:   "Email",
    errorMessage: "Must be a valid email address.",
    errorCode:    "EMAIL_INVALID",
    severity:     ValidationSeverity.Error
);

// Object initializer syntax
var error = new ValidationError
{
    Identifier   = "Email",
    ErrorMessage = "Must be a valid email address.",
    ErrorCode    = "EMAIL_INVALID",
    Severity     = ValidationSeverity.Error
};
```

### ValidationSeverity

```csharp
public enum ValidationSeverity
{
    Error   = 0,  // Blocks processing
    Warning = 1,  // Non-blocking advisory
    Info    = 2   // Informational only
}
```

### Returning multiple validation errors

```csharp
var errors = new List<ValidationError>
{
    new("Name",  "Name is required.",          "REQUIRED",      ValidationSeverity.Error),
    new("Email", "Must be a valid email.",      "EMAIL_INVALID", ValidationSeverity.Error),
    new("Age",   "Age should be 18 or older.", "AGE_WARNING",   ValidationSeverity.Warning),
};

return Result<Order>.Invalid(errors);
```

Reading validation errors:

```csharp
if (result.Status == ResultStatus.Invalid)
{
    foreach (var ve in result.ValidationErrors)
    {
        Console.WriteLine($"[{ve.Severity}] {ve.Identifier}: {ve.ErrorMessage} ({ve.ErrorCode})");
    }
}
```

---

## Functional Extensions

ResultEdge ships a full suite of functional extensions for composable, pipeline-style result handling. All extensions are in the `ResultEdge` namespace.

### Map

Transforms the `Data` of a successful result without unwrapping it. Non-success statuses, errors, `SuccessMessage`, and `CorrelationId` are propagated automatically — `func` is never called.

```csharp
Result<Order> orderResult = await orderRepository.GetAsync(id);

Result<OrderDto> dtoResult = orderResult.Map(order => new OrderDto
{
    Id    = order.Id,
    Total = order.Total,
});
```

Chaining:

```csharp
Result<string> result = Result<int>.Success(10)
    .Map(x => x * 2)   // Result<int> → Result<int>
    .Map(x => x + 5)   // Result<int> → Result<int>
    .Map(x => $"{x}"); // Result<int> → Result<string>
// result.Data == "25"
```

### MapAsync

Async overload of `Map`. Accepts an async transform function, or can be called on a `Task<Result<T>>` directly.

```csharp
// Async transform
Result<int> result = Result<int>.Success(21);

Result<int> doubled = await result.MapAsync(async x =>
{
    await Task.Yield();
    return x * 2;
});

// On a Task<Result<T>> — synchronous transform
Task<Result<int>> resultTask = Task.FromResult(Result<int>.Success(7));
Result<int> mapped = await resultTask.MapAsync(x => x * 6);
// mapped.Data == 42
```

### Bind

Chains an operation that itself returns a `Result<TDestination>`. Unlike `Map`, the `func` you provide owns the returned result — including its status. If the input result is a failure, `func` is not called and the failure is propagated.

```csharp
Result<User> userResult = await userRepo.GetAsync(userId);

Result<Order> orderResult = userResult.Bind(user =>
{
    if (!user.IsActive)
        return Result<Order>.Forbidden();

    return Result<Order>.Success(new Order(user));
});
```

Chaining:

```csharp
Result<string> result = Result<int>.Success(5)
    .Bind(x => Result<int>.Success(x * 2))
    .Bind(x => Result<string>.Success($"Value: {x}"));
// result.Data == "Value: 10"
```

### BindAsync

Async overload of `Bind`. Also available as a `Task<Result<T>>` extension.

```csharp
// Async func
Result<string> bound = await result.BindAsync(async x =>
{
    var validated = await ValidateAsync(x);
    return validated
        ? Result<string>.Success($"ok:{x}")
        : Result<string>.Error("Validation failed.");
});

// On a Task<Result<T>> — synchronous func
Task<Result<int>> resultTask = Task.FromResult(Result<int>.Success(3));
Result<int> chained = await resultTask.BindAsync(x => Result<int>.Success(x * 10));
// chained.Data == 30
```

### Match

Pattern-matches on success or failure, executing exactly one branch.

**Value-returning overload:**

```csharp
Result<int> result = Result<int>.Success(42);

IResult response = result.Match(
    onSuccess: data  => Results.Ok(data),
    onFailure: r     => r.Status switch
    {
        ResultStatus.NotFound  => Results.NotFound(r.Errors),
        ResultStatus.Forbidden => Results.Forbid(),
        _                      => Results.Problem(string.Join(", ", r.Errors))
    });
```

**Void overload (side-effects):**

```csharp
result.Match(
    onSuccess: data => logger.LogInformation("Got {Data}", data),
    onFailure: r    => logger.LogWarning("Failed: {Status}", r.Status));
```

Both overloads also work on void `Result` — the `onSuccess` branch receives no data parameter:

```csharp
Result commandResult = await commandHandler.HandleAsync(command);

// Value-returning
IResult response = commandResult.Match(
    onSuccess: ()  => Results.Ok(),
    onFailure: r   => Results.Problem(string.Join(", ", r.Errors)));

// Side-effect
commandResult.Match(
    onSuccess: ()  => logger.LogInformation("Command succeeded"),
    onFailure: r   => logger.LogWarning("Command failed: {Status}", r.Status));
```

---

## Implicit Operators

ResultEdge provides implicit conversions to reduce boilerplate.

### `T` → `Result<T>`

```csharp
// Assign a value directly — wraps it in a successful Result<T>
Result<int> result = 42;
```

### `Result<T>` → `T`

```csharp
// Unwrap the data directly
Result<int> result = Result<int>.Success(42);
int value = result; // 42
```

### `Result` → `Result<T>`

```csharp
// Propagate a non-generic Result into a typed context
Result voidResult = Result.NotFound("User not found.");
Result<User> typedResult = voidResult; // carries NotFound status and errors
```

---

## ResultStatus Enum

```csharp
public enum ResultStatus
{
    Ok,            // Operation succeeded
    Error,         // General application error (HTTP 500)
    Invalid,       // Validation failure   (HTTP 422)
    NotFound,      // Resource not found   (HTTP 404)
    Forbidden,     // Authenticated but not authorised (HTTP 403)
    Unauthorized,  // Not authenticated    (HTTP 401)
    Conflict,      // State conflict       (HTTP 409)
    CriticalError, // Unhandled exception  (HTTP 500)
    Unavailable    // Dependency unavailable — transient (HTTP 503)
}
```

---

## Real-World Examples

### Service layer

```csharp
public async Task<Result<OrderDto>> GetOrderAsync(int id)
{
    var order = await _repository.FindAsync(id);

    if (order is null)
        return Result<OrderDto>.NotFound($"Order {id} was not found.");

    if (!_currentUser.CanView(order))
        return Result<OrderDto>.Forbidden();

    return Result<OrderDto>.Success(order.ToDto());
}
```

### Pipeline with Bind

```csharp
public async Task<Result<InvoiceDto>> CreateInvoiceAsync(int orderId)
{
    return await _orderRepo.GetAsync(orderId)            // Task<Result<Order>>
        .BindAsync(ValidateOrderForInvoice)              // Result<Order>   → Result<Order>
        .BindAsync(order => _invoiceService.CreateAsync(order)) // → Result<Invoice>
        .MapAsync(invoice => invoice.ToDto());           // → Result<InvoiceDto>
}

private Result<Order> ValidateOrderForInvoice(Order order)
{
    if (order.Status != OrderStatus.Completed)
        return Result<Order>.Invalid(
            new ValidationError("Status", "Only completed orders can be invoiced.", "ORDER_STATUS"));

    return Result<Order>.Success(order);
}
```

### CQRS query handler with pagination

```csharp
public async Task<PagedResult<List<ProductDto>>> Handle(GetProductsQuery query, CancellationToken ct)
{
    try
    {
        var (items, total) = await _repository.GetPagedAsync(query.Page, query.PageSize, ct);

        var pagedInfo = new PagedInfo(
            pageNumber:   query.Page,
            pageSize:     query.PageSize,
            totalPages:   (long)Math.Ceiling(total / (double)query.PageSize),
            totalRecords: total
        );

        return PagedResult<List<ProductDto>>.Success(pagedInfo, items.Select(p => p.ToDto()).ToList());
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving products");
        return PagedResult<List<ProductDto>>.CriticalError("Error retrieving products.");
    }
}
```

### Minimal API endpoint with Match

```csharp
app.MapGet("/orders/{id}", async (int id, IOrderService service) =>
{
    var result = await service.GetOrderAsync(id);

    return result.Match(
        onSuccess: data => Results.Ok(data),
        onFailure: r    => r.Status switch
        {
            ResultStatus.NotFound    => Results.NotFound(r.Errors),
            ResultStatus.Forbidden   => Results.Forbid(),
            ResultStatus.Unauthorized=> Results.Unauthorized(),
            ResultStatus.Invalid     => Results.UnprocessableEntity(r.ValidationErrors),
            _                        => Results.Problem(string.Join(", ", r.Errors))
        });
});
```

### Validation with multiple errors

```csharp
public Result<User> CreateUser(CreateUserRequest request)
{
    var errors = new List<ValidationError>();

    if (string.IsNullOrWhiteSpace(request.Name))
        errors.Add(new("Name", "Name is required.", "REQUIRED", ValidationSeverity.Error));

    if (!IsValidEmail(request.Email))
        errors.Add(new("Email", "Invalid email address.", "EMAIL_INVALID", ValidationSeverity.Error));

    if (errors.Count > 0)
        return Result<User>.Invalid(errors);

    var user = new User(request.Name, request.Email);
    return Result<User>.Success(user, "User created successfully.");
}
```

### Distributed tracing with CorrelationId

```csharp
public async Task<Result<PaymentDto>> ProcessPaymentAsync(PaymentRequest request, string traceId)
{
    var gatewayResult = await _gateway.ChargeAsync(request);

    if (gatewayResult.IsFailure)
        return Result<PaymentDto>.ErrorWithCorrelationId(traceId, "Payment gateway error.");

    return Result<PaymentDto>.Success(gatewayResult.Data!.ToDto());
}

// Caller preserves the correlation ID across the call chain:
var result = await ProcessPaymentAsync(request, HttpContext.TraceIdentifier);
if (result.IsFailure)
    logger.LogError("Payment failed. TraceId: {TraceId}", result.CorrelationId);
```

---

## Contributing

Contributions are welcome!

1. Fork the repository
2. Create a feature branch
3. Add tests for your changes
4. Submit a pull request

## License

MIT License — see [LICENSE](LICENSE) for details.
