namespace JsonBatchClient.Base
{
    public class OperationRequest
    {
        public TemportaryKey? TemporaryId { get; set; } 
        public string EntityType { get; set; }
        public string KeyType { get; set; }
        public OperationType OperationType { get; set; } 
        public BaseEntity Data { get; set; }
        public List<TemportaryKey>? ForeignTemporaryKeys { get; set; }

    }
}
