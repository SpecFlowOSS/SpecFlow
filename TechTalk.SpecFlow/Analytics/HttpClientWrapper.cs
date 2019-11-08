using System.Net.Http;

namespace TechTalk.SpecFlow.Analytics
{
    public class HttpClientWrapper
    {
        public HttpClientWrapper()
        {
            HttpClient = new HttpClient();
        }

        public HttpClient HttpClient { get; private set; }
    }
}