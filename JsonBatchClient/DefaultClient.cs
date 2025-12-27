using System.Text;

namespace JsonBatchClient.Base
{
    public abstract class DefaultClient : IBatchClient, ITransactionClient
    {
        private readonly HttpClient _httpClient;
        public DefaultClient(HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
            _httpClient = httpClient;
        }
        public virtual async Task<BatchResponse> ExecuteBatchAsync(BatchRequest request, string requestUri = "/batch")
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Operations, nameof(request.Operations));


            string jsonPayload = SerializeBatchRequest(request);

            using (var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
            {
                using (HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri))
                {
                    using (HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            string responseContent = await httpResponse.Content.ReadAsStringAsync();
                            return DeserializeBatchResponse(responseContent);
                        }
                        else
                        {
                            string responseContent = await httpResponse.Content.ReadAsStringAsync();
                            throw new HttpRequestException($"Request failed with status code {httpResponse.StatusCode} and body {responseContent}");
                        }
                    }
                }
            }
        }

        public virtual async Task<BatchResponse> ExecuteTransactionAsync(BatchRequest request, string requestUri = "/transaction")
        {
            return await ExecuteBatchAsync(request, requestUri);
        }

        protected abstract string SerializeBatchRequest(BatchRequest request);

        protected abstract BatchResponse DeserializeBatchResponse(string jsonResponse);
    }
}
