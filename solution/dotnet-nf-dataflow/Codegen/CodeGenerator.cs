using DotLiquid;
using NF.Tools.DataFlow.CodeGen.Internal;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("NF.Tools.DataFlow.Test")]
namespace NF.Tools.DataFlow.CodeGen
{
    public class CodeGenerator
    {
        public readonly record struct CodeGenInfo(List<WorkbookResultInfo> WorkbookResultInfos);

        public readonly record struct WorkbookResultInfo(WorkbookInfo WorkbookInfo, List<RenderResult> RenderResults);
        public readonly record struct RenderResult(string Contents, string OutputFpath);

        private readonly CodeGeneratorOptions _opt;
        private readonly Template _templateConst;
        private readonly Template _templateEnum;
        private readonly Template _templateClass;

        public CodeGenerator(CodeGeneratorOptions opt)
        {
            this._opt = opt;

            string templateStrConst = _LoadTemplateOrNull(opt.TemplateDir, opt.TemplateConstPath, "const.liquid");
            string templateStrEnum = _LoadTemplateOrNull(opt.TemplateDir, opt.TemplateEnumPath, "enum.liquid");
            string templateStrClass = _LoadTemplateOrNull(opt.TemplateDir, opt.TemplateClassPath, "class.liquid");

            _templateConst = _getGenerator(templateStrConst);
            _templateEnum = _getGenerator(templateStrEnum);
            _templateClass = _getGenerator(templateStrClass);
            // =======================================================

            string _LoadTemplateOrNull(string templateDir, string templateFPath, string innerPath)
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

            Template _getGenerator(string template)
            {
                //FormatCompiler compiler = new FormatCompiler { RemoveNewLines = false };
                //compiler.RegisterTag(this.Tag, true);
                //return compiler.Compile(template);
                return Template.Parse(template);
            }
        }

        public bool TryGetCodeGenInfo(out CodeGenInfo outCodeGenInfo)
        {
            var excelPaths = Directory.GetFiles(_opt.InputExcelDir, "*.xlsx");
            List<WorkbookResultInfo> rrs = new List<WorkbookResultInfo>(excelPaths.Length);

            foreach (string excelPath in excelPaths)
            {
                IWorkbook excel = _getExcelDataReader(excelPath);
                if (!TryGetWorkbookResultInfo(excel, out WorkbookResultInfo workbookResultInfo))
                {
                    continue;
                }
                List<RenderResult> xs = workbookResultInfo.RenderResults;
                if (xs == null)
                {
                    outCodeGenInfo = default;
                    return false;
                }
                rrs.Add(workbookResultInfo);
            }
            outCodeGenInfo = new CodeGenInfo(rrs);
            return true;

            // ========================================
            IWorkbook _getExcelDataReader(string excelFpath)
            {
                using (FileStream fileStream = File.Open(excelFpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XSSFWorkbook workbook = new XSSFWorkbook(fileStream);
                    // force evaulate! NPOI.XSSF.UserModel.XSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook)
                    return workbook;
                }
            }

            bool TryGetWorkbookResultInfo(IWorkbook excel, out WorkbookResultInfo outWorkbookResultInfo)
            {
                DateTime date = DateTime.Now;

                if (!WorkbookInfo.TryGetWorkbookInfo(excel, _opt, out WorkbookInfo workbookInfo))
                {
                    // 
                    outWorkbookResultInfo = default(WorkbookResultInfo);
                    return false;
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
                    string outputPathClass = $"{_opt.OutputDir}/{n}.cs";
                    Hash o = Hash.FromAnonymousObject(new { date, data });
                    string rendered = _templateClass.Render(o);
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
                    string outputPathClass = $"{_opt.OutputDir}/{n}.cs";
                    Hash o = Hash.FromAnonymousObject(new { date, data });
                    string rendered = _templateConst.Render(o);
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
                    string outputPathClass = $"{_opt.OutputDir}/{n}.cs";
                    Hash o = Hash.FromAnonymousObject(new { date, data });
                    string rendered = _templateEnum.Render(o);
                    rrs.Add(new RenderResult { OutputFpath = outputPathClass, Contents = rendered });
                }
                outWorkbookResultInfo = new WorkbookResultInfo { WorkbookInfo = workbookInfo, RenderResults = rrs };
                return true;
            }
        }

        public int Generate()
        {
            if (string.IsNullOrEmpty(_opt.OutputDir))
            {
                return 1;
            }

            if (!Directory.Exists(_opt.OutputDir))
            {
                Directory.CreateDirectory(_opt.OutputDir);
            }

            if (!TryGetCodeGenInfo(out CodeGenInfo codeGenInfo))
            {
                return 1;
            }
            foreach (WorkbookResultInfo wris in codeGenInfo.WorkbookResultInfos)
            {
                foreach (RenderResult rr in wris.RenderResults)
                {
                    _writeToFile(rr);
                }
            }

            return 0;

            // ==============================================================
            void _writeToFile(RenderResult rr)
            {
                using (StreamWriter sw = new StreamWriter(rr.OutputFpath, false, new UTF8Encoding(false)))
                {
                    sw.Write(rr.Contents);
                }
            }
        }
    }
}