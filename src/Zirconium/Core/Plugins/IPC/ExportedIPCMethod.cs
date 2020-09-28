using System.Collections.Generic;
using System.Reflection;
namespace Zirconium.Core.Plugins.IPC
{
    public class ExportedIPCMethod
    {
        public object Service { get; set; }
        public string MethodName { get; set; }
        public MethodInfo Method { get; set; }
    }
}