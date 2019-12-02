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
 - [ ] GraphQL
 - [ ] Unit test
 - [ ] Kubernetes
 - [ ] Kafka
 - [ ] Azure service bus
 - [ ] Introduce Correlation ID for tracking events


## Run with Docker
This can be run with `docker-compose`.
Simply go to the [Compose](/Compose) folder and run `docker-compose up --build`.

This will start up SQL server, MongoDB, Elasticsearch, Kibana, APM server, RabbitMQ and Consul.

After that you can run the services.

## Architecture
![Micro services architecture](microservices_architecture.png "Micro services archivecture")

The architecture shows that there is one public API (API gateway). This is accessible for the clients.

Next is the GraphQL service. The GraphQL resolves to one or more micro services via HTTP Rest.

Each micro service hosts its own API that is only accessible via the Public API or GraphQL. Each micro service are within its own domain and have directly access to its own dependencies such as databases, stores, etc. This also means that a micro service can have zero, one or more database (mssql, postgres, mongo, etc.). All these dependencies are not accessible for other micro services. In fact micro services are decoupled from each other.
Micro services are event based which means they can publish and/or subscribe to any events. By doing so one or more micro services can publish an event which can be received by one or more micro services unknown for the publisher.

Events are used when something has happened (eg. when a user was created the event could be called UserCreated).

RabbitMQ is used for publish/subscribe in order to deliver a message to multiple consumers.
Outbox has also been added to make sure we save the messages before they are published via RabbitMQ (in case RabbitMQ is not running or they just can't be published). Outbox is set up to MongoDB.

## Structure
- **Api**: Api gateway and graphql api.
- **Compose**: Docker compose to set up all surroundings (eg. RabbitMQ, SQL Server, etc.)
- **Src**: All micro services in their own solution.
- **Src/Events**: Contain all events sent through the event bus.
- **Infrastructure**: Infrastructure for micro services (eg. setup RabbitMQ, Consul, Logging, etc.)

### Api
Api gateway is using Ocelot to have a unified point of entry to all micro services.


### Micro service
Micro services are just REST APIs and are created without any knowledge to each other. If a micro service wants to notify another service it simply publishes an event and the services subscribing to this event can take action on it using publish/subscribe with RabbitMQ.


