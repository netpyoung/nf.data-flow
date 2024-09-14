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
            ParserResult<DataFlowRunnerOption> parseResult = Parser.Default.ParseArguments<DataFlowRunnerOption>(args);
            int shellExitStatus = parseResult.MapResult(opt =>
            {
                try
                {
                    if (!File.Exists(opt.config))
                    {
                        return DataFlowRunner.Run(opt);
                    }

                    string configYamlStr = File.ReadAllText(opt.config);
                    IDeserializer deserializer = new DeserializerBuilder().Build();
                    DataFlowRunnerOption yamlBasedOpt = deserializer.Deserialize<DataFlowRunnerOption>(configYamlStr);
                    if (yamlBasedOpt == null)
                    {
                        return DataFlowRunner.Run(opt);
                    }

                    DataFlowRunnerOption mergedOpt = yamlBasedOpt.Merge(opt);
                    return DataFlowRunner.Run(mergedOpt);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    return 1;
                }
            },
            err =>
            {
                Console.Error.WriteLine($"err: {err}");
                return 1;
            });
            return shellExitStatus;
        }
    }
}