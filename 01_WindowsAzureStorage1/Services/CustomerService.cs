using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Services
{
    public class CustomerService
    {
        private string tableName = "SampleTable01";

        private string ConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        /// <summary>
        /// Insert エンティティを追加する
        /// </summary>
        public CustomerEntity Insert(CustomerEntity entity)
        {
            // テーブルクライアント作成し、テーブル参照を取得します。
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            
            // 非同期処理で
            Task<bool> createTask = table.CreateIfNotExistsAsync();
            createTask.Wait();
            // テーブル作成の成功・失敗を判断したい場合は、このようにフラグで結果を取得してください。
            // まあ失敗したらほぼ例外が発生しますので、あんまり使う事無いと思います。
            bool isCreateSuccess = createTask.Result;

            TableOperation operation = TableOperation.Insert(entity);

            // 追加した結果を取得する場合にはこのように。
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult tableResult = task.Result;

            return (CustomerEntity)tableResult.Result;
        }

        /// <summary>
        /// Insert エンティティを追加する
        /// </summary>
        public CustomerEntity Insert(Guid partitionKey, int rowKey)
        {
            // テーブルクライアント作成し、テーブル参照を取得します。
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            // テーブル作成の成功・失敗を判断したい場合は、このようにフラグで結果を取得してください。
            // まあ失敗したらほぼ例外が発生しますので、あんまり使う事無いと思います。
            Task<bool> createTask = table.CreateIfNotExistsAsync();
            createTask.Wait();
            bool isCreateSuccess = createTask.Result;
            // 新しいエンティティを作成し、挿入します。
            CustomerEntity entity = 
                new CustomerEntity()
            {
                PartitionKey = partitionKey.ToString(),
                RowKey = rowKey.ToString(),
                CustomerId = "xxx012345",
                RegisterDate = DateTime.UtcNow
            };
            TableOperation operation = TableOperation.Insert(entity);

            // 追加した結果を取得する場合にはこのように。
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult tableResult = task.Result;

            return (CustomerEntity)tableResult.Result;
        }

        /// <summary>
        /// 取得
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public CustomerEntity Retrieve(Guid partitionKey, int rowKey)
        {
            // テーブルクライアント作成
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            // テーブル参照を取得します
            var table = tableClient.GetTableReference(tableName);
            TableOperation operation = TableOperation.Retrieve<CustomerEntity>(partitionKey.ToString(), rowKey.ToString());

            // 追加した結果を取得する場合にはこのように。
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult tableResult = task.Result;

            return (CustomerEntity)tableResult.Result;
        }

        /// <summary>
        /// 別のEntityを使用してマージする。既存のカラムが残る
        /// </summary>
        public FakeCustomerEntity Merge(CustomerEntity source)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            // 違うEntityに入れ替える
            var faceCustomerEntity = new FakeCustomerEntity();
            faceCustomerEntity.PartitionKey = source.PartitionKey;
            faceCustomerEntity.ETag = source.ETag;
            faceCustomerEntity.CustomerId = source.CustomerId;
            faceCustomerEntity.FakeRegisterDate = source.RegisterDate;

            TableOperation operation = TableOperation.Merge(faceCustomerEntity);
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult result = task.Result;
            return (FakeCustomerEntity)result.Result;
        }

        /// <summary>
        /// Entityを更新する。取得済みのEntityを使用しているが、
        /// PartitionKeyとRowKeyとETagがあれば同じエンティティだと識別してくれる。
        /// エンティティクラスに無いプロパティはnullとなる。
        /// </summary>
        public CustomerEntity Replace(CustomerEntity source)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            // 日付を更新
            source.RegisterDate = DateTime.UtcNow;

            TableOperation operation = TableOperation.Replace(source);
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult result = task.Result;
            return (CustomerEntity)result.Result;
        }

        /// <summary>
        /// Entityを更新する。違うEntityを使用しているので、既存のカラムの一部が消えてしまう
        /// </summary>
        public FakeCustomerEntity Replace2(CustomerEntity source)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            // 違うEntityに入れ替える
            var customerEntityDummy = new FakeCustomerEntity();
            customerEntityDummy.PartitionKey = source.PartitionKey;
            customerEntityDummy.ETag = source.ETag;
            customerEntityDummy.CustomerId = source.CustomerId;
            customerEntityDummy.FakeRegisterDate = source.RegisterDate;

            TableOperation operation = TableOperation.Replace(source);
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult result = task.Result;
            return (FakeCustomerEntity)result.Result;
        }

        /// <summary>]
        /// エンティティを削除する 
        /// </summary>
        public CustomerEntity Delete(CustomerEntity source)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            TableOperation operation = TableOperation.Delete(source); // Etagが無ければ例外が発生する
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult result = task.Result;
            return (CustomerEntity)result.Result;
        }

        /// <summary>]
        /// エンティティを削除する。違うEntityで削除してみる。
        /// </summary>
        public CustomerEntity Delete2(FakeCustomerEntity source)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            TableOperation operation = TableOperation.Delete(source); // Etagが無ければ例外が発生する
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult result = task.Result;
            return (CustomerEntity)result.Result;
        }
    }
}