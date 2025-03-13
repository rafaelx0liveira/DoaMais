using Microsoft.Extensions.DependencyInjection;
using DoaMais.MessageBus.Configuration;
using DoaMais.MessageBus.Interface;

namespace DoaMais.MessageBus.Extensions
{
    /// <summary>
    /// Extension methods for configuring RabbitMQ in the dependency injection container.
    /// </summary>
    public static class AddRabbitMQExtensions
    {
        /// <summary>
        /// Registers RabbitMQ in the dependency injection container.
        /// </summary>
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services,
            string hostname,
            string username,
            string password)
        {
            var rabbitMQSettings = new RabbitMQSettings
            {
                HostName = hostname,
                UserName = username,
                Password = password
            };

            services.AddSingleton(rabbitMQSettings);

            services.AddSingleton<IMessageBus, RabbitMQMessageBus>();

            return services;
        }
    }
}
