using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Entities
{
    /// <summary>
    /// Replaceの実行用のEntityです。
    /// CustomerEntityには無いプロパティがあります。
    /// </summary>
    public class FakeCustomerEntity : TableEntity
    {
        public FakeCustomerEntity() { }

        public string? CustomerId { get; set; }

        public DateTime FakeRegisterDate {get; set; }
    }
}