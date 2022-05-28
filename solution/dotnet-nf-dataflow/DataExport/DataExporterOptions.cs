using CommandLine;
using System.Collections.Generic;

namespace NF.Tools.DataFlow.DataExport
{
    [Verb("data-export", HelpText = "export data from excel.")]
    public class DataExporterOptions
    {
        [Option('i', "input_paths", Required = true, HelpText = "input excel paths")]
        public IEnumerable<string> InputExcelPaths { get; set; }

        [Option('o', "output_path", Default = "output.db", HelpText = "output db path (default: output.db)")]
        public string OutputPath { get; set; }

        [Option('p', "password", Required = false, HelpText = "db password")]
        public string Password { get; set; }
    }
}