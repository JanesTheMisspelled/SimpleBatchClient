namespace JsonBatchClient.Base
{
    public interface ITransactionClient
    {
        Task<BatchResponse> ExecuteTransactionAsync(BatchRequest request, string requestUri = "/transaction");
    }
}
