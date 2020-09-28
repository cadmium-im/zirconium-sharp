namespace Zirconium.Core.Plugins.IPC
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class ExportedIPCMethodAttribute : System.Attribute
    {
        public string MethodName { get; private set; }

        public ExportedIPCMethodAttribute(string name)
        {
            this.MethodName = name;
        }
    }
}