using EventSourcingDemo.Domain.Base;
using EventSourcingDemo.Domain.Product;
using EventSourcingDemo.Domain.Product.Events;
using EventSourcingDemo.Domain.PubSub;
using EventSourcingDemo.Domain.Warehouse;
using EventSourcingDemo.Domain.Warehouse.Events;
using EventSourcingDemo.EventStore;
using EventSourcingDemo.EventStore.Repository;
using EventSourcingDemo.Logic.Product;
using EventSourcingDemo.Logic.Warehouse;
using EventSourcingDemo.ReadModel;
using EventSourcingDemo.ReadModel.DatabaseContext;
using EventSourcingDemo.ReadModel.ReadModels;
using EventSourcingDemo.ReadModel.Repository;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
namespace EventSourcingDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //connection setting for evenstore connection
            var connectionSetting = ConnectionSettings.Create()
                                                      .KeepRetrying()
                                                      .KeepReconnecting()
                                                      .DisableTls()
                                                      .Build();

            //eventstore connection we connect to the eventstore
            //then send the only instance of the connection to the service
            var eventStoreConnection = EventStoreConnection.Create(connectionSetting, new Uri(Configuration["EvenstoreUrl"]));
            eventStoreConnection.ConnectAsync().Wait();

            services.AddSingleton(_ => eventStoreConnection);
            services.AddDbContext<EventSourcingReadModelDbContext>(_ =>
                                _.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddScoped<IEventStore, EventStore.EventStore>();
            services.AddScoped<IEventStoreRepository<WarehouseAggregate>, EventStoreRepository<WarehouseAggregate>>();
            services.AddScoped<IEventStoreRepository<ProductAggregate>, EventStoreRepository<ProductAggregate>>();
            services.AddScoped<IEventSubscriber, EventPubSub>();
            services.AddScoped<IEventPublisher, EventPubSub>();
            services.AddScoped<IWarehouseWriter, WarehouseWriter>();
            services.AddScoped<IWarehouseReader, WarehouseReader>();
            services.AddScoped<IProductWriter, ProductWriter>();
            services.AddScoped<IProductReader, ProductReader>();

            AddEventHandlers(services);
            AddRepositories(services);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventSourcingDemo", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            using (var context = scope.ServiceProvider.GetService<EventSourcingReadModelDbContext>())
                context.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventSourcingDemo v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void AddEventHandlers(IServiceCollection services)
        {
            services.AddTransient<IEventHandler<WarehouseCreatedEvent>, WarehouseUpdater>();
            services.AddTransient<IEventHandler<WarehouseNameChangedEvent>, WarehouseUpdater>();
            services.AddTransient<IEventHandler<ItemAddedToWarehouseEvent>, WarehouseUpdater>();
            services.AddTransient<IEventHandler<ExistingItemAddedToWarehouseEvent>, WarehouseUpdater>();
            services.AddTransient<IEventHandler<WithdrawnEvent>, WarehouseUpdater>();
            services.AddTransient<IEventHandler<ProductNameChangedEvent>, WarehouseUpdater>();
            services.AddTransient<IEventHandler<WarehouseDeletedEvent>, WarehouseUpdater>();
            services.AddTransient<IEventHandler<WarehouseItemDeletedEvent>, WarehouseUpdater>();

            services.AddTransient<IEventHandler<ProductCreatedEvent>, ProductUpdater>();
            services.AddTransient<IEventHandler<ProductNameChangedEvent>, ProductUpdater>();
            services.AddTransient<IEventHandler<ProductDeletedEvent>, ProductUpdater>();
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IWriteRepository<WarehouseReadModel>, Repository<WarehouseReadModel>>();
            services.AddScoped<IReadRepository<WarehouseReadModel>, Repository<WarehouseReadModel>>();

            services.AddScoped<IWriteRepository<ProductReadModel>, Repository<ProductReadModel>>();
            services.AddScoped<IReadRepository<ProductReadModel>, Repository<ProductReadModel>>();

            services.AddScoped<IWriteRepository<WarehouseItemReadModel>, Repository<WarehouseItemReadModel>>();
            services.AddScoped<IReadRepository<WarehouseItemReadModel>, Repository<WarehouseItemReadModel>>();
            services.AddScoped<IWarehouseItemRepository, WarehouseItemRepository>();
        }
    }
}
