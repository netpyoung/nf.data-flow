using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;
using SqlCipher4Unity3D;
using SQLite.Attribute;
using ToolDataClassGenerator;

namespace NF.CLI.DataExporter
{
    internal class Options
    {
        [Option('d', "dll", Required = true, HelpText = "intput dll")]
        public string DLL { get; set; }

        [Option('e', "excel", Required = true, HelpText = "input excel")]
        public string Excel { get; set; }

        [Option('o', "output", Required = true, HelpText = "output db")]
        public string Output { get; set; }

        [Option('p', "password", Required = false, HelpText = "db password")]
        public string Password { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            ParserResult<Options> result = Parser.Default.ParseArguments<Options>(args);
            result.WithParsed(Run).WithNotParsed(Fail);
        }

        private static void Fail(IEnumerable<Error> obj)
        {
        }

        private static void Run(Options opt)
        {
            SqliteExporter.Export(opt.DLL, opt.Excel, opt.Output, opt.Password);
        }
    }

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