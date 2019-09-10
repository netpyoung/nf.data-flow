using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotLiquid;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace NF.Tools.DataFlow.CodeGen
{
    public class ExcelClassGenerator
    {
        public ExcelClassGenerator(string excelFpath)
        {
            this._reader = this.GetExcelDataReader(excelFpath);
            this._excelFpath = excelFpath;
        }

        internal static void Generate(string inputExcelFpath, string templateDir, string outputDir)
        {
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            new ExcelClassGenerator(inputExcelFpath).Render(templateDir, outputDir);
        }

        private IWorkbook GetExcelDataReader(string excelFpath)
        {
            using (FileStream fileStream = File.Open(excelFpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XSSFWorkbook workbook = new XSSFWorkbook(fileStream);
                // force evaulate! NPOI.XSSF.UserModel.XSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook)
                return workbook;
            }
        }

        private int GetColumnMax(ISheet sheet)
        {
            IRow row = sheet.GetRow(0);
            if (row == null)
            {
                return 0;
            }

            for (int cellIndex = 0; cellIndex < row.LastCellNum; ++cellIndex)
            {
                ICell cell = row.GetCell(cellIndex);

                if (cell?.CellStyle?.FillForegroundColorColor != null)
                {
                    if (cell.CellStyle.FillForegroundColorColor.RGB.SequenceEqual(this.END_COLOR))
                    {
                        return cellIndex;
                    }
                }
            }

            return row.LastCellNum;
        }

        #region const

        private ConstDeclare GetConstDeclareFromSingleSheet(ISheet sheet)
        {
            ConstDeclare ret = new ConstDeclare();
            List<ConstMember> args = new List<ConstMember>();
            for (int rownum = 1; rownum <= sheet.LastRowNum; ++rownum)
            {
                IRow row = sheet.GetRow(rownum);
                if (row == null)
                {
                    continue;
                }

                string partvalue = row.GetCell((int) E_SINGLE_CONST_COLUMN.PART).StringOrNull();
                if (string.IsNullOrEmpty(partvalue))
                {
                    continue;
                }

                E_PART part = (E_PART) Enum.Parse(typeof(E_PART), partvalue);
                string typevalue = row.GetCell((int) E_SINGLE_CONST_COLUMN.TYPE).StringOrNull();
                string attribute = row.GetCell((int) E_SINGLE_CONST_COLUMN.ATTRIBUTE).StringOrNull();
                string name = row.GetCell((int) E_SINGLE_CONST_COLUMN.NAME).StringCellValue;
                string value = row.GetCell((int) E_SINGLE_CONST_COLUMN.VALUE).StringOrNull();
                string desc = row.GetCell((int) E_SINGLE_CONST_COLUMN.DESC).StringOrNull();

                string[] attributes = null;
                if (!string.IsNullOrEmpty(attribute))
                {
                    attributes = attribute.Split(';').ToArray();
                }

                args.Add(new ConstMember
                {
                    part = part,
                    type = typevalue,
                    name = name,
                    value = value,
                    desc = desc,
                    attributes = attributes
                });
            }

            ret.name = sheet.SheetName.Remove(0, COST_SHEET_PREFIX.Length);
            ret.const_members = args.ToArray();
            return ret;
        }

        #endregion const

        private void WriteToFile(string path, string contents)
        {
            using (StreamWriter sw = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                sw.Write(contents);
            }
        }

        #region Predefined

        private const string SINGLE_ENUM_SHEETNAME = "_ENUM";
        private const string COST_SHEET_PREFIX = "_C_";
        private const string ENUM_SHEET_PREFIX = "_E_";
        private const string IGNORE_PREFIX = "_";

        private Dictionary<string, Template> GetClassRenderDic(string template_dir)
        {
            Dictionary<string, Template> ret = new Dictionary<string, Template>();
            ISheet sheet = this._reader.GetSheet("_RENDERER");
            if (sheet == null)
            {
                return ret;
            }

            int columnmax = this.GetColumnMax(sheet);
            for (int rownum = 0; rownum <= sheet.LastRowNum; ++rownum)
            {
                IRow row = sheet.GetRow(rownum);
                if (row == null)
                {
                    break;
                }


                ICell tpl_cell = row.GetCell(0);
                if (tpl_cell == null)
                {
                    break;
                }


                string template_name = tpl_cell.StringCellValue;
                if (string.IsNullOrEmpty(template_name))
                {
                    break;
                }


                Template gen = this.GetGenerator(File.ReadAllText(Path.Combine(template_dir, template_name)));

                for (int cellnum = 1; cellnum < columnmax; ++cellnum)
                {
                    ICell cell = row.GetCell(cellnum);
                    if (cell == null)
                    {
                        break;
                    }

                    string cell_value = cell.StringCellValue;
                    if (string.IsNullOrEmpty(cell_value))
                    {
                        break;
                    }

                    ret.Add(cell_value, gen);
                }
            }

            return ret;
        }

        public void Render(string templateDir, string outputDir)
        {
            string tplConst = File.ReadAllText(Path.Combine(templateDir, "const.liquid"));
            string tplEnum = File.ReadAllText(Path.Combine(templateDir, "enum.liquid"));
            string tplClass = File.ReadAllText(Path.Combine(templateDir, "class.liquid"));

            Template genConst = this.GetGenerator(tplConst);
            Template genEnum = this.GetGenerator(tplEnum);
            Template genClass = this.GetGenerator(tplClass);

            Dictionary<string, Template> classDic = this.GetClassRenderDic(templateDir);

            List<ClassDeclare> lst = new List<ClassDeclare>();

            for (int sheetIndex = 0; sheetIndex < this._reader.NumberOfSheets; ++sheetIndex)
            {
                ISheet sheet = this._reader.GetSheetAt(sheetIndex);
                string sheetname = sheet.SheetName;
                if (sheetname.StartsWith(COST_SHEET_PREFIX))
                {
                    ConstDeclare constDeclare = this.GetConstDeclareFromSingleSheet(sheet);
                    string path = Path.Combine(outputDir, $"const_{constDeclare.name}.cs");
                    this.WriteToFile(path, genConst.Render(Hash.FromAnonymousObject(new {c = constDeclare})));
                }
                else if (sheetname == SINGLE_ENUM_SHEETNAME)
                {
                    EnumDeclare[] enumDeclares = this.GetEnumDeclares(sheet);
                    string path = Path.Combine(outputDir, "enum.autogen.cs");
                    this.WriteToFile(path, genEnum.Render(Hash.FromAnonymousObject(new {list = enumDeclares})));
                }
                else if (sheetname.StartsWith(ENUM_SHEET_PREFIX))
                {
                    EnumDeclare enumDeclare = this.GetEnumDeclareFromSingleSheet(sheet);
                    string path = Path.Combine(outputDir, $"enum_{enumDeclare.name}.cs");
                    this.WriteToFile(path,
                        genEnum.Render(Hash.FromAnonymousObject(new {list = new[] {enumDeclare}})));
                }

                if (sheetname.StartsWith(IGNORE_PREFIX))
                {
                    continue;
                }

                if (classDic.ContainsKey(sheet.SheetName))
                {
                    string path = Path.Combine(outputDir, $"class_{sheet.SheetName}.autogen.cs");
                    this.WriteToFile(path,
                        classDic[sheet.SheetName]
                            .Render(Hash.FromAnonymousObject(new {c = this.GetClaassDeclare(sheet)})));
                }
                else
                {
                    lst.Add(this.GetClaassDeclare(sheet));
                }
            }

            string pathc = Path.Combine(outputDir, "class.autogen.cs");
            this.WriteToFile(pathc, genClass.Render(Hash.FromAnonymousObject(new {date = DateTime.Now, list = lst})));

            foreach (FileInfo file in new DirectoryInfo(templateDir).GetFiles())
            {
                if (file.Name.EndsWith(".cs"))
                {
                    string temppath = Path.Combine(outputDir, file.Name);
                    file.CopyTo(temppath, true);
                }
            }
        }

        private Template GetGenerator(string template)
        {
            //FormatCompiler compiler = new FormatCompiler { RemoveNewLines = false };
            //compiler.RegisterTag(this.Tag, true);
            //return compiler.Compile(template);
            return Template.Parse(template);
        }

        private enum E_DATA_ROW
        {
            PART = 0,
            ATTRIBUTE = 1,
            TYPE = 2,
            NAME = 3
        }

        private enum E_ENUM_ROW
        {
            PART = 0,
            ATTRIBUTE = 1,
            NAME = 2
        }

        private enum E_SINGLE_ENUM_COLUMN
        {
            PART = 0,
            ATTRIBUTE = 1,
            NAME = 2,
            VALUE = 3,
            DESC = 4
        }

        private enum E_SINGLE_CONST_COLUMN
        {
            PART = 0,
            ATTRIBUTE = 1,
            TYPE = 2,
            NAME = 3,
            VALUE = 4,
            DESC = 5
        }

        private readonly byte[] END_COLOR = {255, 255, 0};
        private string _excelFpath;
        private readonly IWorkbook _reader;

        #endregion Predefined

        #region enum

        private EnumDeclare GetEnumDeclareFromSingleSheet(ISheet sheet)
        {
            EnumDeclare ret = new EnumDeclare();
            List<EnumMember> args = new List<EnumMember>();
            for (int rownum = 1; rownum <= sheet.LastRowNum; ++rownum)
            {
                IRow row = sheet.GetRow(rownum);
                string part_value = row.GetCell((int) E_SINGLE_ENUM_COLUMN.PART).StringOrNull();
                if (string.IsNullOrEmpty(part_value))
                {
                    continue;
                }

                E_PART part = (E_PART) Enum.Parse(typeof(E_PART), part_value);
                string attribute = row.GetCell((int) E_SINGLE_ENUM_COLUMN.ATTRIBUTE).StringOrNull();
                string name = row.GetCell((int) E_SINGLE_ENUM_COLUMN.NAME).StringOrNull();
                string value = row.GetCell((int) E_SINGLE_ENUM_COLUMN.VALUE).StringOrNull();
                string desc = row.GetCell((int) E_SINGLE_ENUM_COLUMN.DESC).StringOrNull();

                string[] attributes = null;
                if (!string.IsNullOrEmpty(attribute))
                {
                    attributes = attribute.Split(';').ToArray();
                }

                args.Add(new EnumMember {name = name, value = value, desc = desc, attributes = attributes});
            }

            ret.name = sheet.SheetName.Remove(0, ENUM_SHEET_PREFIX.Length);
            ret.args = args.ToArray();
            return ret;
        }

        private EnumDeclare[] GetEnumDeclares(ISheet sheet)
        {
            List<EnumDeclare> ret = new List<EnumDeclare>();

            for (int rownum = 1; rownum <= sheet.LastRowNum; ++rownum)
            {
                EnumDeclare item = this.GetEnumDeclare(sheet, rownum);
                if (item != null)
                {
                    ret.Add(item);
                }
            }

            return ret.ToArray();
        }

        private EnumDeclare GetEnumDeclare(ISheet sheet, int rownum)
        {
            EnumDeclare ret = new EnumDeclare();
            IRow row = sheet.GetRow(rownum);
            ICell partCell = row.GetCell((int) E_ENUM_ROW.PART);
            if (partCell == null)
            {
                return null;
            }

            ICell attributeCell = row.GetCell((int) E_ENUM_ROW.ATTRIBUTE);
            ICell nameCell = row.GetCell((int) E_ENUM_ROW.NAME);
            if (string.IsNullOrEmpty(nameCell.StringCellValue))
            {
                return null;
            }

            E_PART part = (E_PART) Enum.Parse(typeof(E_PART), partCell.StringCellValue);

            string attribute = null;
            if (attributeCell != null)
            {
                attribute = attributeCell.StringCellValue;
            }

            List<string> args = new List<string>();

            for (int cellnum = (int) E_ENUM_ROW.NAME + 1; cellnum < this.GetColumnMax(sheet); ++cellnum)
            {
                ICell cell = row.GetCell(cellnum);
                if (cell == null)
                {
                    break;
                }

                if (string.IsNullOrEmpty(cell.StringCellValue))
                {
                    break;
                }

                args.Add(cell.StringCellValue);
            }

            return EnumDeclare.GenEnumDeclare(part, attribute, nameCell.StringCellValue, args.ToArray());
        }

        #endregion enum

        #region class

        private ClassDeclare GetClaassDeclare(ISheet sheet)
        {
            ClassDeclare ret = new ClassDeclare {name = sheet.SheetName};
            List<ClassMember> lst = new List<ClassMember>();
            int max = this.GetColumnMax(sheet);
            for (int i = 0; i < max; ++i)
            {
                ClassMember item = this.GetMemberDeclare(sheet, i);
                if (item != null)
                {
                    lst.Add(item);
                }
            }

            ret.class_members = lst.ToArray();
            return ret;
        }

        private ClassMember GetMemberDeclare(ISheet sheet, int index)
        {
            IRow partRow = sheet.GetRow((int) E_DATA_ROW.PART);
            ICell partCell = partRow.GetCell(index);
            if (partCell == null)
            {
                return null;
            }

            IRow attributeRow = sheet.GetRow((int) E_DATA_ROW.ATTRIBUTE);

            ICell attributeCell = attributeRow == null ? null : attributeRow.GetCell(index);
            IRow typeRow = sheet.GetRow((int) E_DATA_ROW.TYPE);
            ICell typeCell = typeRow.GetCell(index);
            IRow columnRow = sheet.GetRow((int) E_DATA_ROW.NAME) ?? throw new ArgumentNullException("sheet.GetRow((int) E_DATA_ROW.NAME)");
            ICell columnCell = columnRow.GetCell(index);

            string name = columnCell.StringOrNull();
            string type = typeCell.StringOrNull();

            string attribute = string.Empty;
            if (attributeCell != null)
            {
                attribute = attributeCell.StringOrNull();
            }

            E_PART part = (E_PART) Enum.Parse(typeof(E_PART), partCell.StringCellValue);

            return ClassMember.GenClassMember(part, attribute, type, name);
        }

        #endregion class
    }

    public class BasePart : Drop
    {
        public E_PART part = E_PART.Common;
        public bool is_client => this.part.HasFlag(E_PART.Client);
        public bool is_client_only => this.part == E_PART.Client;
        public bool is_server => this.part.HasFlag(E_PART.Server);
        public bool is_server_only => this.part == E_PART.Server;
    }

    [Flags]
    public enum E_PART
    {
        Client = 1 << 0,
        Server = 1 << 1,
        Common = Client | Server
    }

    #region ConstSheet

    public class ConstDeclare : Drop
    {
        public string name { get; set; }
        public ConstMember[] const_members { get; set; }
    }

    public class ConstMember : BasePart
    {
        public string[] attributes { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string desc { get; set; }
        public bool is_string => this.type == "string";
    }

    #endregion ConstSheet

    #region EnumSheet

    public class EnumSheet : Drop
    {
        public EnumDeclare[] enum_declares { get; set; }
    }

    public class EnumDeclare : BasePart
    {
        public string[] attributes { get; set; }
        public string name { get; set; }
        public EnumMember[] args { get; set; }

        public static EnumDeclare GenEnumDeclare(E_PART part, string attribute, string name, string[] args)
        {
            string[] attributes = null;
            if (!string.IsNullOrEmpty(attribute))
            {
                attributes = attribute.Split(';').ToArray();
            }

            return new EnumDeclare
            {
                part = part,
                attributes = attributes,
                name = name,
                args = EnumMember.GenEnumMembers(args)
            };
        }
    }

    public class EnumMember : BasePart
    {
        public string[] attributes { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string desc { get; set; }

        public static EnumMember[] GenEnumMembers(string[] args)
        {
            List<EnumMember> ret = new List<EnumMember>();
            foreach (string arg in args)
            {
                ret.Add(new EnumMember {name = arg});
            }

            return ret.ToArray();
        }
    }

    #endregion EnumSheet

    #region ClassSheet

    public class ClassDeclare : Drop
    {
        public string name { get; set; }
        public ClassMember[] class_members { get; set; }
    }

    public class ClassMember : BasePart
    {
        public string[] attributes { get; set; }
        public string type { get; set; }
        public string name { get; set; }

        public static ClassMember GenClassMember(E_PART part, string attribute, string type, string name)
        {
            string[] attributes = null;
            if (!string.IsNullOrEmpty(attribute))
            {
                attributes = attribute.Split(';').ToArray();
            }

            return new ClassMember {part = part, attributes = attributes, type = type, name = name};
        }
    }

    #endregion ClassSheet

    public static class ExtNPOI
    {
        public static string StringOrNull(this ICell cell)
        {
            if (cell == null)
            {
                return null;
            }

            return cell.ToString();
        }
    }

    public static class ExtSystem
    {
        public static bool HasFlag(this Enum self, Enum flag)
        {
            int selfValue = Convert.ToInt32(self);
            int flagValue = Convert.ToInt32(flag);

            return (selfValue & flagValue) == flagValue;
        }
    }
}