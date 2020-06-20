using YamlDotNet.Serialization;

namespace Zirconium.Core
{
    public class Config
    {
        // A list of enabled plugins (or extensions) in server
        [YamlMember(Alias = "enabledPlugins")]
        public string[] EnabledPlugins {get; set;}

        // Server domain names (e.g. example.com)
        [YamlMember(Alias = "serverDomains")]
        public string[] ServerDomains {get; set;}

        // Path to directory with plugin assemblies
        [YamlMember(Alias = "pluginsDirPath")]
        public string PluginsDirPath {get; set;}

        // ID of this server in terms of Cadmium federation network
        [YamlMember(Alias = "serverID")]
        public string ServerID {get; set;}
    }
}