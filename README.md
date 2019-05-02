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

## Architecture

![Microservices architecture](microservices_architecture.png "Microservices archivecture")

The architecture shows that there is one public Api (Api gateway). This is accessible for the clients.

Next is the GraphQL service. The GraphQL resolves to one or more micro service(s) via HTTP Rest.

Each micro service has its own API that is only accessible from the Public API or GraphQL. Each micro service has one or more database (mssql, postgres, elastic search, etc.). That also means the micro services are decoupled from other micro services.
Micro services are event based which means they can publish or subscribe to any events. By doing so one or more micro services can publish an event which can be received by one or more micro services unknown for the publisher(s).

RabbitMQ is used in this example but can easily be swapped with any other tool just by editing _Microservices/Base_.

## Structure

- Api: Api gateway and graphql api.
- Base: Base startup class containing metrics and logging.
- Microservices: All micro services in their own solution.
  - Base: Base startup class and test class extending Base in root.

### Api

Api gateway is using Ocelot to have a unified point of entry to all micro services.


### Microservice

Micro services are just REST APIs and are created without any knowledge to each other. If a micro service wants to notify another service it simply publishes an event and the services subscribing this event can take action on it.
They can be customized as you like it but in this sample each micro service is set up with CQRS pattern using Mediator.

## Development

Run Consul via Docker

`docker run -d --name=dev-consul --net=host -e CONSUL_BIND_INTERFACE=eth0 consul`

_Remember to check if your interface is eth0_

This runs Consul in-memory server agent with default bridge networking and no services exposed on the host.
