using RabbitMQ.Client;

namespace RabbitMQ.Common
{
    public class ConnectionFactoryProvider
    {
        public static ConnectionFactory Get()
        {
            return new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
        }
    }
}
