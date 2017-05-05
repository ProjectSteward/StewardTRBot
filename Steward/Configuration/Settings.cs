using System;
using System.Configuration;

namespace Steward.Configuration
{
    internal class Settings : ISettings
    {
        string ISettings.this[string name] => ConfigurationManager.AppSettings[name];
    }
}