using BobTheBot.Kernel;
using RJ.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BobTheBot
{
    public class ErrorTypesRepository : IErrorTypesRepository
    {
        private Dictionary<Enum, string> errorTypeMappings = new Dictionary<Enum, string>()
        {
            { ErrorType.NONE, "An unexpected error has occurred." },
            { ErrorType.MissingRequiredParameter, "Missing required parameter '{0}'." },
            { ErrorType.NotFound, "{0} with parameter '{1}' was not found" },
            { ErrorType.NullReference,"The '{0}' has no value." }
        };

        private Dictionary<Type, Dictionary<Enum, string>> typedErrorTypeMappings = new Dictionary<Type, Dictionary<Enum, string>>()
        {

        };

        public IDictionary<Enum, string> GetErrorTypeMappings()
        {
            return errorTypeMappings;
        }

        public IDictionary<Type, Dictionary<Enum, string>> GetTypedErrorTypeMappings()
        {
            return typedErrorTypeMappings;
        }
    }
}
