using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SqlCipher4Unity3D;
using SQLite.Attribute;

namespace NF.Tool.DataFlow
{
    public static class SqliteExporter
    {
        public static void Export(string dllFpath, string inputExcelFpath, string outputFpath, string password)
        {
            List<Type> types = new List<Type>();
            foreach (Type type in Assembly.LoadFrom(dllFpath).GetTypes())
            {
                if (!type.IsClass)
                {
                    continue;
                }

                if (type.GetCustomAttributes(typeof(ExportAttribute), true).FirstOrDefault() == null)
                {
                    continue;
                }

                types.Add(type);
            }

            ExcelLoader loader = new ExcelLoader(inputExcelFpath);
            SQLiteConnection conn = new SQLiteConnection(outputFpath, password,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            for (int i = 0; i < types.Count; ++i)
            {
                Type type = types[i];
                List<object> dataList = loader.GetDataList(type, type.Name);
                Console.WriteLine($"{i + 1}/{types.Count} {type} :  {dataList.Count}");

                conn.DropTable(type);
                conn.CreateTable(type);
                conn.InsertAll(dataList, type);
            }
        }
    }
}