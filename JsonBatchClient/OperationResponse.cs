namespace JsonBatchClient.Base
{
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public TemportaryKey? TemporaryId { get; set; }
        public string EntityType { get; set; }
        public string KeyType { get; set; }
        public OperationType OperationType { get; set; }
        public BaseEntity Data { get; set; }
        public List<TemportaryKeyKeyPair>? ForeignTemporaryKeys { get; set; }

    }
}
