using System.Text.Json;
using JsonBatchClient.Base;

namespace JsonBatchClient.Json
{
    public class DefaultClient: JsonBatchClient.Base.DefaultClient
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        DefaultClient(HttpClient httpClient,JsonSerializerOptions jsonSerializerOptions) : base (httpClient)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
        }
        protected override string SerializeBatchRequest(BatchRequest request)
        {
            return JsonSerializer.Serialize(request, _jsonSerializerOptions);
        }
        protected override BatchResponse DeserializeBatchResponse(string responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new ArgumentNullException(nameof(responseContent));
            }
            return JsonSerializer.Deserialize<BatchResponse>(responseContent, _jsonSerializerOptions);
        }
    }
}
