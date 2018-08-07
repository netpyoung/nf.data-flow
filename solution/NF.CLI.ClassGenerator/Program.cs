using System;
using CommandLine;

namespace NF.CLI.ClassGenerator
{
    internal class Options
    {
        [Option('t', "template_dir", Required = true)]
        public string TemplateDir { get; set; }

        [Option('e', "input_excel", Required = true)]
        public string InputExcel { get; set; }

        [Option('o', "output_dir", Required = true)]
        public string OutputDir { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed((opt) => Run(opt))
                .WithNotParsed((errs) =>
                {
                    foreach (Error err in errs)
                    {
                        Console.Error.WriteLine(err);
                    }
                });
        }

        private static void Run(Options opt)
        {
            ExcelClassGenerator.Generate(opt.InputExcel, opt.TemplateDir, opt.OutputDir);
        }
    }
}