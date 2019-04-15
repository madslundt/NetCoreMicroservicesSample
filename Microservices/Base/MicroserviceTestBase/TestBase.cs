using AutoFixture;
using MicroserviceBase.Validation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StructureMap;
using System;

namespace TestBase
{
    public class TestBase<T> where T : DbContext, IDisposable
    {
        protected readonly IMediator _mediator;
        protected readonly T _db;
        protected readonly Mock<IBackgroundJobClient> _jobClientMock;
        protected readonly Fixture _fixture;

        public TestBase()
        {
            var services = new ServiceCollection();

            // Services
            services.AddMediatR();
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddMvc().AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining(typeof(TestBase<T>)); });


            // Database
            _db = (T) Activator.CreateInstance(typeof(T), DatabaseContextMock<T>.InMemoryDatabase());


            // Global objects
            _jobClientMock = new Mock<IBackgroundJobClient>();
            _jobClientMock.Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<EnqueuedState>()));

            _fixture = new Fixture();


            IContainer container = new Container(cfg =>
            {
                cfg.For<IBackgroundJobClient>().Use(_jobClientMock.Object);
                cfg.For<T>().Use(_db);
                cfg.For(typeof(ILogger<>)).Use(typeof(NullLogger<>));
                cfg.Populate(services);
            });

            _mediator = container.GetInstance<IMediator>();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
