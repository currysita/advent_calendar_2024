using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Entities
{
    // テーブルエンティティのクラスを定義します
    public class CustomerEntity : TableEntity
    {
        public string? CustomerId { get; set; }
        public DateTimeOffset RegisterDate { get; set; }
    }
}