using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;


namespace Adventcalendar2024.MicrosoftAzureCosmosTable.Entities
{
    // テーブルエンティティのクラスを定義します
    public class CustomerEntity : TableEntity
    {
        public string? CustomerId { get; set; }
        public DateTimeOffset? RegisterDate { get; set; }
    }
}