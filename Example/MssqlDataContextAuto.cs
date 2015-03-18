using System;
using System.Collections.Generic;
using AZMapper.AQ;

namespace Example
{
    public class MssqlDataContextAuto : DataContextBase
    {
        static MssqlDataContextAuto()
        {
            TableMapperConfiguration.BindingValueMark = "@";
            //1. 'automapping' for insert/update/bulk insert queries
            var conf = new TableMapperConfiguration();

            var typeResolver = new DbTypeResolver();
            conf.AddMapping(typeof(TestEntity),
                new Table("mapper_ex_person", typeResolver).AddFieldsAuto(typeof(TestEntity)).SetPrimaryKey("id", true));

            /*
            //2. manual mapping for insert/update/bulk insert queries
            conf.AddMapping(typeof(TestEntity), new Table("mapper_ex_person")
                .AddField("id", System.Data.DbType.Int32, true, true)
                .AddField("name", System.Data.DbType.String)
                .AddField("dateofbirth", System.Data.DbType.DateTime)
                .AddField("Xml", System.Data.DbType.Binary));
             */
        }

        public int InsertEntity()
        {
            var entity = new TestEntity()
            {
                DateOfBirth = DateTime.Now.Subtract(new TimeSpan(new Random().Next(100000))),
                Name = "Name " + new Random().Next(1000),
                Xml = null
            };

            using (var q = this.CreateQuery())
            {
                return q.Insert(entity);
                //the same as return q.RowsAffected;
            }
        }

        public int BulkInsertEntity()
        {
            var list = new List<TestEntity>()
            {
                new TestEntity()
                {
                    DateOfBirth = DateTime.Now.Subtract(new TimeSpan(new Random().Next(100300))),
                    Name = "Name BK1",
                    Xml = null
                },
                new TestEntity()
                {
                    DateOfBirth = DateTime.Now.Subtract(new TimeSpan(new Random().Next(10000))),
                    Name = "Name BK2",
                    Xml = null
                },
                new TestEntity()
                {
                    DateOfBirth = DateTime.Now.Subtract(new TimeSpan(new Random().Next(100))),
                    Name = "Name BK3",
                    Xml = new byte[]{1,2,3,4}
                }
            };

            using (var q = this.CreateQuery())
            {
                return q.BulkInsert(list);
            }
        }

        public int UpdateEntity()
        {
            var entity = new TestEntity()
            {
                Id = 140,
                DateOfBirth = DateTime.Now.Subtract(new TimeSpan(new Random().Next(100))),
                Name = "Name BK3333",
                Xml = new byte[] { 1, 2 }
            };

            using (var q = this.CreateQuery())
            {
                //updates all fields
                return q.Update(entity);
            }
        }

        public int Update2Entity()
        {
            var entity = new TestEntity()
            {
                Id = 15,
                DateOfBirth = DateTime.Now.Subtract(new TimeSpan(new Random().Next(100))),
                Name = "Name BK2",
                Xml = new byte[] { 1, 2,3,4,5,6 }
            };

            var fields = new string[]{"name"};

            using (var q = this.CreateQuery())
            {
                //updates all listed fields
                return q.Update<TestEntity>(entity, fields);
            }
        }

        public int UpdateWithMonitorEntity()
        {
            var entity = new TestEntity()
            {
                Id = 16,
                DateOfBirth = DateTime.Now,
                Name = "Name BK3",
                Xml = new byte[] { 1, 2 }
            };

            EntityMonitor.Instance.Add(entity);

            using (var q = this.CreateQuery())
            {
                entity.Xml = null;
                entity.DateOfBirth = DateTime.Now.Subtract(new TimeSpan(100, 10, 10, 10, 10));

                //updates only changed fields
                var res = q.UpdateWithMonitor(entity);

                EntityMonitor.Instance.Remove(entity);

                return res;
            }
        }
    }
}
