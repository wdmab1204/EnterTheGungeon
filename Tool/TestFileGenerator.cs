using System;
using System.Collections.Generic;
using System.IO;

namespace ByteConverter
{
    public struct TestData
    {
        public int timeStamp;
        public int templateId;
        public long dbID;
        public long userDbId;
        public int amount;
    }

    //데이터 10*1024*1024일 때
    //csv : 약 332mb
    //bin : 약 286mb
    //약 60mb 감소
    internal static class TestFileGenerator
    {
        static string workspace => Directory.GetCurrentDirectory();

        public static void Create()
        {
            var items = new List<TestData>();
            var random = new Random();

            for (int i = 0; i < 10 * 1024* 1024; i++)
            {
                items.Add(new TestData
                {
                    timeStamp = i + 1,
                    dbID = random.Next(10000, 30000),
                    userDbId = random.Next(),
                    templateId = random.Next(1000, 4000),
                    amount = random.Next(1, 50)
                });
            }

            //items.Sort((left, right) => left.timeStamp - right.timeStamp);

            var path = Path.Combine(workspace, "test.csv");
            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine(string.Join(",",
                    Define.ColumnType.Int32,
                    Define.ColumnType.Int32,
                    Define.ColumnType.Int64,
                    Define.ColumnType.Int64,
                    Define.ColumnType.Int32
                    ));

                foreach(var item in items)
                {
                    writer.WriteLine(string.Join(",",
                        item.timeStamp, item.dbID, item.userDbId, item.templateId, item.amount));
                }
            }

            System.Diagnostics.Process.Start(workspace);
        }
    }
}
