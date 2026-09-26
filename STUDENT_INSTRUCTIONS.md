# Lab: Fix the Service–Repository Web API

This ASP.NET Core Web API is supposed to follow the **Service–Repository pattern**,
but it has been broken. Your job is to find and fix every bug.

There are **29 bugs**. The project **compiles**, so the compiler won't find them for you.
Every bug is a runtime/logic bug, a wrong HTTP contract (route, verb, status code), or a
**design violation**: code sitting in the wrong layer. Some bugs only show up after you
fix another one, so re-test after every change.

## Do NOT change these (they are correct)

- `Models/Product.cs`
- `Data/AppDbContext.cs`
- `Program.cs`
- `appsettings.json` (except your own connection string)
- `ServiceRepoApi.csproj`
- `ServiceRepoApi.http` (the test requests; see below)

## Setup

1. Set `ConnectionStrings:DefaultConnection` in `appsettings.json`.
2. Run:

       dotnet ef migrations add InitialCreate
       dotnet ef database update
       dotnet run

3. Test with Swagger (`https://localhost:7081/swagger`) or with the requests in
   `ServiceRepoApi.http`. Each request is labelled with the result it should return
   once the API is fixed. Run them top to bottom on a fresh database.

## Architecture rules

    Controller  ->  IProductService  ->  IProductRepository  ->  AppDbContext

- **Controllers** depend only on `IProductService`. They must not use `AppDbContext` or
  contain business rules. They handle HTTP only: routes, verbs, binding, status codes.
- **Services** contain all business rules and report failures with `ServiceResult`.
- **Repositories** contain data access only (EF Core queries, add/update/remove, save).

## Required API contract

Base route: `/api/products`. Error responses have a JSON body `{ "error": "..." }`
(automatic validation errors use the standard ProblemDetails format).

| Request | Success | Failure |
|---|---|---|
| `GET /api/products` | 200, **all** products sorted by name A–Z | |
| `GET /api/products/{id}` | 200, the product | 404 if it doesn't exist |
| `POST /api/products` | 201 with a `Location` header and the created product | 400 invalid body, 409 duplicate name |
| `PUT /api/products/{id}` | 204 | 400 invalid body or route id ≠ body id, 404 not found, 409 duplicate name |
| `DELETE /api/products/{id}` | 204 | 404 not found, 409 if stock > 0 |

## Business rules

1. Leading and trailing spaces are removed from names on create **and** update.
2. Names must be **unique** (after trimming) on create **and** update.
3. The server assigns `Id`. An `id` sent in a POST body is ignored.
4. `CreatedAt` is set once, by the **service**, in **UTC**. A `createdAt` sent by the
   client (POST or PUT) is ignored, and updates never change it.
5. PUT saves every editable field: name, description, price, stock.
6. A product can only be deleted when its stock is exactly **0**.
7. Unexpected exceptions (500) must never be the answer to a normal request,
   such as asking for an id that doesn't exist.

## Tips

- Watch the status code, the headers and the body of every response, not just whether it "worked".
- Check the database directly (e.g. SSMS) to see what was really saved.
- Read the app's console log when you get a 500.
- For each bug you fix, write down the file, what was wrong, the symptom, and your fix.


BUG FIXES: 29
1. ProductsController.cs...The ApiController was missing...I Added ApiController for automatic validation.
2. ProductsController.cs...The base route became /api/products...Changed it into /api/products.
3. ProductsController.cs...Controller Directly used AppDbContext...Removed it and only used IProductService.
4. ProductsController.cs...GET all accessed the database directly...Changed it to call the service.
5. ProductsController.cs...GET-by-ID used the wrong route parameter...Changed the route parameter to {id}.
6. ProductsController.cs...Missing products did not return the correct result...Added a 404 Not Found response.
7. ProductsController.cs...POST contained business rules...Moved business rules to the service.
8. ProductsController.cs...POST returned 200 OK....Changed it to 201 Created with a Location header.      
9. ProductsController.cs...Update used POST instead of PUT...Changed the endpoint to [HttpPut].
10.ProductsController.cs...PUT did not compare route ID and body ID...Added a 400 check when IDs differ.
11.ProductsController.cs...DELETE route did not include an ID...Changed it to [HttpDelete("{id}")].
12.ProductsController.cs...DELETE directly accessed the database...Moved deletion to the service.
13.ProductsController.cs...Delete stock rule was reversed...Allowed deletion only when stock is.
14.ProductsController.cs...NotFound was returned as 409....Corrected it to 404.
15.ProductsController.cs...Conflict was returned as 404...Corrected it to 409.
16.ProductRepository.cs...No duplicate-name checking method existed...Added NameExistsAsync().
17.ProductRepository.cs...GET all returned only 10 products...Removed .Take(10).
18.ProductRepository.cs...Missing IDs caused an exception...Replaced SingleAsync() with SingleOrDefaultAsync().
19.ProductRepository.cs...Update marked the product as a new record...Changed it to a proper update operation.
20.IProductRepository.cs...Delete was missing from the service...Added DeleteAsync().
21.IProductRepository.cs...Conflict result used the wrong status...Corrected it to Conflict.
22.ProductRepository.cs...Create did not trim product names...Added Trim().
23.ProductRepository.cs...Create allowed duplicate names...Added a duplicate-name check.
24.ProductRepository.cs...Client-provided IDs were accepted...Reset the ID so the server assigns it.
25.ProductService.cs...CreatedAt was not properly assigned by the service...Set it using DateTime.UtcNow.
26.ProductService.cs...Updating a missing product returned success...Changed it to return 404.
27.ProductService.cs...Edit allowed duplicate names...Added duplicate checking during update.
28.ProductService.cs...Edit missed Description and changed CreatedAt...Saved all editable fields and preserved CreatedAt.
29.ProductService.cs...Delete business rules were missing...Added not-found, stock, delete, and save logic.
