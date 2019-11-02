# WIP NetCoreMicroservices

## Run with Docker
This can be run with `docker-compose`.
Simply just go to the [Compose](/Compose) folder and run `docker-compose up --build`.

This will start the **APIGateway**, **UsersService**, **MessagesService** and the tools for them to run.
You can only access APIGateway from outside on port 8080.

## Architecture

![Microservices architecture](microservices_architecture.png "Microservices archivecture")

The architecture shows that there is one public Api (Api gateway). This is accessible for the clients.

Next is the GraphQL service. The GraphQL resolves to one or more micro service(s) via HTTP Rest.

Each micro service has its own API that is only accessible from the Public API or GraphQL. Each micro service can have one or more database (mssql, postgres, etc.). That also means the micro services are decoupled from other micro services.
Micro services are event based which means they can publish or subscribe to any events. By doing so one or more micro services can publish an event which can be received by one or more micro services unknown for the publisher(s).

RabbitMQ is used for publish/subscribe in order to deliver a message to multiple consumers.

## Structure

- Api: Api gateway and graphql api.
- Src: All micro services in their own solution.

### Api

Api gateway is using Ocelot to have a unified point of entry to all micro services.


### Microservice

Micro services are just REST APIs and are created without any knowledge to each other. If a micro service wants to notify another service it simply publishes an event and the services subscribing to this event can take action on it.
