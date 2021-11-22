using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmanApi.Interfaces
{
    public interface IMongoDBSettings
    {
        string DefaultConnection { get; set; }
        string Database { get; set; }
        string Collection { get; set; }
    }
}
