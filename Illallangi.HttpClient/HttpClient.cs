using System;
using System.IO;
using System.Net;
using System.Security;
using Ninject.Extensions.Logging;

namespace Illallangi
{
    public sealed class HttpClient : IHttpClient
    {
        #region Fields

        private readonly ILogger currentLogger;
        private readonly IHttpClientConfig currentConfig;

        #endregion

        #region Constructors

        public HttpClient()
            :this(null, null)
        {
        }

        public HttpClient(ILogger logger)
            :this(null, logger)
        {
        }

        public HttpClient(IHttpClientConfig config)
            : this(config, null)
        {
        }
        
        public HttpClient(IHttpClientConfig config, ILogger logger)
        {
            this.currentLogger = logger ?? new NullLogger(typeof(HttpClient));
            this.currentConfig = config ?? new NullHttpClientConfig();
            this.Logger.Debug("Constructor Complete");
        }

        #endregion

        #region Methods

        public string HttpGet(string uri, string accept = "", string proxy = "")
        {
            var cacheFile = Path.Combine(this.Config.CachePath, string.Format(@"{0}.{1}", uri.ToMd5Hash(), "json"));
            if (File.Exists(cacheFile))
            {
                this.Logger.Debug("Cache Hit on uri {0}, file {1}", uri, cacheFile);
                return File.ReadAllText(cacheFile);
            }

            this.Logger.Debug("Cache Miss on uri {0}, file {1}", uri, cacheFile);

            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest) WebRequest.Create(uri);
            }
            catch (NotSupportedException e)
            {
                this.Logger.ErrorException("NotSupportedException", e);
            }
            catch (ArgumentNullException e)
            {
                this.Logger.ErrorException("ArgumentNullException", e);
            }
            catch (SecurityException e)
            {
                this.Logger.ErrorException("SecurityException", e);
            }
            catch (UriFormatException e)
            {
                this.Logger.ErrorException("UriFormatException", e);
            }
            if (null == request)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(accept))
            {
                request.Accept = accept;
            }
            if (!string.IsNullOrWhiteSpace(proxy))
            {
                request.Proxy = new WebProxy(proxy);
            }

            WebResponse response = null;
            try
            {
                response = request.GetResponse();
            }
            catch (ProtocolViolationException e)
            {
                this.Logger.ErrorException("ProtocolViolationException", e);
            }
            catch (WebException e)
            {
                this.Logger.ErrorException("WebException", e);
            }
            catch (InvalidOperationException e)
            {
                this.Logger.ErrorException("InvalidOperationException", e);
            }
            catch (NotSupportedException e)
            {
                this.Logger.ErrorException("NotSupportedException", e);
            }
            if (null == response)
            {
                return null;
            }


            Stream responseStream = null;

            try
            {
                responseStream = response.GetResponseStream();
            }
            catch (NotSupportedException e)
            {
                this.Logger.ErrorException("NotSupportedException", e);
            }
            if (null == responseStream)
            {
                return null;
            }

            var sr = new StreamReader(responseStream);

            File.WriteAllText(cacheFile, sr.ReadToEnd().Trim());
            this.Logger.Debug("HTTP GET succedded, {0} bytes.", response.ContentLength);

            return File.ReadAllText(cacheFile);
        }

        #endregion

        #region Properties

        private ILogger Logger
        {
            get { return this.currentLogger; }
        }

        private IHttpClientConfig Config
        {
            get { return this.currentConfig; }
        }

        #endregion
    }
}
