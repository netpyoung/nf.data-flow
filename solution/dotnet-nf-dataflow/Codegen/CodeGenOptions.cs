using CommandLine;

namespace NF.Tools.DataFlow.CodeGen
{
    [Verb("code-gen", HelpText = "generate code from exel.")]
    internal class CodeGenOptions
    {
        [Option('t', "template_dir", Required = true)]
        public string TemplateDir { get; set; }

        [Option('e', "input_excel", Required = true)]
        public string InputExcel { get; set; }

        [Option('o', "output_dir", Required = true)]
        public string OutputDir { get; set; }
    }
}