using System.Net;

namespace CountryExplorer.Tests
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;

        public string Input { get; private set; }
        public Uri RequestUri { get; private set; }

        public MockHttpMessageHandler(string response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Input = await request.Content?.ReadAsStringAsync();
            RequestUri = request.RequestUri;

            return new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_response),
                RequestMessage = request
            };
        }
    }

}
