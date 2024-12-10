
using Adventcalendar2024.MicrosoftAzureCosmosTable.Configurations;
using Adventcalendar2024.MicrosoftAzureCosmosTable.Entities;
using Adventcalendar2024.MicrosoftAzureCosmosTable.Services;
using Microsoft.Extensions.Configuration;

namespace Adventcalendar2024.MicrosoftAzureCosmosTable
{
    public class Program
    {
        public static void Main()
        {
            // 接続文字列の設定情報とテーブル名
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            AppSettings appSettings = configuration.Get<AppSettings>() ?? new AppSettings();
            var tableName = "SampleTable2";

            var customerService = new CustomerService(
                    tableName, appSettings.ConnectionStrings.AzureStorageTableConnectionString);
                    
            // 適当なデータでエンティティを生成します
            Guid partitionKey = Guid.Parse("13bb5f6c-dfcf-4cad-a879-c9c02c6ce2aa");
            var rowKey = 0;
            var customerId = "xxx012345";
            var registerDate = DateTimeOffset.Parse("2024-01-01 00:00:00 +00:00");
            // エンティティの挿入
            CustomerEntity customerEntity = new CustomerEntity();
            customerEntity.PartitionKey = partitionKey.ToString();
            customerEntity.RowKey = rowKey.ToString();
            customerEntity.CustomerId = customerId;
            customerEntity.RegisterDate = registerDate;
            customerEntity = customerService.Insert(customerEntity);

            // エンティティの取得
            CustomerEntity retrieveEntity = customerService.Retrieve(partitionKey, rowKey);

            // エンティティのマージ
            var fakeEntity = new FakeEntity();
            fakeEntity.PartitionKey = retrieveEntity.PartitionKey;
            fakeEntity.RowKey = retrieveEntity.RowKey;
            fakeEntity.ETag = retrieveEntity.ETag;
            fakeEntity.CustomerId = retrieveEntity.CustomerId;
            fakeEntity.FakeRegisterDate = retrieveEntity.RegisterDate;
            FakeEntity mergedEntity = customerService.Merge(fakeEntity);

            // Insertした時と同じEntityでReplaceします。
            // ETagだけ更新してます。ETagの例="W/"datetime'2024-12-10T00%3A25%3A12.7987294Z'"
            customerEntity.ETag = mergedEntity.ETag;
            CustomerEntity replacedEntity = customerService.Replace(customerEntity);
            
            // 削除します。
            CustomerEntity deletedEntity = customerService.Delete(customerEntity);
            // もう一度挿入した上で、いくつかの情報を削除してから削除を実行してみます。
            // partitionKey、rowkey、ETagさえあれば削除が可能であることが解ります。
            customerEntity = customerService.Insert(customerEntity);
            customerEntity.CustomerId = null;
            customerEntity.RegisterDate = null;
            CustomerEntity deletedEntity2 = customerService.Delete(customerEntity);
        }
    }
}