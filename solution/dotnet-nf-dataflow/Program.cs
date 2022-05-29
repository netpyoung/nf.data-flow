using CommandLine;
using System;
using System.IO;
using YamlDotNet.Serialization;

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
                    if (File.Exists(opt.config))
                    {
                        string configYamlStr = File.ReadAllText(opt.config);
                        IDeserializer deserializer = new DeserializerBuilder().Build();
                        DataFlowRunnerOption yaml = deserializer.Deserialize<DataFlowRunnerOption>(configYamlStr);
                        if (yaml == null)
                        {
                            return DataFlowRunner.Run(opt);
                        }
                    
                        yaml.Merge(opt);
                        return DataFlowRunner.Run(yaml);
                    }

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