using System;
using System.Collections.Generic;
using System.Data;
using AZMapper;
using AZMapper.Extensions;

namespace Example
{
    public class MssqlDataContext : DataContextBase
    {
        static MssqlDataContext()
        {
            //Extensions
            var mapper = new ResultEntityMapper<TestEntity>();
            mapper.AddExtension<byte[]>(e => e.Xml, (reader, index) =>
                {
                    return reader.GetObject(index) as byte[];
                });
        }

        public int InsertEntity()
        {
            string query = "INSERT INTO mapper_ex_person (name,dateofbirth,Xml) VALUES (@name, @dob, @xml)";

            using (var q = this.CreateQuery(query))
            {
                q.AddInputParameter("dob", DateTime.Now, (int)DbType.DateTime)
                    .AddInputParameter("name", "Name " + new Random().Next(1000), (int)DbType.String)
                    .AddInputParameter("xml", new byte[] { 10, 20, 30 }, (int)DbType.Binary)
                    .ExecuteNonQuery();

                return q.RowsAffected;
            }
        }

        public int InsertEntity(TestEntity entity)
        {
            string query = "INSERT INTO mapper_ex_person (name,dateofbirth,Xml) VALUES (@name, @dob, @xml)";

            using (var q = this.CreateQuery(query))
            {
                q.AddInputParameter("dob", entity.DateOfBirth, (int)DbType.DateTime)
                    .AddInputParameter("name", entity.Name, (int)DbType.String)
                    .AddInputParameter("xml", entity.Xml, (int)DbType.Binary)
                    .ExecuteNonQuery();

                return q.RowsAffected;
            }
        }

        public List<TestEntity> SelectEntities()
        {
            string query = "SELECT * FROM mapper_ex_person";

            using (var q = this.CreateQuery(query))
            {
                q.UseAutoMapper = true;
                q.ExecuteReader();

                return q.ResultAsList<TestEntity>();
            }
        }

        public List<ExampleEntity> SelectUserMappingEntities()
        {
            string query = "SELECT * FROM mapper_ex_person";

            using (var q = this.CreateQuery(query))
            {
                q.UseAutoMapper = false;
                q.ExecuteReader();

                return q.ResultAsList<ExampleEntity>();
                //return q.ResultAsList<ExampleEntity>(action: (e) => e.Xml = new byte[] { (byte)(e.Id % 2) });
            }
        }

        public DataTable SelectAsDataTableEntities()
        {
            string query = "SELECT * FROM mapper_ex_person";

            using (var q = this.CreateQuery(query))
            {
                q.UseAutoMapper = true;
                q.ExecuteReader();

                return q.ResultAsDataTable();
            }
        }

        public List<string> SelectEntitiesSingleField()
        {
            string query = "SELECT * FROM mapper_ex_person";

            using (var q = this.CreateQuery(query))
            {
                q.UseAutoMapper = true;
                q.ExecuteReader();

                return q.ResultAsList<string>("name", (reader, index) => { return reader.GetString(index); });
            }
        }

        public int UpdateEntity()
        {
            using (var q = this.CreateQuery(@"UPDATE mapper_ex_person SET Xml = @xml WHERE id = @id"))
            {
                q.UseAutoMapper = true;
                q.AddInputParameter("xml", new byte[] { 11, 22, 33, 44, 55, 66, 77, 88 }, (int)DbType.Binary)
                 .AddInputParameter("id", 15, (int)DbType.Int32)
                 .ExecuteReader();

                return q.RowsAffected;
            }
        }

        public List<TestEntity> GetAllSP()
        {
            using (var q = this.CreateQuery("dbo", "GetEntities"))
            {
                q.AddInputParameter("@birthData", DateTime.Now, (int)DbType.DateTime);
                q.UseAutoMapper = true;
                q.ExecuteReader();
                //first query
                var t = q.ResultAsList<TestEntity>();
                q.NextResult();
                //secdond query
                var t2 = q.ResultAsList<TestEntity>();
                q.NextResult();

                return t2;
            }
        }

        public int GetAllSP2()
        {
            using (var q = this.CreateQuery("dbo", "GetEntitiesWOP"))
            {
                q.AddOutputParameter("maxid", (int)DbType.Int32);
                q.UseAutoMapper = true;
                q.ExecuteReader();

                return Convert.ToInt32(q.GetOutputValue("maxid"));
            }
        }

        public void TruncateTable()
        {
            using (var q = this.CreateQuery("TRUNCATE TABLE mapper_ex_person"))
            {
                q.ExecuteNonQuery();
            }
        }
    }
}
