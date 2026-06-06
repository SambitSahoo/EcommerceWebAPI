using System;

namespace Order.Application.Exceptions
{
    public class OrderNotFoundException : ApplicationException
    {
        public OrderNotFoundException()
        {
        }

        public OrderNotFoundException(string name,object key)
            : base("Entity \"{name}\" ({key}) was not found.")
        {
        }

        public OrderNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}