using System.Net.Http;

namespace TechTalk.SpecFlow.Analytics
{
    public class HttpClientWrapper
    {
        private HttpClient httpClient;

        public HttpClient HttpClient => httpClient ??= new HttpClient();
    }
}