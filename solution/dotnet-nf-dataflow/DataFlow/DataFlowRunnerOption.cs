﻿using CommandLine;
using System.Collections.Generic;

namespace NF.Tools.DataFlow
{
    [Verb("code-gen", HelpText = "generate code from exel.")]
    public class DataFlowRunnerOption
    {
        [Option('i', "input_paths", Default = "input")]
        public IEnumerable<string> input_paths { get; set; }

        [Option('o', "output_code_dir", Default = "output", HelpText = "output directory")]
        public string output_code_dir { get; set; }

        [Option('o', "output_db_path", Default = "output.db", HelpText = "output db path")]
        public string output_db_path { get; set; }

        [Option('p', "password", Required = false, HelpText = "db password")]
        public string password { get; set; }

        [Option('n', "namespace", Default = "AutoGenerated.DB", HelpText = "namespace")]
        public string @namespace { get; set; }

        // TODO(pyoung): template는 하나로 줄이고
        [Option('t', "template_dir")]
        public string template_dir { get; set; }

        [Option("template_const")]
        public string template_const { get; set; }

        [Option("template_enum")]
        public string template_enum { get; set; }

        [Option("template_class")]
        public string template_class { get; set; }

        [Option('c', "config", Default = ".dataflow.yaml", HelpText = "config file path")]
        public string config { get; set; }

        public void Merge(in DataFlowRunnerOption o)
        {
            if (this.input_paths == null)
            {
                this.input_paths = o.input_paths;
            }
            if (this.output_code_dir == null)
            {
                this.output_code_dir = o.output_code_dir;
            }
            if (this.output_db_path == null)
            {
                this.output_db_path = o.output_db_path;
            }
            if (this.password == null)
            {
                this.password = o.password;
            }
            if (this.@namespace == null)
            {
                this.@namespace = o.@namespace;
            }
            if (this.template_dir == null)
            {
                this.template_dir = o.template_dir;
            }
            if (this.template_const == null)
            {
                this.template_const = o.template_const;
            }
            if (this.template_enum == null)
            {
                this.template_enum = o.template_enum;
            }
            if (this.template_class == null)
            {
                this.template_class = o.template_class;
            }

            // NOTE(pyoung): pass - ConfigFpath 
        }

        // TODO(pyoung): 아싸리 커스텀 sln폴더를 포함시켜 빌드시키게 하는 걸 옵션으로 주면 어떨까?
    }
}