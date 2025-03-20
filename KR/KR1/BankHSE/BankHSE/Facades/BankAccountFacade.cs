using BankHSE.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankHSE.Facades
{
    public class BankAccountFacade
    {
        private readonly List<BankAccount> _accounts = new List<BankAccount>();

        public BankAccount CreateAccount(BankAccount account)
        {
            _accounts.Add(account);
            return account;
        }

        public void DeleteAccount(Guid accountId)
        {
            var acc = _accounts.FirstOrDefault(a => a.Id == accountId);
            if (acc != null)
            {
                _accounts.Remove(acc);
            }
        }

        public BankAccount GetAccountById(Guid accountId)
        {
            return _accounts.FirstOrDefault(a => a.Id == accountId);
        }

        public IEnumerable<BankAccount> GetAllAccounts()
        {
            return _accounts;
        }

        public void UpdateBalance(Guid accountId, decimal newBalance)
        {
            var acc = GetAccountById(accountId);
            if (acc != null)
            {
                acc.UpdateBalance(newBalance);
            }
        }
    }
}