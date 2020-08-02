# NetCoreMicroservices


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

Events are used when something did happen (eg. when a user was created the event could be called UserCreated).

RabbitMQ is used for publish/subscribe in order to deliver a message to multiple consumers.
Outbox has also been added to make sure we save the messages before they are published via RabbitMQ (in case RabbitMQ is not running or it can't be reached). Outbox is set up to MongoDB.
Messages can either be deleted afterwards or kept in MongoDB.

Event sourcing is also possible. This will divide each microservice into having a write and read model.

## Structure
- **Api**: Api gateway.
- **Compose**: Docker compose to set up all surroundings (eg. RabbitMQ, SQL Server, etc.)
- **Src/\*Service**: All microservices in their own solution.
- **Src/Events**: Contain all events sent to the event bus.
- **Infrastructure**: Infrastructure for microservices (eg. setup RabbitMQ, Consul, Logging, etc.)

### Infrastructure
Infrastructure contains the logic for the different services and keeps most of the boiler code away from our services.

#### Consul
Consul is used as service discovery. This is used by the services and the API gateway in order to call other services by name/id, rather than by uri.

Appsettings for consul looks like this
```
{
    "consulOptions": {
        "Id": "",
        "Name": "",
        "Tags": [],
        "Address": ""
    }
}
```

- **Id** is optional and is by default a random UUIDv4 appended with the port from the address.
- **Name** is optional and is by default the name of the application domain.
- **Tags**: is optional and is by default empty.
- **Address** is required and must include uri schema, host and port (eg. http://localhost:8500).


#### Core
Core contain basic functionalities and must be imported most of the other services to work well. Core functionalities are:

- **Commands** contain a mediator wrapper for command, command handler and command bus.
- **Events** contain a mediator wrapper for event, event handler and event bus.
- **Queries** contain a mediator wrapper for query, query handler and query bus.
- **ExceptionFilter** contain logging exceptions, add error message to http response, and translating exceptions to http status codes.
- **Mapping** contain functionality to map one object into a class.
- **ValidationBehavior** contain validation and is added as a mediator pipeline. This makes sure to run validation if such is added to the request.

#### Eventstores
Event store is a database where all events published in the application are stored. This is used with eventsourcing and will be a write model for the application.

At the moment the only eventstore supported is *MongoDb*.

Besides that it also contain interfaces and abstract classes:
- **Aggregates** ...
- **Projections** ...
- **Repository** is the repository for the aggregates. This makes sure to store the event to the event store and publish the event to the message broker (if Outbox is added to the service it will use Outbox).

- **Core**: Contain core functionalities and must be imported for other services to work well. This contain aggregate, command, query, and event type, global exception filter

##### MongoDb appsettings
```
{
    "MongoDbEventStoreOptions": {
        "DatabaseName": "",
        "CollectionName": "",
        "ConnectionString": ""
    }
}
```

- **DatabaseName** is optional and is by default 'EventStore'
- **CollectionName** is optional and is by default 'Events'.
- **ConnectionString**: is required and must include uri schema, host and port (eg. mongodb://localhost:27017)

#### Logging
Logging is using Serilog and ELK APM.

#### MessageBrokers
Message broker is used to publish and subscribe to events across services. This is to allow services send events to each other.

At the moment the only message brokers supported is *RabbitMQ*

#### Outbox
Outbox is used to store events before they are published to the message broker. The events are either removed after being published to the message broker or kept with the *processed* property set to the datetime, in UTC, it was published to the message broker.

This sounds very familiar with the event store but Outbox is primarily to avoid connection problems with the message broker (slow connection, bad connection, etc.).
That also means that the Outbox database should be hosted very close to the service.

In order to publish events to the message broker a hosted service is running in Outbox to look for unpublished events in the interval of every 2 second.

#### Swagger
Swagger is used for API documentation.

### Api
Api gateway is using Ocelot to have a unified point of entry to all microservices.

### Microservice
Microservices are just REST APIs and are created without any knowledge to each other. If a microservice wants to notify another service it simply publishes an event and the services subscribing to this event can take action on it using publish/subscribe with RabbitMQ.

A microservice consists of:
 - **Controllers**: API endpoints for the microservice.
 - **Queries**: Queries used to get data when client wants to read data.
 - **Commands**: Commands used to write data when client wants to modify data.
 - **EventHandlers**: Handlers for events to take action when events are being published.
 - **Repository**: Repository used when writing to the application. This will also publish the correct events.

Next to the microservice is the data model. This contain the migrations, models and update handlers (if used) for the database.

