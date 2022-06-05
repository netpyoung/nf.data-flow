using DotLiquid;
using DotLiquid.NamingConventions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using NF.Tools.DataFlow.Internal;
using NPOI.XSSF.UserModel;
using SqlCipher4Unity3D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("NF.Tools.DataFlow.Test")]
namespace NF.Tools.DataFlow
{
    public class DataFlowRunner
    {
        public readonly struct RenderResult
        {
            public string RenderedText { get; init; }
            public string OutputFpath { get; init; }
        }

        public enum E_GENERATE_DB_RESULT
        {
            OK,
            FAIL_COMPILE,
            FAIL_DB_OPEARTE,
        }

        public readonly struct TemplatePaths
        {
            public string LiquidConst { get; init; }
            public string LiquidEnum { get; init; }
            public string LiquidClass { get; init; }
            public string IncludeCsharp { get; init; }
        }

        const string LIQUID_CONST = "const.liquid";
        const string LIQUID_ENUM = "enum.liquid";
        const string LIQUID_CLASS = "class.liquid";
        const string INCLUDE_CSHARP = "Include.cs";

        public static int Run(in DataFlowRunnerOption opt)
        {
            bool shouldGenerateCode = !string.IsNullOrEmpty(opt.output_code_dir);
            bool shouldGenerateDb = !string.IsNullOrEmpty(opt.output_db_path);
            if (!shouldGenerateCode && !shouldGenerateDb)
            {
                return 1;
            }

            WorkbookInfo[] workbookInfos = GetWorkbookInfosOrNull(opt);
            if (workbookInfos == null)
            {
                return 1;
            }

            TemplatePaths templatePaths = GetTemplatePaths(opt.template_paths);

            RenderResult[] rrs = GetRenderResultsOrNull(workbookInfos, templatePaths, opt.output_code_dir);
            if (rrs == null)
            {
                return 1;
            }

            bool isNeedGenerateAssembly = opt.pre_assemble || shouldGenerateDb;
            Assembly assembly = null;
            System.Runtime.Loader.AssemblyLoadContext context = null;
            if (isNeedGenerateAssembly)
            {
                string includeCsharpStr = null;
                if (!string.IsNullOrEmpty(templatePaths.IncludeCsharp))
                {
                    if (!File.Exists(templatePaths.IncludeCsharp))
                    {
                        return 1;
                    }
                    includeCsharpStr = File.ReadAllText(templatePaths.IncludeCsharp);
                }
                if (includeCsharpStr == null)
                {
                    using (Stream stream = typeof(DataFlowRunner).Assembly.GetManifestResourceStream(INCLUDE_CSHARP))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        includeCsharpStr = reader.ReadToEnd();
                    }
                }
                context = new System.Runtime.Loader.AssemblyLoadContext("temp", isCollectible: true);
                assembly = GetAssemblyOrNull(rrs, includeCsharpStr, context);
                if (assembly == null)
                {
                    return 1;
                }
            }

            if (shouldGenerateCode)
            {
                GenerateCode(rrs, opt.output_code_dir);
            }

            if (shouldGenerateDb)
            {
                E_GENERATE_DB_RESULT dbResult = GenerateDb(assembly, workbookInfos, opt.password, opt.output_db_path);
                if (dbResult != E_GENERATE_DB_RESULT.OK)
                {
                    return 1;
                }
            }
            if (context != null)
            {
                context.Unload();
            }
            return 0;
        }

        private static void GenerateCode(in RenderResult[] rrs, in string outputCodeDir)
        {
            if (!Directory.Exists(outputCodeDir))
            {
                Directory.CreateDirectory(outputCodeDir);
            }

            foreach (ref readonly RenderResult rr in rrs.AsSpan())
            {
                File.WriteAllText(path: rr.OutputFpath, contents: rr.RenderedText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            }
        }

        private static Assembly GetAssemblyOrNull(in RenderResult[] rrs, in string includeCsharpStr, in System.Runtime.Loader.AssemblyLoadContext context)
        {
            List<SyntaxTree> trees = new List<SyntaxTree>(rrs.Length + 1);
            foreach (ref readonly DataFlowRunner.RenderResult r in rrs.AsSpan())
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(r.RenderedText);
                trees.Add(syntaxTree);
            }
            trees.Add(CSharpSyntaxTree.ParseText(includeCsharpStr));
            MetadataReference[] referenceArray = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DescriptionAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll")),
            };
            SyntaxTree[] treeArr = trees.ToArray();
            CSharpCompilation compilation = CSharpCompilation.Create(
                "assemblyName",
                treeArr,
                referenceArray,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (MemoryStream peStream = new MemoryStream())
            using (MemoryStream pdbStream = new MemoryStream())
            {
                EmitResult emitResult = compilation.Emit(peStream, pdbStream);
                if (!emitResult.Success)
                {
                    return null;
                }
                peStream.Seek(0, SeekOrigin.Begin);
                pdbStream.Seek(0, SeekOrigin.Begin);
                return context.LoadFromStream(peStream, pdbStream);
            }
        }

