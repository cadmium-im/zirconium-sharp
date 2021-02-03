using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using Zirconium.Utils;

namespace Zirconium.Core.Plugins.IPC
{
    public class IPCRouter
    {
        private IDictionary<string, IList<ExportedIPCMethod>> methodTable = new Dictionary<string, IList<ExportedIPCMethod>>();

        public void RegisterIPCService(string pluginName, object service)
        {
            Type t = service.GetType();
            var exportedMethods = t.GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(ExportedIPCMethodAttribute), false).Length > 0)
                    .ToArray();
            foreach (var m in exportedMethods)
            {
                var attr = (ExportedIPCMethodAttribute)m.GetCustomAttributes(typeof(ExportedIPCMethodAttribute), false).First();
                var exportedMethod = new ExportedIPCMethod();
                exportedMethod.Service = service;
                exportedMethod.MethodName = attr.MethodName;
                exportedMethod.Method = m;
                if (methodTable.GetValueOrDefault(pluginName, null) == null)
                {
                    methodTable[pluginName] = new List<ExportedIPCMethod>();
                }
                methodTable[pluginName].Add(exportedMethod);
            }
        }

        public Task<dynamic> MakeRequest(string pluginName, string methodName, dynamic paramsObject)
        {
            return Task.Factory.StartNew<dynamic>(() =>
            {
                var method = methodTable[pluginName].FirstOrDefault(x => x.MethodName == methodName);
                object returnValue = null;
                try
                {
                    returnValue = method?.Method.Invoke(method.Service, new object[] { paramsObject });
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
                return returnValue;
            });
        }

        public Task MakeNotif(string pluginName, string methodName, dynamic paramsObject)
        {
            return Task.Factory.StartNew(() =>
            {
                var method = methodTable[pluginName].Where(x => x.MethodName == methodName).FirstOrDefault();
                try
                {
                    method.Method.Invoke(method.Service, new object[] { paramsObject });
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            });
        }
    }
}