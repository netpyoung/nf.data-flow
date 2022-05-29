using DotLiquid;
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
        public record struct RenderResult
        {
            public string Contents { get; init; }
            public string OutputFpath { get; init; }
        }

        public enum E_GENERATE_DB_RESULT
        {
            OK,
            FAIL_COMPILE,
            FAIL_DB_OPEARTE,
        }

        public static int Run(in DataFlowRunnerOption opt)
        {
            if (string.IsNullOrEmpty(opt.OutputCodeDir) || string.IsNullOrEmpty(opt.OutputDbPath))
            {
                return 1;
            }

            if (!TryGetWorkbookInfos(opt, out WorkbookInfo[] workbookInfos))
            {
                return 1;
            }
            RenderResult[] rrs = GetRenderResultsOrNull(workbookInfos, opt);
            if (rrs == null)
            {
                return 1;
            }

            if (!string.IsNullOrEmpty(opt.OutputCodeDir))
            {
                GenerateCode(rrs, opt.OutputCodeDir);
            }

            if (!string.IsNullOrEmpty(opt.OutputDbPath))
            {
                E_GENERATE_DB_RESULT dbResult = GenerateDb(workbookInfos, rrs, opt.Password, opt.OutputDbPath);
                if (dbResult != E_GENERATE_DB_RESULT.OK)
                {
                    return 1;
                }
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
                File.WriteAllText(path: rr.OutputFpath, contents: rr.Contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            }
        }

        private static E_GENERATE_DB_RESULT GenerateDb(in WorkbookInfo[] workbookInfos, in RenderResult[] rrs, in string dbPassword, in string dbPath)
        {
            List<SyntaxTree> trees = new List<SyntaxTree>(rrs.Length + 1);
            foreach (ref readonly DataFlowRunner.RenderResult r in rrs.AsSpan())
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(r.Contents);
                trees.Add(syntaxTree);
            }
            // TODO(pyoung): SQLite.Attributes.txt 말고 다른것들도 처리했으면 좋겠는데..
            using (Stream stream = typeof(Program).Assembly.GetManifestResourceStream("SQLite.Attributes.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string content = reader.ReadToEnd();
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(content);
                trees.Add(syntaxTree);
            }
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

            Assembly asm = null;
            using (MemoryStream peStream = new MemoryStream())
            using (MemoryStream pdbStream = new MemoryStream())
            {
                EmitResult emitResult = compilation.Emit(peStream, pdbStream);
                if (!emitResult.Success)
                {
                    return E_GENERATE_DB_RESULT.FAIL_COMPILE;
                }
                peStream.Seek(0, SeekOrigin.Begin);
                pdbStream.Seek(0, SeekOrigin.Begin);
                asm = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(peStream, pdbStream);
            }

            Type[] assemplyTypoes = asm.GetTypes();
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

        internal static RenderResult[] GetRenderResultsOrNull(in WorkbookInfo[] workbookInfos, in DataFlowRunnerOption opt)
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
            foreach (ref readonly WorkbookInfo workbookInfo in workbookInfos.AsSpan())
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
                    string outputPathClass = $"{opt.OutputCodeDir}/{n}.cs";
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
                    string outputPathClass = $"{opt.OutputCodeDir}/{n}.cs";
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
                    string outputPathClass = $"{opt.OutputCodeDir}/{n}.cs";
                    Hash o = Hash.FromAnonymousObject(new { date, data });
                    string rendered = templateEnum.Render(o);
                    rrs.Add(new RenderResult { OutputFpath = outputPathClass, Contents = rendered });
                }
            }
            return rrs.ToArray();
        }

        internal static bool TryGetWorkbookInfos(in DataFlowRunnerOption opt, out WorkbookInfo[] outWorkbookInfos)
        {
            List<string> excelPaths = GetExcelFpaths(opt.InputExcelPaths);
            if (excelPaths.Count == 0)
            {
                outWorkbookInfos = default;
                return false;
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
            outWorkbookInfos = infos.ToArray();
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
                        if (set.Add(pathIndir))
                        {
                            ret.Add(pathIndir);
                        }
                    }
                }
                else
                {
                    if (set.Add(path))
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