using CommandLine;
using System;

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

            ParserResult<DataFlowRunnerOption> parseResult = Parser.Default.ParseArguments<DataFlowRunnerOption>(args);
            int shellExitStatus = parseResult.MapResult(opt =>
            {
                try
                {
                    return DataFlowRunner.Run(opt);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    return 1;
                }
            },
            err =>
            {
                Console.WriteLine(err);
                return 1;
            });
            return shellExitStatus;
        }
    }
}