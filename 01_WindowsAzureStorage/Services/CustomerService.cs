using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adventcalendar2024.WindowsAzureStorage.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Adventcalendar2024.WindowsAzureStorage.Services
{
    public class CustomerService
    {
        private string TableName {get; set;}

        private string ConnectionString {get; set;}

        public CustomerService(string tableName, string connectionString)
        {
            TableName = tableName;
            ConnectionString = connectionString;
        }
        /// <summary>
        /// Insert エンティティを追加する
        /// </summary>
        public CustomerEntity Insert(CustomerEntity entity)
        {
            // テーブルクライアント作成し、テーブル参照を取得します。
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);
            
            // 非同期処理で
            Task<bool> createTask = table.CreateIfNotExistsAsync();
            createTask.Wait();
            // テーブル作成の成功・失敗を判断したい場合は、このようにフラグで結果を取得してください。
            // まあ失敗したらほぼ例外が発生しますので、あんまり使う事無いと思います。
            bool isCreateSuccess = createTask.Result;

            TableOperation operation = TableOperation.Insert(entity);

            // 追加した結果を取得する場合にはこのように。
            // table.ExecuteAsync(operation);
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
            var table = tableClient.GetTableReference(TableName);
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
        public FakeEntity Merge(FakeEntity fakeCustomerEntity)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);

            TableOperation operation = TableOperation.Merge(fakeCustomerEntity);
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult result = task.Result;
            return (FakeEntity)result.Result;
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
            var table = tableClient.GetTableReference(TableName);

            TableOperation operation = TableOperation.Replace(source);
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult result = task.Result;
            return (CustomerEntity)result.Result;
        }

        /// <summary>
        /// Entityを更新する。違うEntityを使用しているので、既存のカラムの一部が消えてしまう
        /// </summary>
        public FakeEntity Replace2(CustomerEntity source)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);

            // 違うEntityに入れ替える
            var customerEntityDummy = new FakeEntity();
            customerEntityDummy.PartitionKey = source.PartitionKey;
            customerEntityDummy.ETag = source.ETag;
            customerEntityDummy.CustomerId = source.CustomerId;
            customerEntityDummy.FakeRegisterDate = source.RegisterDate;

            TableOperation operation = TableOperation.Replace(source);
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult result = task.Result;
            return (FakeEntity)result.Result;
        }

        /// <summary>]
        /// エンティティを削除する 
        /// </summary>
        public CustomerEntity Delete(CustomerEntity source)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);

            TableOperation operation = TableOperation.Delete(source); // Etagが無ければ例外が発生する
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult result = task.Result;
            return (CustomerEntity)result.Result;
        }

        /// <summary>]
        /// エンティティを削除する。違うEntityで削除してみる。
        /// </summary>
        public CustomerEntity Delete2(FakeEntity source)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);

            TableOperation operation = TableOperation.Delete(source); // Etagが無ければ例外が発生する
            Task<TableResult> task = table.ExecuteAsync(operation);
            task.Wait();
            TableResult result = task.Result;
            return (CustomerEntity)result.Result;
        }
    }
}