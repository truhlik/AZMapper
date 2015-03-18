using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test - start");

            //using (var dc = new MssqlDataContext())
            //{
            //    dc.OpenConnection();

            //    //var result = dc.InsertEntity();
            //    //var result = dc.SelectEntities();
            //    //var result = dc.SelectEntitiesSingleField();
            //    //var result = dc.GetAllSP();
            //    //var result = dc.GetAllSP2();
            //    //var result = dc.UpdateEntity();
            //    //var result = dc.SelectAsDataTableEntities();
            //    var result = dc.SelectUserMappingEntities();
            //    Console.WriteLine("Result: " + result);
            //}

            //using (var dc = new MssqlDataContextAuto())
            //{
            //    dc.OpenConnection();

            //    //var res = dc.InsertEntity();
            //    //var res = dc.BulkInsertEntity();
            //    //var res = dc.UpdateEntity();
            //    //var res = dc.Update2Entity();
            //    var res = dc.UpdateWithMonitorEntity();
            //    Console.WriteLine("Result: " + res);
            //}

            var test = new PerformanceTest();
            test.Init();
            //test.InsertSingleThread();
            //test.InsertSingleThreadAuto();
            //test.InsertSingleThreadBulk();
            //test.InsertMultiThread();
            //test.InsertMultiThreadAuto();
            //test.SelectAll();
            //test.SelectAllUserMapping();
            //test.InsertSingleThreadSql();
            //test.InsertMultiThreadSql();

            for (int i = 0; i < 5; i++)
            {
                //test.InsertSingleThread();
                //test.InsertSingleThreadBulk();

                //test.InsertSingleThreadBulk();
                //test.InsertSingleThreadSql();

                test.InsertSingleThread();
                test.InsertSingleThreadAuto();
            }

            Console.WriteLine("Test - end");
        }
    }
}
