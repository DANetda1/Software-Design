using BankHSE.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankHSE.Facades
{
    public class OperationFacade
    {
        private readonly List<Operation> _operations = new List<Operation>();

        public Operation CreateOperation(Operation operation)
        {
            _operations.Add(operation);
            return operation;
        }

        public void DeleteOperation(Guid operationId)
        {
            var op = _operations.FirstOrDefault(o => o.Id == operationId);
            if (op != null)
            {
                _operations.Remove(op);
            }
        }

        public Operation GetOperationById(Guid operationId)
        {
            return _operations.FirstOrDefault(o => o.Id == operationId);
        }

        public IEnumerable<Operation> GetAllOperations()
        {
            return _operations;
        }
    }
}