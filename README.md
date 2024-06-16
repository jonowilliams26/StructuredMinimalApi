# Structured Minimal API
An example on how to structure a minmal API using [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/)

## Video Walkthrough
Prefer watching videos rather than reading the code? Check [this video out](https://www.youtube.com/watch?v=ZA2X1gaAhJk), it's a walkthrough of the solution and explains some concepts / reasoning

## What is Vertical Slice Architecture?
Vertical slice architecture is an approach for organising your code into features/vertical slices rather than organising by technical conerns (e.g. Controllers, Models, Services etc).
Each slice will contain all the code which fullfills a feature or use case inside the application.
One of the main benefits of VSA is the ability to structure each feature/slice independently, so each feature/slice can be as simple or complicated as it needs to be.

## What does this API do?
This is an API for Twitter/X like social media, where users can make text posts, like and comment on posts and follow other users.

## Some Important Design Decisions In This Project
1. Each `endpoint` will define their own `request`/`response` contract
    - I have found trying to resuse DTOs can be a pain as soon as an edge case requires the DTO to diverge from the common structure. Rather than dealing with that pain later, bite the bullet now and create a seperate `request`/`response` DTO.
2. Let **DATA BE DATA**
    - I'm not using [Domain Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html), after using DDD on real world projects, I personally, don't like the approach (if it works for you, keep doing it, don't let me change that), I found I was always trying to search for the code that was actually doing the thing, or trying to figure out which *Domain object* the code belongs too rather than writing the code.
    - After experimenting with [Golang](https://go.dev/) I really like the approach of letting data just be simple `structs` and using methods/functions to operate over them.
    - So, in this project our `Data Types` will just be simple data buckets, no logic inside them, they are just there to represent our data. Some people call this an [Anemic Domain Model](https://martinfowler.com/bliki/AnemicDomainModel.html)
3. No `IRepository` abstraction over [EF Core](https://learn.microsoft.com/en-us/ef/core/)
    - Controversial, I know. My take is EF Core is already a pretty solid abstraction over a database and covers 99.9% of use cases. Some people say "What about unit testing?", I think you shouldn't be mocking **YOUR OWN** database, if you need to test something which is reading/writing to your database, you should be writing an [integration test](https://en.wikipedia.org/wiki/Integration_testing)
    - If EF Core isn't suitable for a specific feature/slice, we can use anything we want (e.g. [Dapper](https://github.com/DapperLib/Dapper)) as each feature/slice is independent.
