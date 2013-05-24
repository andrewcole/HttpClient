using System;

namespace Illallangi
{
    public class NullHttpClientConfig : IHttpClientConfig
    {
        public string CachePath
        {
            get { return Environment.ExpandEnvironmentVariables(@"%temp%\Illallangi.HttpClient").CreatePath(); } 
        }
    }
}