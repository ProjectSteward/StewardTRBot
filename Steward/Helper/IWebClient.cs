using System;
using System.Net;
using System.Threading.Tasks;

namespace Steward.Helper
{
    internal interface IWebClient : IDisposable
    {
        WebHeaderCollection Headers { get; set; }

        Task<string> UploadStringTaskAsync(Uri address, string data);
    }
}