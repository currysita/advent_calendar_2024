using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adventcalendar2024.MicrosoftAzureCosmosTable.Entities;
using Microsoft.Azure.Cosmos.Table;

namespace Adventcalendar2024.MicrosoftAzureCosmosTable.Services
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

            // Microsoft.WindowsAzure.Storage の頃と違って、同期処理ができる！
            TableOperation operation = TableOperation.Insert(entity);
            TableResult tableResult = table.Execute(operation);

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
            TableResult tableResult = table.Execute(operation);

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
            TableResult tableResult = table.Execute(operation);
            return (FakeEntity)tableResult.Result;
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
            TableResult tableResult = table.Execute(operation);
            return (CustomerEntity)tableResult.Result;
        }

        /// <summary>]
        /// エンティティを削除する 
        /// </summary>
        public CustomerEntity Delete(CustomerEntity source)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);

            TableOperation operation = TableOperation.Delete(source);
            TableResult tableResult = table.Execute(operation);
            return (CustomerEntity)tableResult.Result;
        }

    }
}