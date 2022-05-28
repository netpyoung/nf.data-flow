using System;
using System.IO;
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NF.Tools.DataFlow.CodeGen;
using NF.Tools.DataFlow.DataExport;

namespace NF.Tools.DataFlow
{
    public class Program
    {
        private static int Main(string[] args)
        {
            //DataExporterOptions opt = new DataExporterOptions
            //{
            //    InputExcelDir = "C:/prj/nf.data-flow/exels",
            //    OutputDatabasePath = "output.db",
            //};
            //return new DataExporter(opt).Export();
          

            try
            {
                return Parser.Default.ParseArguments<CodeGeneratorOptions, DataExporterOptions>(args)
                    .MapResult(
                        (CodeGeneratorOptions opt) =>
                        {
                            return CodeGenerator.Generate(opt);
                        },
                        (DataExporterOptions opt) =>
                        {
                            return DataExporter.Export(opt);
                        },
                        errs => 1);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }
        }
    }
}