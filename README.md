# Questao 5
This is a .NET API designed to handle financial transfers and account balance queries. It provides two main endpoints: one for registering transfers and another for retrieving the account balance. The API is built with modern practices and technologies, ensuring reliability, scalability, and maintainability.

# Features & Highlights
* Register Transfers: Allows registering financial transfers with validation for account status, type, and value.
* Account Balance: Retrieves the current balance of an account.
* Idempotency: The Register Transfer endpoint is idempotent, ensuring repeated requests do not impact the financial balance of the account.
* Comprehensive Error Handling: Detailed error responses for invalid inputs or account issues following standards standards from [Problem Details for HTTP APIs - RFC7807](https://datatracker.ietf.org/doc/html/rfc7807#section-3)
* Swagger Documentation: Fully documented API endpoints following proper HTTP response codes for easy testing and integration.
* Integration and Unit Tests: Thorough test coverage for endpoints, services, validators, and handlers.
* Global Exception Handling: Centralized exception handling for consistent error responses.
* Verify Tests (xUnit): Leverage [Verify](https://github.com/VerifyTests/Verify) for integration tests to simplify result verification with snapshot-based testing, ensuring consistent and reliable outputs.

# Technologies
* .NET 8: The API has been upgraded to the latest Long-Term Support (LTS) version of .NET.
* Dapper: Used for efficient database access and query execution.
* CQRS: Implements the Command Query Responsibility Segregation pattern for better separation of concerns.
* Mediator: MediatR library is used to implement the mediator pattern for handling requests and commands.
* Swagger: Provides API documentation and an interactive UI for testing endpoints.
* NSubstitute: Used for mocking dependencies in unit tests.

# Endpoints
1. Register Transfer
* Endpoint: POST /api/transfers
* Description: Registers a financial transfer for a specific account.
* Request Body:
```json
{
  "key": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "date": "2025-02-13",
  "value": 0,
  "type": "string"
}
```

* Responses:
    * 200 OK: Transfer registered successfully.
    * 400 BAD REQUEST:
        * `INVALID_TYPE`: Type is not "C" or "D".
        * `INVALID_ACCOUNT`: Account does not exist.
        * `INACTIVE_ACCOUNT`: Account is not active.
        * `INVALID_VALUE`: Value is less than or equal to zero.
        * `INVALID_KEY`: Key is empty or is the default Guid value.

2. Get Account Balance
* Endpoint: GET /api/accounts/{accountId}/balance
* Description: Retrieves the current balance of an account.
* Responses:
    * 200 OK: Returns the account balance.
    * 400 BAD REQUEST:
        * `INVALID_ACCOUNT`: Account does not exist.
        * `INACTIVE_ACCOUNT`: Account is not active.

# How To run
## API
1. `https://github.com/KevinGuedes/Exercises.git`
2. `cd Exercises`
3. `cd Questao5`
4. `dotnet run`
The API will be available on https://localhost:7140/swagger/index.html

## Unit Tests
Inside the main project folder (`Exercises` folder)
1. `cd Questao5.UnitTests`
2. `dotnet test`

## Integrations Tests
Inside the main project folder (`Exercises` folder)
1. `cd Questao5.IntegrationTests` 
2. `dotnet test`