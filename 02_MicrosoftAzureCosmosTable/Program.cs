
using Entities;
using Services;

public class Program
{
    public static void Main()
    {
        string tableName = "SampleTable02";
        string connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        var customerService = new CustomerService(tableName, connectionString);
        // 主に使うデータを定義しとく
        Guid partitionKey = Guid.Parse("13bb5f6c-dfcf-4cad-a879-c9c02c6ce2aa");
        var rowKey = 0;
        var CustomerId = "xxx012345";
        var RegisterDate = DateTime.Parse("2024-01-01 00:00:00");
        var dt2 = DateTime.SpecifyKind(RegisterDate, DateTimeKind.Local);
        // エンティティを生成します
        CustomerEntity customerEntity = new CustomerEntity();
        customerEntity.PartitionKey = partitionKey.ToString();
        customerEntity.RowKey = rowKey.ToString();
        customerEntity.CustomerId = CustomerId;
        customerEntity.RegisterDate = RegisterDate;
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
