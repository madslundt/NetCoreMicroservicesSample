using DataModel;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using UsersService;
using UsersService.Infrastructure.Pipeline;

namespace UnitTest
{
    public class TestBase : IDisposable
    {
        protected readonly IMediator _mediator;
        protected readonly DatabaseContext _db;

        public TestBase()
        {
            var services = new ServiceCollection();

            // Services
            services.AddMediatR(typeof(Startup));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddMvc().AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Startup>(); });


            // Database
            var databaseName = Guid.NewGuid().ToString();
            _db = new DatabaseContext(DatabaseContextMock<DatabaseContext>.InMemoryDatabase());


            // Service provider
            var serviceProvider = services.BuildServiceProvider();
            _mediator = serviceProvider.GetService<IMediator>();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
