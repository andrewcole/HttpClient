using System;

namespace Illallangi
{
    public sealed class NullHttpClientConfig : IHttpClientConfig
    {
        public string CachePath
        {
            get { return Environment.ExpandEnvironmentVariables(@"%temp%\Illallangi.HttpClient").CreatePath(); } 
        }
    }
}