using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQApp.Models
{
    public class BaseMessage
    {
        public DateTime Timestamp { get; set; }
    }

    public class FruitMessage : BaseMessage
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UserMessage : BaseMessage
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
