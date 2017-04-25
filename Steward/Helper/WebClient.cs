using System;
using System.Net;
using System.Threading.Tasks;

namespace Steward.Helper
{
    internal class WebClient : IWebClient
    {
        private System.Net.WebClient webClient;

        internal WebClient()
        {
            webClient = new System.Net.WebClient();
        }

        #region IDisposable Support

        private bool disposedValue;

        WebHeaderCollection IWebClient.Headers
        {
            get
            {
                return webClient.Headers;
            }

            set
            {
                webClient.Headers = value;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                webClient.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        Task<string> IWebClient.UploadStringTaskAsync(Uri address, string data)
        {
            return webClient.UploadStringTaskAsync(address, data);
        }
        #endregion
    }
}