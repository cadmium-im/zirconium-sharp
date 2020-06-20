using System.Collections.Generic;

namespace Zirconium.Core.Models
{
    public class CoreEvent
    {
        public string Name { get; set; }
        public IDictionary<object, object> Payload { get; set; }
        public string SourceModuleName { get; set; }
    }
}