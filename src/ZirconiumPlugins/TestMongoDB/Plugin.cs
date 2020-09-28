using System.Dynamic;
using Zirconium.Core.Plugins.Interfaces;

namespace TestMongoDB
{
    class Plugin : IPluginAPI
    {

        public string GetPluginUniqueName()
        {
            return "TestMongoDB";
        }

        public async void Initialize(IPluginHostAPI pluginHostAPI)
        {
            var tm = new TestModel();
            tm.ABC = "qweqowie";
            dynamic paramsObject = new ExpandoObject();
            paramsObject.ColName = "test_model";
            paramsObject.Model = tm;
            await pluginHostAPI.MakeIPCRequest("MongoDB", "Insert", paramsObject);
        }

        public void PreInitialize(IPluginManager pluginManager)
        {
            pluginManager.Depends(this, "MongoDB");
        }
    }

    class TestModel
    {
        public string ABC { get; set; }
    }
}
