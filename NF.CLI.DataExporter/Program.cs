namespace NF.CLI.DataExporter
{
    using System;
    using System.Reflection;
    using SQLite.Attribute;
    using SqlCipher.Core;
    using ToolDataClassGenerator;
    using CommandLine;
    using System.Linq;
    using System.Collections.Generic;

    class Options
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

    class Program
    {
        static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<Options>(args);
            result.WithParsed(Run);
        }

        private static void Run(Options opt)
        {
            SqliteExporter.Export(opt.DLL, opt.Excel, opt.Output, opt.Password);
        }
    }

    public static class SqliteExporter
    {
        public static void Export(string dll_fpath, string input_excel_fpath, string output_fpath, string password)
        {
            List<Type> types = new List<Type>();
            foreach (var type in Assembly.LoadFrom(dll_fpath).GetTypes())
            {
                if (!type.IsClass)
                    continue;

                if (type.GetCustomAttributes(typeof(ExportAttribute), true).FirstOrDefault() == null)
                    continue;

                types.Add(type);
            }

            var loader = new ExcelLoader(input_excel_fpath);
            var conn = new SQLiteConnection(output_fpath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            for (int i = 0; i < types.Count(); ++i)
            {
                var type = types[i];
                var data_list = loader.GetDataList(type, type.Name);
                Console.WriteLine(string.Format("{0}/{1} {2} :  {3}", i + 1, types.Count(), type, data_list.Count));

                conn.DropTable(type);
                conn.CreateTable(type);
                conn.InsertAll(data_list, type);
            }
        }
    }
}
