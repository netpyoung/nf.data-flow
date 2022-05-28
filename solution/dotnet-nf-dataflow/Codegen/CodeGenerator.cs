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
        public record struct RenderResult
        {
            public string Contents { get; init; }
            public string OutputFpath { get; init; }
        }

        public static int Generate(in CodeGeneratorOptions opt)
        {
            if (string.IsNullOrEmpty(opt.OutputDir))
            {
                return 1;
            }

            if (!TryGetWorkbookInfos(opt, out List<WorkbookInfo> workbookInfos))
            {
                return 1;
            }
            List<RenderResult> rrs = GetRenderResultsOrNull(workbookInfos, opt);
            if (rrs == null)
            {
                return 1;
            }

            if (!Directory.Exists(opt.OutputDir))
            {
                Directory.CreateDirectory(opt.OutputDir);
            }
            foreach (var rr in rrs)
            {
                File.WriteAllText(path: rr.OutputFpath, contents: rr.Contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            }
            return 0;
        }

        internal static List<RenderResult> GetRenderResultsOrNull(in List<WorkbookInfo> workbookInfos, in CodeGeneratorOptions opt)
        {
            int sum = workbookInfos.Select(x => x.ClassSheets.Length + x.ConstSheets.Length + x.EnumSheets.Length).Sum();
            List<RenderResult> rrs = new List<RenderResult>(sum);

            // preload templates
            string templateStrConst = LoadTemplateOrNull(opt.TemplateDir, opt.TemplateConstPath, "const.liquid");
            string templateStrEnum = LoadTemplateOrNull(opt.TemplateDir, opt.TemplateEnumPath, "enum.liquid");
            string templateStrClass = LoadTemplateOrNull(opt.TemplateDir, opt.TemplateClassPath, "class.liquid");
            Template templateConst = Template.Parse(templateStrConst);
            Template templateEnum = Template.Parse(templateStrEnum);
            Template templateClass = Template.Parse(templateStrClass);

            // render
            DateTime date = DateTime.Now;
            foreach (WorkbookInfo workbookInfo in workbookInfos)
            {
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
            }
            return rrs;
        }

        internal static bool TryGetWorkbookInfos(in CodeGeneratorOptions opt, out List<WorkbookInfo> outCodeGenInfo)
        {
            List<string> excelPaths = GetExcelFpaths(opt.InputExcelPaths);
            if (excelPaths.Count == 0)
            {
                outCodeGenInfo = default;
                return false;
            }

            outCodeGenInfo = new List<WorkbookInfo>(excelPaths.Count);
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
                    outCodeGenInfo.Add(workbookInfo);
                }
            }
            return true;
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