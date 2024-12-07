
using Entities;
using Services;

public class Program
{
    public static void Main()
    {
        var customerService = new CustomerService();
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
        // カラムが異なるEntityで Merge する。既存のカラムが残る。
        FakeCustomerEntity faceCustomerEntity = customerService.Merge(customerEntity);
        // カラムが異なるEntityで Replace する。
        CustomerEntity customerEntity4 = customerService.Replace(customerEntity);
        // 削除する
        CustomerEntity customerEntity5 = customerService.Delete(customerEntity);
        // もう一回Entityを作成してから、別エンティティで削除する
    }
}
