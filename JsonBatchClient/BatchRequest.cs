using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JsonBatchClient.Base
{
    public class BatchRequest
    {
        public List<OperationRequest> Operations { get; } = new List<OperationRequest>();

        private HashSet<Guid> _createTempKeys = new HashSet<Guid>();

        public void AddCreateOperation<TData>(TData data) where TData : BaseEntity
        {
            TemportaryKey? temporaryId = null;

            temporaryId = new TemportaryKey { Key = data.Id };

            _createTempKeys.Add(data.Id);

            var foreignTemporaryKeys = GetTemportaryKeys(data);

            AddOperation(data, OperationType.Create);
        }
        public void AddUpdateOperation<TData>(TData data) where TData : BaseEntity
        {
            AddOperation(data, OperationType.Update);
        }
        public void AddDeleteOperation<TData>(TData data) where TData : BaseEntity
        {
           AddOperation(data, OperationType.Delete);
        }

        private void AddOperation<TData>(TData data, OperationType operationType) where TData : BaseEntity
        {
                var foreignTemporaryKeys = GetTemportaryKeys(data);
                        Operations.Add(
                                new OperationRequest
                                {
                                    TemporaryId = null,
                                    EntityType = typeof(TData).FullName,
                                    KeyType = typeof(Guid).FullName,
                                    OperationType = operationType,
                                    Data = data,
                                    ForeignTemporaryKeys = foreignTemporaryKeys
                    });
        }


        protected List<TemportaryKey>? GetTemportaryKeys(BaseEntity entity)
        {
            var foreignKeysFields = FindBaseEntities(entity);
            List<TemportaryKey>? foreignTemporaryKeys = null;
            if (foreignKeysFields.Any())
            {
                foreignTemporaryKeys = foreignKeysFields
                    .Where(e => _createTempKeys.Contains(e.Id))
                    .Select(e => new TemportaryKey { Key = e.Id })
                    .ToList();
            }
            return foreignTemporaryKeys;
        }

        protected static IEnumerable<BaseEntity> FindBaseEntities(object root)
        {
            var foundEntities = new HashSet<BaseEntity>();
            var visitedObjects = new HashSet<object>();

            SearchRecursive(root, foundEntities, visitedObjects);

            return foundEntities;
        }

        protected static void SearchRecursive(
            object obj,
            HashSet<BaseEntity> results,
            HashSet<object> visited)
        {
            // Null check or already processed check to prevent infinite recursion
            if (obj == null || visited.Contains(obj))
            {
                return;
            }

            visited.Add(obj);

            // If the current object is a BaseEntity, add it to results
            if (obj is BaseEntity entity)
            {
                results.Add(entity);
            }

            // Handle Collections (Lists, Arrays, etc.) separately to dive into elements
            if (obj is IEnumerable enumerable && !(obj is string))
            {
                foreach (var item in enumerable)
                {
                    SearchRecursive(item, results, visited);
                }
                return;
            }

            // Get all public and private instance fields
            var fields = obj.GetType().GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance
            );

            foreach (var field in fields)
            {
                // Optimization: Only recurse if the field is a class or interface 
                // (excluding strings and primitives)
                if (!field.FieldType.IsValueType && field.FieldType != typeof(string))
                {
                    var value = field.GetValue(obj);
                    SearchRecursive(value, results, visited);
                }
            }
        }
    }
}
