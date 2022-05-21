using CommandLine;

namespace NF.Tools.DataFlow.DataExport
{
    [Verb("data-export", HelpText = "export data from excel.")]
    public class DataExporterOptions
    {
        [Option('i', "input_dir", Required = true, HelpText = "input excel")]
        public string InputExcelDir { get; set; }

        [Option('o', "output", Default = "output.db", HelpText = "output db")]
        public string OutputDatabasePath { get; set; }

        [Option('p', "password", Required = false, HelpText = "db password")]
        public string Password { get; set; }
    }
}