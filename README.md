# WIP NetCoreMicroservices

Using
 - Consul: Service discovery.
 - Ocelot: API gateway.
 - MediatR: CQRS pattern used in micro services.
 - dotnet-graphql: GraphQL.
 - FluentValidation: Request validation in microservices.
 - FluentAssertion: Fluent assertions in microservices test.
 - Hangfire: Background worker.
 - Microsoft.EntityFrameworkCore: Object-relational mapping.
 - Microsoft.Extensions.Logging: Logging API that allow other providers.
 - Moq: Mocking framework used for testing.
 - Sentry.io: Logging provider.
 - StructureMap: IoC Container.
 - Swagger: API documentation page.
 - Xunit: Testing framework.


## Structure
 - Api: Api gateway and graphql api.
 - Base: Base startup class containing metrics and logging.
 - Microservices: All micro services in their own solution.
   - Base: Base startup class and test class extending Base in root.

### Api
Api gateway is using Ocelot to have a unified point of entry to all micro services.

#### GraphQL
GraphQL server can be integrated directly in the Api gateway with Ocelot, or having GraphQL servers independently of Ocelot.
By doing the first this adds some logic to the Api gateway but avoid Ocelot having an extra hop to GraphQL.
By doing the later the GraphQL server can easily scale but requires an extra setup for GraphQL.

### Microservice
Micro services are just REST APIs and are  created without any knowledge to each other. If a micro service wants to notify another service it simply publishes an event and the services subscribing this event can take action on it.
They can be customized as you like it but in this sample each micro service is set up with CQRS pattern using Mediator.

## Development
Run Consul via Docker

```docker run -d --name=dev-consul --net=host -e CONSUL_BIND_INTERFACE=eth0 consul```

*Remember to check if your interface is eth0*

This runs Consul in-memory server agent with default bridge networking and no services exposed on the host.


