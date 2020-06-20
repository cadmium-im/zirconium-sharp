using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using McMaster.NETCore.Plugins;
using Zirconium.Core.Models;
using Zirconium.Core.Modules.Interfaces;

namespace Zirconium.Core.Modules
{
    // Class which responsible for module managing (loading, initializing) and module lifetime cycle
    public class ModuleManager
    {
        private IList<IModuleAPI> _modules;
        private IHostModuleAPI _hostModuleAPI;
        private Mutex _moduleMutex;

        public ModuleManager(IHostModuleAPI hostModuleAPI)
        {
            _hostModuleAPI = hostModuleAPI;
            _modules = new List<IModuleAPI>();
            _moduleMutex = new Mutex();
        }

        public void LoadModules(string folderPath) {
            var loaders = new List<PluginLoader>();

            // create module loaders
            foreach (var dir in Directory.GetDirectories(folderPath))
            {
                var dirName = Path.GetFileName(dir);
                var pluginDll = Path.Combine(dir, dirName + ".dll");
                if (File.Exists(pluginDll))
                {
                    var loader = PluginLoader.CreateFromAssemblyFile(
                        pluginDll,
                        sharedTypes: new[] { 
                                                typeof(IModuleAPI),
                                                typeof(IHostModuleAPI),
                                                typeof(IC2SMessageHandler),
                                                typeof(ICoreEventHandler),
                                                typeof(BaseMessage),
                                                typeof(CoreEvent)
                                            }
                    );
                    loaders.Add(loader);
                }
            }

            // Create an instance of module types
            foreach (var loader in loaders)
            {
                foreach (var pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(IModuleAPI).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    // This assumes the implementation of IPlugin has a parameterless constructor
                    IModuleAPI module = (IModuleAPI)Activator.CreateInstance(pluginType);
                    Console.WriteLine($"Created module instance '{module.GetModuleUniqueName()}'.");
                    module.Initialize(_hostModuleAPI);
                    _modules.Add(module);
                }
            }
        }
    }
}