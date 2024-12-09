
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
            string tableName = "SampleTable2";
            var customerService = new CustomerService(
                    tableName, appSettings.ConnectionStrings.AzureStorageTableConnectionString);
            // 主に使うデータを定義しとく
            Guid partitionKey = Guid.Parse("13bb5f6c-dfcf-4cad-a879-c9c02c6ce2aa");
            var rowKey = 0;
            var customerId = "xxx012345";
            var registerDate = DateTimeOffset.Parse("2024-01-01 00:00:00 +00:00");
            // エンティティを生成します
            CustomerEntity customerEntity = new CustomerEntity();
            customerEntity.PartitionKey = partitionKey.ToString();
            customerEntity.RowKey = rowKey.ToString();
            customerEntity.CustomerId = customerId;
            customerEntity.RegisterDate = registerDate;
            // Entityを作成する。この時点ではエンティティもテーブルも存在しない物とする
            customerEntity = customerService.Insert(customerEntity);
            // 取得する。
            CustomerEntity customerEntity2 = customerService.Retrieve(partitionKey, rowKey);
            // // カラムが異なるEntityで Replace する
            // FakeCustomerEntity FakeCustomerEntity = customerService.Replace(customerEntity);
            // // 元のエンティティに戻す。
            // customerEntity = customerService.Replace2(customerEntityReplace);
            // // 自作のエンティティクラスを使わずにPartitionKeyとRowKeyのみで取得する
            // var dynamicTableEntity = customerService.RetrieveDynamic(customerEntity);
            // // RegisterDateTime を null で更新する
            // dynamicTableEntity = customerService.ReplaceDynamicTableEntity(dynamicTableEntity);
            // // もう一度DynamicTableEntityで検索する。RegisterDateTimeはnullとなっている。
            // dynamicTableEntity = customerService.RetrieveDynamic(customerEntity);
            // // もう一度検索。存在しないプロパティを検索すると、nullで帰ってくる。
            // dynamicTableEntity = customerService.RetrieveDynamicFakeProperty(customerEntity);
        }
    }
}