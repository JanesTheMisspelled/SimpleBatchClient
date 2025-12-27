namespace JsonBatchClient.Base
{
    public interface IBatchClient
    {
        Task<BatchResponse> ExecuteBatchAsync(BatchRequest request, string requestUri = "/batch");
    }
}
