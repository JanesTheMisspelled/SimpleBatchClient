
using JsonBatchClient.Base;
using Newtonsoft.Json;

namespace JsonBatchClient.Newtonsoft
{
    public class DefaultClient : JsonBatchClient.Base.DefaultClient
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public DefaultClient(HttpClient httpClient, JsonSerializerSettings jsonSerializerSettings) : base(httpClient)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }
        protected override string SerializeBatchRequest(BatchRequest request)
        {
            return JsonConvert.SerializeObject(request, _jsonSerializerSettings);
        }
        protected override BatchResponse DeserializeBatchResponse(string responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new ArgumentNullException(nameof(responseContent));
            }
            return JsonConvert.DeserializeObject<BatchResponse>(responseContent, _jsonSerializerSettings);
        }
    }
    
}
