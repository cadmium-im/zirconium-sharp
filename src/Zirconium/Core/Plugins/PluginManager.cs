using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using McMaster.NETCore.Plugins;
using Zirconium.Core.Logging;
using Zirconium.Core.Models;
using Zirconium.Core.Plugins.Interfaces;
using Zirconium.Utils;

namespace Zirconium.Core.Plugins
{
    // Class which responsible for plugin managing (loading, initializing) and plugin lifetime cycle
    public class PluginManager : IPluginManager
    {
        private IDictionary<string, IPluginAPI> _plugins;
        private IPluginHostAPI _pluginHostAPI;
        private Mutex _pluginsMutex;
        private string _currentPluginFolderPath;

        public PluginManager(IPluginHostAPI hostModuleAPI)
        {
            _pluginHostAPI = hostModuleAPI;
            _plugins = new Dictionary<string, IPluginAPI>();
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

            _currentPluginFolderPath = folderPath;

            foreach (var dir in Directory.GetDirectories(folderPath))
            {
                var pluginName = Path.GetFileName(dir);
                if (enabledPlugins.Where(x => x == pluginName).FirstOrDefault() == null)
                {
                    continue;
                }

                var plugin = this.LoadPlugin(pluginName);
                _pluginsMutex.WaitOne();
                _plugins[pluginName] = plugin;
                _pluginsMutex.ReleaseMutex();
            }
        }

        public IPluginAPI LoadPlugin(string pluginName)
        {
            PluginLoader loader;
            var pluginDll = Path.Combine(_currentPluginFolderPath, pluginName, pluginName + ".dll");
            if (File.Exists(pluginDll))
            {
                Logging.Log.Debug("Found plugin " + pluginName);
                Logging.Log.Debug("Try to initialize plugin " + pluginName);
                loader = PluginLoader.CreateFromAssemblyFile(
                    pluginDll,
                    sharedTypes: new[] {
                                            typeof(Log),
                                            typeof(IPluginAPI),
                                            typeof(IPluginHostAPI),
                                            typeof(IPluginManager),
                                            typeof(IAuthProvider),
                                            typeof(IExposedSessionManager),
                                            typeof(IC2SMessageHandler),
                                            typeof(ICoreEventHandler),
                                            typeof(BaseMessage),
                                            typeof(CoreEvent)
                                        },
                    config => config.PreferSharedTypes = true
                );
            }
            else
            {
                throw new Exception("specified plugin is not found");
            }

            var pluginTypes = loader.LoadDefaultAssembly().GetTypes();

            IPluginAPI plugin = null;
            foreach (var pluginType in pluginTypes
                    .Where(t => typeof(IPluginAPI).IsAssignableFrom(t) && !t.IsAbstract))
            {
                // This assumes the implementation of IPlugin has a parameterless constructor
                plugin = (IPluginAPI)Activator.CreateInstance(pluginType);
                Logging.Log.Debug($"Created plugin instance '{plugin.GetPluginUniqueName()}'.");
                var exportedTypes = plugin.GetExportedTypes();
                if (exportedTypes != null)
                {
                    foreach (var type in exportedTypes)
                    {
                        var path = new Uri(type.Module.Assembly.CodeBase).LocalPath;
                        AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                    }
                }
                plugin.PreInitialize(this);
                plugin.Initialize(_pluginHostAPI);
            }
            return plugin;
        }

        public dynamic Depends(IPluginAPI currentPlugin, string pluginName)
        {
            var dependantPlugin = _plugins.GetValueOrDefault(pluginName, null);
            if (dependantPlugin != null) return dependantPlugin.GetExportedAPI();
            this.LoadPlugin(pluginName);
            dependantPlugin = _plugins[pluginName];
            return dependantPlugin.GetExportedAPI();
        }
    }
}