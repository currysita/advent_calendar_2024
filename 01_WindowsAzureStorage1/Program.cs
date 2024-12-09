
using Entities;
using Services;
using Microsoft.Extensions.Configuration;
using Adventcalendar2024.WindowsAzureStorage.Configurations;

namespace Adventcalendar2024.WindowsAzureStorage
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
            var tableName = "SampleTable1";

            var customerService = new CustomerService(
                    tableName, appSettings.ConnectionStrings.AzureStorageTableConnectionString);

            // 適当なデータでエンティティを生成します
            // 同じエンティティを作る事はできないので、事前に削除しといてください
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
            // エンティティの取得。デバッグで値が同じである事を見てみましょう
            // 挿入した結果のEntityはETagが異なります。Insertで使ったエンティティは使用できなくなります。
            CustomerEntity retrieveEntity = customerService.Retrieve(partitionKey, rowKey);

            // カラムが異なるEntityでMergeします。異なるカラムができるのを確認しましょう
            var fakeCustomerEntity = new FakeEntity();
            fakeCustomerEntity.PartitionKey = retrieveEntity.PartitionKey;
            fakeCustomerEntity.RowKey = retrieveEntity.RowKey;
            fakeCustomerEntity.ETag = retrieveEntity.ETag;
            fakeCustomerEntity.CustomerId = retrieveEntity.CustomerId;
            fakeCustomerEntity.FakeRegisterDate = retrieveEntity.RegisterDate;
            FakeEntity mergedEntity = customerService.Merge(fakeCustomerEntity);

            // Insertした時と同じEntityでReplaceします。
            // ETagだけ更新してます。
            customerEntity.ETag = mergedEntity.ETag;
            CustomerEntity replacedEntity = customerService.Replace(customerEntity);
            // // 削除する
            // CustomerEntity customerEntity5 = customerService.Delete(customerEntity);
            // もう一回Entityを作成してから、別エンティティで削除する
        }
    }
}
