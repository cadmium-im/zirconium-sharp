using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using McMaster.NETCore.Plugins;
using Zirconium.Core.Models;
using Zirconium.Core.Plugins.Interfaces;

namespace Zirconium.Core.Plugins
{
    // Class which responsible for plugin managing (loading, initializing) and plugin lifetime cycle
    public class PluginManager
    {
        private IList<IPluginAPI> _plugins;
        private IPluginHostAPI _pluginHostAPI;
        private Mutex _pluginsMutex;

        public PluginManager(IPluginHostAPI hostModuleAPI)
        {
            _pluginHostAPI = hostModuleAPI;
            _plugins = new List<IPluginAPI>();
            _pluginsMutex = new Mutex();
        }

        public void LoadPlugins(string folderPath, string[] enabledPlugins)
        {
            var loaders = new List<PluginLoader>();
            if (folderPath == "")
            {
                Logging.Log.Warning("Plugins folder path is not specified!");
                return;
            }

            // create module loaders
            foreach (var dir in Directory.GetDirectories(folderPath))
            {
                var dirName = Path.GetFileName(dir);
                if (enabledPlugins.Where(x => x == dirName).FirstOrDefault() == null) {
                    continue;
                }
                var pluginDll = Path.Combine(dir, dirName + ".dll");
                if (File.Exists(pluginDll))
                {
                    Logging.Log.Debug("found plugin " + dirName);
                    Logging.Log.Debug("try to init plugin " + dirName);
                    var loader = PluginLoader.CreateFromAssemblyFile(
                        pluginDll,
                        sharedTypes: new[] {
                                                    typeof(IPluginAPI),
                                                    typeof(IPluginHostAPI),
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
                    .Where(t => typeof(IPluginAPI).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    // This assumes the implementation of IPlugin has a parameterless constructor
                    IPluginAPI plugin = (IPluginAPI)Activator.CreateInstance(pluginType);
                    Logging.Log.Debug($"Created plugin instance '{plugin.GetPluginUniqueName()}'.");
                    plugin.Initialize(_pluginHostAPI);
                    _plugins.Add(plugin);
                }
            }
        }
    }
}