namespace JsonBatchClient.Base
{
    public class BatchResponse
    {
        public List<OperationResult> OperationResults { get; } = new List<OperationResult>();
        public bool IsSuccess { get {return OperationResults.All(r => r.IsSuccess);} }
        public List<OperationResult> FailedResults { get { return OperationResults.Where(r => !r.IsSuccess).ToList(); } }
        public List<OperationResult> SuccessfulResults { get { return OperationResults.Where(r => r.IsSuccess).ToList(); } }
    }
}
