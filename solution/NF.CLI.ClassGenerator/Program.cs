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
            Options opt = new Options();
            if (Parser.Default.ParseArguments(args, opt))
            {
                Run(opt);
            }
            else
            {
                Console.WriteLine("usage -e #{excel} -t #{template} -o #{output}");
            }
        }

        private static void Run(Options opt)
        {
            ExcelClassGenerator.Generate(opt.InputExcel, opt.TemplateDir, opt.OutputDir);
        }
    }
}