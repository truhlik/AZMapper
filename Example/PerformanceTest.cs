using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Example
{
    public class PerformanceTest
    {
        private const int NUMBER = 50000;
        private readonly List<TestEntity> _source;

        public PerformanceTest()
        {
            _source = new List<TestEntity>(NUMBER);
        }

        public void Init()
        {
            _source.Clear();

            var random = new Random();

            for (int i = 0; i < NUMBER; i++)
            {
                var entity = new TestEntity()
                {
                    Name = "PT: " + i,
                    DateOfBirth = DateTime.Now.AddMinutes(i),
                    Xml = new byte[] { byte.Parse(random.Next(126).ToString()) }
                };

                _source.Add(entity);
            }

            using (var dc = new MssqlDataContext())
            {
                dc.OpenConnection();
                dc.TruncateTable();
            }
        }

        public void InsertSingleThread()
        {
            Console.WriteLine("InsertSingleThread - Start");
            var sw = new Stopwatch();

            sw.Start();

            using (var dc = new MssqlDataContext())
            {
                dc.OpenConnection();
                dc.BeginTransaction();

                foreach (var e in _source)
                {
                    dc.InsertEntity(e);
                }

                dc.CommitTransaction();
            }

            sw.Stop();

            Console.WriteLine(string.Format("Time[ms]: {0}", sw.ElapsedMilliseconds));
            Console.WriteLine("InsertSingleThread - End");
        }

        public void InsertSingleThreadAuto()
        {
            Console.WriteLine("InsertSingleThreadAuto - Start");
            var sw = new Stopwatch();

            sw.Start();

            using (var dc = new MssqlDataContextAuto())
            {
                dc.OpenConnection();
                dc.BeginTransaction();

                foreach (var e in _source)
                {
                    dc.Insert(e);
                }

                dc.CommitTransaction();
            }

            sw.Stop();

            Console.WriteLine(string.Format("Time[ms]: {0}", sw.ElapsedMilliseconds));
            Console.WriteLine("InsertSingleThreadAuto - End");
        }

        public void InsertSingleThreadBulk()
        {
            Console.WriteLine("InsertSingleThreadBulk - Start");
            var sw = new Stopwatch();

            sw.Start();

            using (var dc = new MssqlDataContextAuto())
            {
                dc.OpenConnection();
                dc.BeginTransaction();

                dc.BulkInsert(_source);

                dc.CommitTransaction();
            }

            sw.Stop();

            Console.WriteLine(string.Format("Time[ms]: {0}", sw.ElapsedMilliseconds));
            Console.WriteLine("InsertSingleThreadBulk - End");
        }

        public void InsertMultiThread()
        {
            Console.WriteLine("InsertMultiThread - Start");
            var sw = new Stopwatch();

            sw.Start();

            using (var dc = new MssqlDataContext())
            {
                dc.OpenConnection();
                dc.BeginTransaction();

                var option = new ParallelOptions();
                option.MaxDegreeOfParallelism = 10;

                var res = Parallel.ForEach(_source, option, (entity) =>
                    {
                        dc.InsertEntity(entity);
                    });

                if (!res.IsCompleted)
                    throw new Exception("InsertMultiThreadSql not finished");

                dc.CommitTransaction();
            }

            sw.Stop();

            Console.WriteLine(string.Format("Time[ms]: {0}", sw.ElapsedMilliseconds));
            Console.WriteLine("InsertMultiThread - End");
        }

        public void InsertMultiThreadAuto()
        {
            Console.WriteLine("InsertMultiThreadAuto - Start");
            var sw = new Stopwatch();

            sw.Start();

            using (var dc = new MssqlDataContextAuto())
            {
                dc.OpenConnection();
                dc.BeginTransaction();

                var option = new ParallelOptions();
                option.MaxDegreeOfParallelism = 10;

                var res = Parallel.ForEach(_source, option, (entity) =>
                {
                    dc.Insert(entity);
                });

                if (!res.IsCompleted)
                    throw new Exception("InsertMultiThreadSql not finished");

                dc.CommitTransaction();
            }

            sw.Stop();

            Console.WriteLine(string.Format("Time[ms]: {0}", sw.ElapsedMilliseconds));
            Console.WriteLine("InsertMultiThreadAuto - End");
        }

        public void SelectAll()
        {
            Console.WriteLine("SelectAll - Start");
            var sw = new Stopwatch();
            List<TestEntity> list = null;

            sw.Start();

            using (var dc = new MssqlDataContext())
            {
                dc.OpenConnection();

                list = dc.SelectEntities();
            }

            sw.Stop();

            Console.WriteLine(string.Format("Time[ms]: {0}", sw.ElapsedMilliseconds));
            Console.WriteLine("SelectAll - End");
        }

        public void SelectAllUserMapping()
        {
            Console.WriteLine("SelectAllUserMapping - Start");
            var sw = new Stopwatch();
            List<ExampleEntity> list = null;

            sw.Start();

            using (var dc = new MssqlDataContext())
            {
                dc.OpenConnection();

                list = dc.SelectUserMappingEntities();
            }

            sw.Stop();

            Console.WriteLine(string.Format("Time[ms]: {0}", sw.ElapsedMilliseconds));
            Console.WriteLine("SelectAllUserMapping - End");
        }

        public void InsertSingleThreadSql()
        {
            Console.WriteLine("InsertSingleThreadSql - Start");
            var sw = new Stopwatch();

            sw.Start();

            using (var conn = new SqlConnection(ConnectionConfiguration.SqlConn.ConnectionString))
            {
                conn.Open();
                var tran = conn.BeginTransaction();

                foreach (var e in _source)
                {
                    InsertSQL(e, conn, tran);
                }

                tran.Commit();
            }

            sw.Stop();

            Console.WriteLine(string.Format("Time[ms]: {0}", sw.ElapsedMilliseconds));
            Console.WriteLine("InsertSingleThreadSql - End");
        }

        public void InsertMultiThreadSql()
        {
            Console.WriteLine("InsertMultiThreadSql - Start");
            var sw = new Stopwatch();

            sw.Start();

            using (var conn = new SqlConnection(ConnectionConfiguration.SqlConn.ConnectionString))
            {
                conn.Open();
                var tran = conn.BeginTransaction();

                var option = new ParallelOptions();
                option.MaxDegreeOfParallelism = 10;

                var res = Parallel.ForEach(_source, option, (entity) =>
                {
                    InsertSQL(entity, conn, tran);
                });

                if (!res.IsCompleted)
                    throw new Exception("InsertMultiThreadSql not finished");

                tran.Commit();
            }

            sw.Stop();

            Console.WriteLine(string.Format("Time[ms]: {0}", sw.ElapsedMilliseconds));
            Console.WriteLine("InsertMultiThreadSql - End");
        }

        #region Sql

        public void InsertSQL(TestEntity entity, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO mapper_ex_person (name,dateofbirth,Xml) VALUES (@name, @dob, @xml)";

            using (var command = new SqlCommand(query, conn, tran))
            {
                var p = command.Parameters.Add("name", System.Data.SqlDbType.NVarChar);
                p.Value = entity.Name;

                p = command.Parameters.Add("dob", System.Data.SqlDbType.DateTime);
                p.Value = entity.DateOfBirth;

                p = command.Parameters.Add("xml", System.Data.SqlDbType.Binary);
                p.Value = entity.Xml;

                command.ExecuteNonQuery();
            }
        }

        #endregion

    }
}
