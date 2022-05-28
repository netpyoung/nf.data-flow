using DotLiquid;
using NF.Tools.DataFlow.CodeGen.Internal;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("NF.Tools.DataFlow.Test")]
namespace NF.Tools.DataFlow.CodeGen
{
    public class CodeGenerator
    {
        public readonly struct CodeGenInfo
        {
            public List<WorkbookResultInfo> WorkbookResultInfos { get; init; }
        }

        public record struct WorkbookResultInfo
        {
            public WorkbookInfo WorkbookInfo { get; init; }
            public List<RenderResult> RenderResults { get; init; }
        }

        public record struct RenderResult
        {
            public string Contents { get; init; }
            public string OutputFpath { get; init; }
        }


        internal static bool TryGetCodeGenInfo(in CodeGeneratorOptions opt, out CodeGenInfo outCodeGenInfo)
        {
            DateTime date = DateTime.Now;

            // preload templates
            string templateStrConst = LoadTemplateOrNull(opt.TemplateDir, opt.TemplateConstPath, "const.liquid");
            string templateStrEnum = LoadTemplateOrNull(opt.TemplateDir, opt.TemplateEnumPath, "enum.liquid");
            string templateStrClass = LoadTemplateOrNull(opt.TemplateDir, opt.TemplateClassPath, "class.liquid");
            Template templateConst = Template.Parse(templateStrConst);
            Template templateEnum = Template.Parse(templateStrEnum);
            Template templateClass = Template.Parse(templateStrClass);

            // generate workbookresultinfo from input excel paths
            List<string> excelPaths = GetExcelFpaths(opt.InputExcelPaths);
            List<WorkbookResultInfo> wris = new List<WorkbookResultInfo>(excelPaths.Count);
            foreach (string excelPath in excelPaths)
            {
                using (FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XSSFWorkbook excel = new XSSFWorkbook(fileStream);
                    // force evaulate! NPOI.XSSF.UserModel.XSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook)

                    if (!WorkbookInfo.TryGetWorkbookInfo(excel, opt, out WorkbookInfo workbookInfo))
                    {
                        continue;
                    }

                    List<RenderResult> rrs = new List<RenderResult>(workbookInfo.ClassSheets.Length + workbookInfo.ConstSheets.Length + workbookInfo.EnumSheets.Length);
                    foreach (ClassSheet data in workbookInfo.ClassSheets)
                    {
                        string n = data.sheet_info.sheet_name;
                        if (data.reserved_dic.TryGetValue(ReservedCell.E_RESERVED.TABLE, out ReservedCell cell))
                        {
                            if (string.IsNullOrEmpty(cell.CellString))
                            {
                                n = cell.CellString;
                            }
                        }
                        string outputPathClass = $"{opt.OutputDir}/{n}.cs";
                        Hash o = Hash.FromAnonymousObject(new { date, data });
                        string rendered = templateClass.Render(o);
                        rrs.Add(new RenderResult { OutputFpath = outputPathClass, Contents = rendered });
                    }
                    foreach (ConstSheet data in workbookInfo.ConstSheets)
                    {
                        string n = data.sheet_info.sheet_name;
                        if (data.reserved_dic.TryGetValue(ReservedCell.E_RESERVED.TABLE, out ReservedCell cell))
                        {
                            if (string.IsNullOrEmpty(cell.CellString))
                            {
                                n = cell.CellString;
                            }
                        }
                        string outputPathClass = $"{opt.OutputDir}/{n}.cs";
                        Hash o = Hash.FromAnonymousObject(new { date, data });
                        string rendered = templateConst.Render(o);
                        rrs.Add(new RenderResult { OutputFpath = outputPathClass, Contents = rendered });
                    }
                    foreach (EnumSheet data in workbookInfo.EnumSheets)
                    {
                        string n = data.sheet_info.sheet_name;
                        if (data.reserved_dic.TryGetValue(ReservedCell.E_RESERVED.TABLE, out ReservedCell cell))
                        {
                            if (string.IsNullOrEmpty(cell.CellString))
                            {
                                n = cell.CellString;
                            }
                        }
                        string outputPathClass = $"{opt.OutputDir}/{n}.cs";
                        Hash o = Hash.FromAnonymousObject(new { date, data });
                        string rendered = templateEnum.Render(o);
                        rrs.Add(new RenderResult { OutputFpath = outputPathClass, Contents = rendered });
                    }
                    WorkbookResultInfo workbookResultInfo = new WorkbookResultInfo { WorkbookInfo = workbookInfo, RenderResults = rrs };
                    List<RenderResult> xs = workbookResultInfo.RenderResults;
                    if (xs == null)
                    {
                        continue;
                    }
                    wris.Add(workbookResultInfo);
                }
            }
            if (wris.Count == 0)
            {
                outCodeGenInfo = default;
                return false;
            }
            outCodeGenInfo = new CodeGenInfo { WorkbookResultInfos = wris };
            return true;
        }

        public static int Generate(in CodeGeneratorOptions opt)
        {
            if (string.IsNullOrEmpty(opt.OutputDir))
            {
                return 1;
            }

            if (!TryGetCodeGenInfo(opt, out CodeGenInfo codeGenInfo))
            {
                return 1;
            }

            if (!Directory.Exists(opt.OutputDir))
            {
                Directory.CreateDirectory(opt.OutputDir);
            }

            foreach (WorkbookResultInfo wris in codeGenInfo.WorkbookResultInfos)
            {
                foreach (RenderResult rr in wris.RenderResults)
                {
                    File.WriteAllText(path: rr.OutputFpath, contents: rr.Contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                }
            }
            return 0;
        }

        private static List<string> GetExcelFpaths(in IEnumerable<string> inputExcelPaths)
        {
            int pathCounts = inputExcelPaths.Count();
            List<string> ret = new List<string>(pathCounts);

            HashSet<string> set = new HashSet<string>(pathCounts);
            foreach (string path in inputExcelPaths)
            {
                FileAttributes attr = File.GetAttributes(path);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    string[] pathsInDir = Directory.GetFiles(path, "*.xlsx");
                    foreach (string pathIndir in pathsInDir)
                    {
                        if (!set.Add(pathIndir))
                        {
                            ret.Add(pathIndir);
                        }
                    }
                }
                else
                {
                    if (!set.Add(path))
                    {
                        ret.Add(path);
                    }
                }
            }
            set.Clear();

            return ret;
        }

        private static string LoadTemplateOrNull(in string templateDir, in string templateFPath, in string innerPath)
        {
            if (File.Exists(templateFPath))
            {
                return File.ReadAllText(templateFPath);
            }

            if (!string.IsNullOrEmpty(templateDir))
            {
                string path = Path.Combine(templateDir, innerPath);
                if (!File.Exists(path))
                {
                    return null;
                }
                return File.ReadAllText(path);
            }

            using (Stream stream = typeof(Program).Assembly.GetManifestResourceStream(innerPath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}