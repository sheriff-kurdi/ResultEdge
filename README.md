SharpResult

SharpResult is a lightweight, expressive Result pattern implementation for .NET that helps you write clear, predictable,
and exception-free business logic.

It enables you to model success and failure explicitly, improving readability, testability, and correctness across your
application.

✨ Features

Simple and expressive Result and Result<T> types

Explicit success / failure flow

No exceptions for control flow

Immutable and thread-safe

Zero dependencies

Designed for clean architecture & DDD

Works perfectly with async code

📦 Installation
dotnet add package SharpResult

🚀 Basic Usage
Creating results
var success = Result.Success();
var failure = Result.Failure("Something went wrong");

Generic result
Result<int> result = Result.Success(42);

if (result.IsSuccess)
{
Console.WriteLine(result.Value);
}


🎯 Why SharpResult?

Avoids exceptions as control flow

Encourages explicit, predictable code paths

Improves readability in complex business logic

Works perfectly with Clean Architecture & CQRS

Minimal API surface — no magic, no hidden behavior

🧪 Testing Friendly

SharpResult is designed to be easy to test:

result.IsFailure.Should().BeTrue();
result.Error.Should().Be("Invalid input");

📂 Project Structure
SharpResult/
├── SharpResult/
└── SharpResult.Tests/

🤝 Contributing

Contributions are welcome!

Fork the repo

Create a feature branch

Add tests

Submit a PR

📄 License

MIT License