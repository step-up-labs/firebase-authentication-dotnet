using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using Windows.ApplicationModel;
using Firebase.Auth.UI.Helpers;
using System.Reflection;
using System.Diagnostics;


namespace Firebase.Auth.UI.Repository
{
    internal class LocalSettingsWrapper
    {
        private const string _defaultLocalSettingsFile = "LocalSettings.json";
        private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private readonly string _applicationDataFolder;
        private readonly string _localsettingsFile;

        private IDictionary<string, object> _settings;

        private bool _isInitialized;

        //make as singletion
        private static LocalSettingsWrapper _instance;
        public static LocalSettingsWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalSettingsWrapper();
                }

                return _instance;
            }
        }
        public LocalSettingsWrapper()
        {
            var process = Process.GetCurrentProcess();
            string mainExecutablePath = process.MainModule.FileName;
            string appName = System.IO.Path.GetFileNameWithoutExtension(mainExecutablePath);

            _applicationDataFolder = System.IO.Path.Combine(_localApplicationData, $"{appName}Data");
            _localsettingsFile = _defaultLocalSettingsFile;

            _settings = new Dictionary<string, object>();
        }

        public bool Exists(string key)
        {
            if (RuntimeHelper.IsMSIX)
            {
                return ApplicationData.Current.LocalSettings.Values.ContainsKey(key);
            }
            else
            {
                return _settings.ContainsKey(key);
            }
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                //_settings = await Task.Run(() => _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile)) ?? new Dictionary<string, object>();

                string filepath = System.IO.Path.Combine(_applicationDataFolder, _localsettingsFile);
                if (System.IO.File.Exists(filepath))
                {
                    string content = await System.IO.File.ReadAllTextAsync(filepath).ConfigureAwait(false);
                    _settings = await Json.ToObjectAsync<IDictionary<string, object>>(content).ConfigureAwait(false);
                }
                else
                {
                    _settings = new Dictionary<string, object>();
                }

                _isInitialized = true;
            }
        }

        public async Task<T> ReadSettingAsync<T>(string key)
        {
            if (RuntimeHelper.IsMSIX)
            {
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
                {
                    return await Json.ToObjectAsync<T>((string)obj).ConfigureAwait(false);
                }
            }
            else
            {
                await InitializeAsync().ConfigureAwait(false);

                if (_settings != null && _settings.TryGetValue(key, out var obj))
                {
                    return await Json.ToObjectAsync<T>((string)obj).ConfigureAwait(false);
                }
            }

            return default;
        }

        public async Task SaveSettingAsync<T>(string key, T value)
        {
            if (RuntimeHelper.IsMSIX)
            {
                ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value).ConfigureAwait(false);
            }
            else
            {
                await InitializeAsync().ConfigureAwait(false);

                _settings[key] = await Json.StringifyAsync(value).ConfigureAwait(false);
                if (System.IO.Directory.Exists(_applicationDataFolder) == false)
                {
                    System.IO.Directory.CreateDirectory(_applicationDataFolder);
                }

                string filepath = System.IO.Path.Combine(_applicationDataFolder, _localsettingsFile);
                await System.IO.File.WriteAllTextAsync(filepath, await Json.StringifyAsync(_settings)).ConfigureAwait(false);


                //await Task.Run(() => _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings));
            }
        }
    }

}
