# WIP NetCoreMicroservices

## Requirements
 - **Consul**: Service discovery
 - **Elasticsearch**: Storing logs, metrics, etc.
 - **EntityFramework**: Database ORM
 - **FluentValidator**: Validate requests
 - **Kibana**: Visualizing logs, metrics, etc.
 - **MediatR**: CQRS
 - **Mongo**: MongoDB driver
 - **RabbitMQ**: Event bus (publish/subscribe)
 - **Serilog**: Logging provider
 - **SQL server**: MSSQL database

## TODO
 - [x] Service discovery
 - [x] Logging
 - [x] Monitoring (APM)
 - [x] CQRS
 - [x] Validation
 - [x] RabbitMQ
 - [x] Data model
 - [x] API Gateway
 - [x] Outbox
 - [x] Swagger
 - [x] Unit test
 - [x] Introduce Correlation ID for tracking events
 - [ ] Kubernetes
 - [ ] Kafka
 - [ ] Azure service bus


## Run with Docker
This can be run with `docker-compose`.
Simply go to the [Compose](/Compose) folder and run `docker-compose up --build`.

This will start up SQL server, MongoDB, Elasticsearch, Kibana, APM server, RabbitMQ and Consul.

After that you can run the services with default `appsettings.json`.

## Architecture
![Microservices architecture](microservices_architecture.png "Microservices archivecture")

The architecture shows that there is one public API (API gateway). This is accessible for the clients.

Each microservice hosts its own REST API that is only accessible via its Public API. Each microservice are within its own domain and have directly access to its own dependencies such as databases, stores, etc. This also means that a microservice can have zero, one or multiple databases (mssql, postgres, mongo, etc.). All these dependencies are not accessible for other microservices. In fact microservices are decoupled from each other.
Microservices are event based which means they can publish and/or subscribe to any events. By doing so one or more microservices can publish an event which can be received by one or more microservices unknown for the publisher.

Events are used when something has happened (eg. when a user was created the event could be called UserCreated).

RabbitMQ is used for publish/subscribe in order to deliver a message to multiple consumers.
Outbox has also been added to make sure we save the messages before they are published via RabbitMQ (in case RabbitMQ is not running or it can't be reached). Outbox is set up to MongoDB.
Messages can either be deleted afterwards or kept in MongoDB.

By using the `BaseController` to send commands, events created and added in the request will be committed and published to the event bus.

## Structure
- **Api**: Api gateway.
- **Compose**: Docker compose to set up all surroundings (eg. RabbitMQ, SQL Server, etc.)
- **Src/\*Service**: All microservices in their own solution.
- **Src/Events**: Contain all events sent to the event bus.
- **Infrastructure**: Infrastructure for microservices (eg. setup RabbitMQ, Consul, Logging, etc.)

### Api
Api gateway is using Ocelot to have a unified point of entry to all microservices.E

### Microservice
Microservices are just REST APIs and are created without any knowledge to each other. If a microservice wants to notify another service it simply publishes an event and the services subscribing to this event can take action on it using publish/subscribe with RabbitMQ.

A microservice consists of:
 - **Controllers**: API endpoints for the microservice.
 - **Queries**: Queries used to get data when client wants to read data.
 - **Commands**: Commands used to write data when client wants to modify data.
 - **EventHandlers**: Handlers for events to take action when events are being published.
 - **Repository**: Repository used when writing to the application. This will also publish the correct events.

Next to the microservice is a data model. This contain the migrations and models for the database.