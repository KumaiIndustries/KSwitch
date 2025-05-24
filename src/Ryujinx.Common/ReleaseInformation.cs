using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Ryujinx.Common
{
    public static class ReleaseInformation
    {
        private const string FlatHubChannelOwner = "flathub";
        private const string DefaultConfigName = "Config.json";
        private static readonly string _infoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "release.json");

        private static readonly ReleaseInfoData _data;

        // These are CI placeholder values. Used if release.json isn't found.
        private const string FallbackBuildVersion = "%%RYUJINX_BUILD_VERSION%%";
        private const string FallbackBuildGitHash = "%%RYUJINX_BUILD_GIT_HASH%%";
        private const string FallbackReleaseChannelName = "%%RYUJINX_TARGET_RELEASE_CHANNEL_NAME%%";
        private const string FallbackConfigFileName = "%%RYUJINX_CONFIG_FILE_NAME%%";
        private const string FallbackReleaseChannelOwner = "%%RYUJINX_TARGET_RELEASE_CHANNEL_OWNER%%";
        private const string FallbackReleaseChannelRepo = "%%RYUJINX_TARGET_RELEASE_CHANNEL_REPO%%";

        static ReleaseInformation()
        {
            try
            {
                if (File.Exists(_infoPath))
                {
                    string json = File.ReadAllText(_infoPath);
                    _data = JsonSerializer.Deserialize<ReleaseInfoData>(json) ?? new ReleaseInfoData();
                }
                else
                {
                    _data = GetFallbackData();
                }
            }
            catch
            {
                _data = GetFallbackData();
            }
        }

        private static ReleaseInfoData GetFallbackData() => new()
        {
            BuildVersion = FallbackBuildVersion,
            BuildGitHash = FallbackBuildGitHash,
            ReleaseChannelName = FallbackReleaseChannelName,
            ConfigFileName = FallbackConfigFileName,
            ReleaseChannelOwner = FallbackReleaseChannelOwner,
            ReleaseChannelRepo = FallbackReleaseChannelRepo
        };

        public static string ConfigName => !IsPlaceholder(_data.ConfigFileName) ? _data.ConfigFileName : DefaultConfigName;

        public static bool IsValid =>
            !IsPlaceholder(_data.BuildGitHash) &&
            !IsPlaceholder(_data.ReleaseChannelName) &&
            !IsPlaceholder(_data.ReleaseChannelOwner) &&
            !IsPlaceholder(_data.ReleaseChannelRepo) &&
            !IsPlaceholder(_data.ConfigFileName);

        public static bool IsFlatHubBuild => IsValid && _data.ReleaseChannelOwner.Equals(FlatHubChannelOwner, StringComparison.OrdinalIgnoreCase);

        public static string Version =>
            IsValid ? _data.BuildVersion :
            Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";

        public static string ReleaseChannelOwner => _data.ReleaseChannelOwner ?? string.Empty;
        public static string ReleaseChannelRepo => _data.ReleaseChannelRepo ?? string.Empty;

        private static bool IsPlaceholder(string? value) => string.IsNullOrEmpty(value) || value.StartsWith("%%");
    }

    internal class ReleaseInfoData
    {
        public string BuildVersion { get; set; } = "";
        public string BuildGitHash { get; set; } = "";
        public string ReleaseChannelName { get; set; } = "";
        public string ConfigFileName { get; set; } = "";
        public string ReleaseChannelOwner { get; set; } = "";
        public string ReleaseChannelRepo { get; set; } = "";
    }
}
