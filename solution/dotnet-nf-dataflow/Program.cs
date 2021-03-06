﻿using System;
using CommandLine;
using NF.Tools.DataFlow.CodeGen;
using NF.Tools.DataFlow.DataExport;

namespace NF.Tools.DataFlow
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<CodeGenOptions, DataExportOptions>(args)
                .MapResult(
                    (CodeGenOptions opts) => RunCodeGenOptions(opts),
                    (DataExportOptions opts) => RunDataExportOptions(opts),
                    errs => 1);
        }

        private static int RunCodeGenOptions(CodeGenOptions opt)
        {
            try
            {
                ExcelClassGenerator.Generate(opt.InputExcel, opt.TemplateDir, opt.OutputDir);
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }
        }

        private static int RunDataExportOptions(DataExportOptions opt)
        {
            try
            {
                SqliteExporter.Export(opt.DLL, opt.Excel, opt.Output, opt.Password);
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }
        }
    }
}