        private static E_GENERATE_DB_RESULT GenerateDb(in Assembly assembly, in WorkbookInfo[] workbookInfos, in string dbPassword, in string dbPath)
        {
            Type[] assemplyTypoes = assembly.GetTypes();
            List<Type> types = new List<Type>(assemplyTypoes.Length);
            foreach (Type type in assemplyTypoes)
            {
                if (!type.IsClass)
                {
                    continue;
                }
                if (!type.GetCustomAttributes(inherit: false).Any(x => x.GetType().Name.StartsWith("Export")))
                {
                    continue;
                }
                types.Add(type);
            }

            try
            {
                ExcelLoader loader = new ExcelLoader(workbookInfos);
                using (SQLiteConnection conn = new SQLiteConnection(dbPath, dbPassword, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create))
                {
                    for (int i = 0; i < types.Count; ++i)
                    {
                        Type type = types[i];
                        List<object> dataList = loader.GetDataListOrNull(type, type.Name);
                        if (dataList == null)
                        {
                            continue;
                        }
                        Console.WriteLine($"{i + 1}/{types.Count} | {type} :  {dataList.Count}");

                        conn.DropTable(type);
                        conn.CreateTable(type);
                        conn.InsertAll(dataList, type);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false);
                return E_GENERATE_DB_RESULT.FAIL_DB_OPEARTE;
            }
            return E_GENERATE_DB_RESULT.OK;
        }

        internal static RenderResult[] GetRenderResultsOrNull(in WorkbookInfo[] workbookInfos, in TemplatePaths templatePaths, in string output_code_dir)
        {
            int sum = workbookInfos.Select(x => x.ClassSheets.Length + x.ConstSheets.Length + x.EnumSheets.Length).Sum();
            List<RenderResult> rrs = new List<RenderResult>(sum);

            // preload templates
            string templateStrConst = LoadTemplateOrNull(templatePaths.LiquidConst, LIQUID_CONST);
            string templateStrEnum = LoadTemplateOrNull(templatePaths.LiquidEnum, LIQUID_ENUM);
            string templateStrClass = LoadTemplateOrNull(templatePaths.LiquidClass, LIQUID_CLASS);
            Template.NamingConvention = new CSharpNamingConvention();
            Template templateConst = Template.Parse(templateStrConst);
            Template templateEnum = Template.Parse(templateStrEnum);
            Template templateClass = Template.Parse(templateStrClass);

            // render
            DateTime date = DateTime.Now;
            foreach (ref readonly WorkbookInfo workbookInfo in workbookInfos.AsSpan())
            {
                foreach (ClassSheet data in workbookInfo.ClassSheets)
                {
                    string n = data.SheetInfo.SheetName;
                    if (data.reserved_dic.TryGetValue(ReservedCell.E_RESERVED.TABLE, out ReservedCell cell))
                    {
                        if (string.IsNullOrEmpty(cell.CellString))
                        {
                            n = cell.CellString;
                        }
                    }
                    string outputPathClass = $"{output_code_dir}/{n}.cs";
                    Hash o = Hash.FromAnonymousObject(new { date, data });
                    string rendered = templateClass.Render(o);
                    rrs.Add(new RenderResult { OutputFpath = outputPathClass, RenderedText = rendered });
                }

                foreach (ConstSheet data in workbookInfo.ConstSheets)
                {
                    string n = data.SheetInfo.SheetName;
                    if (data.reserved_dic.TryGetValue(ReservedCell.E_RESERVED.TABLE, out ReservedCell cell))
                    {
                        if (string.IsNullOrEmpty(cell.CellString))
                        {
                            n = cell.CellString;
                        }
                    }
                    string outputPathClass = $"{output_code_dir}/{n}.cs";
                    Hash o = Hash.FromAnonymousObject(new { date, data });
                    string rendered = templateConst.Render(o);
                    rrs.Add(new RenderResult { OutputFpath = outputPathClass, RenderedText = rendered });
                }

                foreach (EnumSheet data in workbookInfo.EnumSheets)
                {
                    string n = data.SheetInfo.SheetName;
                    if (data.reserved_dic.TryGetValue(ReservedCell.E_RESERVED.TABLE, out ReservedCell cell))
                    {
                        if (string.IsNullOrEmpty(cell.CellString))
                        {
                            n = cell.CellString;
                        }
                    }
                    string outputPathClass = $"{output_code_dir}/{n}.cs";
                    Hash o = Hash.FromAnonymousObject(new { date, data });
                    string rendered = templateEnum.Render(o);
                    rrs.Add(new RenderResult { OutputFpath = outputPathClass, RenderedText = rendered });
                }
            }
            return rrs.ToArray();
        }

        internal static WorkbookInfo[] GetWorkbookInfosOrNull(in DataFlowRunnerOption opt)
        {
            List<string> excelPaths = GetExcelFpaths(opt.input_paths);
            if (excelPaths.Count == 0)
            {
                return null;
            }

            List<WorkbookInfo> infos = new List<WorkbookInfo>(excelPaths.Count);
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
                    infos.Add(workbookInfo);
                }
            }
            return infos.ToArray();
        }

        public static List<string> GetExcelFpaths(in IEnumerable<string> inputExcelPaths)
        {
            int pathCounts = inputExcelPaths.Count();
            List<string> ret = new List<string>(pathCounts);

            HashSet<string> set = new HashSet<string>(pathCounts);
            foreach (string path in inputExcelPaths)
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                FileAttributes attr = File.GetAttributes(path);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    string[] pathsInDir = Directory.GetFiles(path, "*.xlsx");
                    foreach (string pathIndir in pathsInDir)
                    {
                        if (set.Add(pathIndir))
                        {
                            ret.Add(pathIndir);
                        }
                    }
                }
                else
                {
                    if (Path.GetExtension(path) != ".xlsx")
                    {
                        continue;
                    }
                    if (set.Add(path))
                    {
                        ret.Add(path);
                    }
                }
            }
            set.Clear();

            return ret;
        }

        private static TemplatePaths GetTemplatePaths(in IEnumerable<string> inputTemplatePaths)
        {
            string liquidConst = null;
            string liquidEnum = null;
            string liquidClass = null;
            string includeCsharp = null;

            if (inputTemplatePaths != null)
            {
                foreach (string path in inputTemplatePaths)
                {
                    FileAttributes attr = File.GetAttributes(path);
                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        string[] pathsInDir = Directory.GetFiles(path, "*.liquid", SearchOption.TopDirectoryOnly);
                        foreach (string pathIndir in pathsInDir)
                        {
                            string filename = Path.GetFileName(pathIndir);
                            switch (filename)
                            {

                                case LIQUID_CONST:
                                    if (liquidConst == null)
                                    {
                                        liquidConst = pathIndir;
                                    }
                                    break;
                                case LIQUID_ENUM:
                                    if (liquidEnum == null)
                                    {
                                        liquidEnum = pathIndir;
                                    }
                                    break;
                                case LIQUID_CLASS:
                                    if (liquidClass == null)
                                    {
                                        liquidClass = pathIndir;
                                    }
                                    break;
                                case INCLUDE_CSHARP:
                                    if (includeCsharp == null)
                                    {
                                        includeCsharp = pathIndir;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        string filename = Path.GetFileName(path);
                        switch (filename)
                        {
                            case LIQUID_CONST:
                                if (liquidConst == null)
                                {
                                    liquidConst = path;
                                }
                                break;
                            case LIQUID_ENUM:
                                if (liquidEnum == null)
                                {
                                    liquidEnum = path;
                                }
                                break;
                            case LIQUID_CLASS:
                                if (liquidClass == null)
                                {
                                    liquidClass = path;
                                }
                                break;
                            case INCLUDE_CSHARP:
                                if (includeCsharp == null)
                                {
                                    includeCsharp = path;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return new TemplatePaths
            {
                LiquidClass = liquidConst,
                LiquidEnum = liquidEnum,
                LiquidConst = liquidConst,
                IncludeCsharp = includeCsharp,
            };
        }

        private static string LoadTemplateOrNull(in string templatePath, in string innerPath)
        {
            if (!string.IsNullOrEmpty(templatePath))
            {
                if (!File.Exists(templatePath))
                {
                    return null;
                }
                return File.ReadAllText(templatePath);
            }

            using (Stream stream = typeof(DataFlowRunner).Assembly.GetManifestResourceStream(innerPath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}