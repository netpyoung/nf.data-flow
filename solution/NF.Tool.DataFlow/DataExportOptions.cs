using CommandLine;

namespace NF.Tool.DataFlow
{
    [Verb("data-export", HelpText = "export data from excel.")]
    internal class DataExportOptions
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
}