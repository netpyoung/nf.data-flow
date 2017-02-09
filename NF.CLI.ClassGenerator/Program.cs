namespace NF.CLI.ClassGenerator
{
    using System;
    using CommandLine;

    class Options
    {
        [Option('t', "template_dir", Required = true)]
        public string TemplateDir { get; set; }

        [Option('e', "input_excel", Required = true)]
        public string InputExcel { get; set; }

        [Option('o', "output_dir", Required = true)]
        public string OutputDir { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var opt = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, opt))
            {
                Run(opt);
            }
            else
            {
                Console.WriteLine("usage -e #{excel} -t #{template} -o #{output}");
            }
        }

        static void Run(Options opt)
        {
            ExcelClassGenerator.Generate(opt.InputExcel, opt.TemplateDir, opt.OutputDir);
        }
    }
}
