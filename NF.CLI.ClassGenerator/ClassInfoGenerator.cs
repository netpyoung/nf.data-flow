﻿namespace NF.CLI.ClassGenerator
{
    using Mustache;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NPOI.SS.UserModel;
    using System.Text;


    public class ExcelClassGenerator
    {
        class DateTag : InlineTagDefinition
        {
            DateTime time = DateTime.Now;

            public DateTag() : base("date")
            {
            }

            public override void GetText(TextWriter writer, Dictionary<string, object> arguments, Scope context)
            {
                writer.Write(time);
            }
        }

        internal static void Generate(string input_excel_fpath, string template_dir, string output_dir)
        {
            Console.WriteLine("FFF " + output_dir);
            if (!Directory.Exists(output_dir))
            {
                Directory.CreateDirectory(output_dir);
            }

            new ExcelClassGenerator(input_excel_fpath).Render(template_dir, output_dir);
        }

        #region Predefined
        const string SINGLE_ENUM_SHEETNAME = "_ENUM";
        const string COST_SHEET_PREFIX = "_C_";
        const string ENUM_SHEET_PREFIX = "_E_";
        const string IGNORE_PREFIX = "_";

        Dictionary<string, Generator> GetClassRenderDic(string template_dir)
        {
            var ret = new Dictionary<string, Generator>();
            ISheet sheet = this._reader.GetSheet("_RENDERER");
            if (sheet == null)
            {
                return ret;
            }

            var columnmax = GetColumnMax(sheet);
            for (int rownum = 0; rownum <= sheet.LastRowNum; ++rownum)
            {
                var row = sheet.GetRow(rownum);
                if (row == null)
                {
                    break;
                }


                ICell tpl_cell = row.GetCell(0);
                if (tpl_cell == null)
                {
                    break;
                }


                var template_name = tpl_cell.StringCellValue;
                if (string.IsNullOrEmpty(template_name))
                {
                    break;
                }


                Generator gen = GetGenerator(File.ReadAllText(Path.Combine(template_dir, template_name)));

                for (int cellnum = 1; cellnum < columnmax; ++cellnum)

                {
                    var cell = row.GetCell(cellnum);
                    if (cell == null)
                    {
                        break;
                    }

                    var cell_value = cell.StringCellValue;
                    if (string.IsNullOrEmpty(cell_value))
                    {
                        break;
                    }

                    ret.Add(cell_value, gen);
                }
            }
            return ret;
        }

        public void Render(string template_dir, string output_dir)
        {
            var tpl_const = File.ReadAllText(Path.Combine(template_dir, "const.mustache"));
            var tpl_enum = File.ReadAllText(Path.Combine(template_dir, "enum.mustache"));
            var tpl_class = File.ReadAllText(Path.Combine(template_dir, "class.mustache"));

            var gen_const = GetGenerator(tpl_const);
            var gen_enum = GetGenerator(tpl_enum);
            var gen_class = GetGenerator(tpl_class);

            Dictionary<string, Generator> class_dic = GetClassRenderDic(template_dir);

            var lst = new List<ClassDeclare>();

            for (int sheet_index = 0; sheet_index < this._reader.NumberOfSheets; ++sheet_index)
            {
                ISheet sheet = this._reader.GetSheetAt(sheet_index);
                string sheetname = sheet.SheetName;
                if (sheetname.StartsWith(COST_SHEET_PREFIX))
                {
                    var const_declare = GetConstDeclareFromSingleSheet(sheet);
                    var path = Path.Combine(output_dir, string.Format("const_{0}.cs", const_declare.Name));
                    WriteToFile(path, gen_const.Render(GetConstDeclareFromSingleSheet(sheet)));
                }
                else if (sheetname == SINGLE_ENUM_SHEETNAME)
                {
                    var enum_declares = GetEnumDeclares(sheet);
                    var path = Path.Combine(output_dir, "enum.cs");
                    WriteToFile(path, gen_enum.Render(enum_declares));
                }
                else if (sheetname.StartsWith(ENUM_SHEET_PREFIX))
                {
                    var enum_declare = GetEnumDeclareFromSingleSheet(sheet);
                    var path = Path.Combine(output_dir, string.Format("enum_{0}.cs", enum_declare.Name));
                    WriteToFile(path, gen_enum.Render(new EnumDeclare[] { enum_declare }));
                }

                if (sheetname.StartsWith(IGNORE_PREFIX))
                {
                    continue;
                }

                if (class_dic.ContainsKey(sheet.SheetName))
                {
                    var path = Path.Combine(output_dir, string.Format("class_{0}.cs", sheet.SheetName));
                    WriteToFile(path, class_dic[sheet.SheetName].Render(GetClaassDeclare(sheet)));
                }
                else
                {
                    lst.Add(GetClaassDeclare(sheet));
                }
            }
            var pathc = Path.Combine(output_dir, "class.cs");
            WriteToFile(pathc, gen_class.Render(lst));

            foreach (FileInfo file in new DirectoryInfo(template_dir).GetFiles())
            {
                if (file.Name.EndsWith(".cs"))
                {
                    string temppath = Path.Combine(output_dir, file.Name);
                    file.CopyTo(temppath, true);
                }
            }
        }

        Generator GetGenerator(string template)
        {
            FormatCompiler compiler = new FormatCompiler { RemoveNewLines = false };
            compiler.RegisterTag(this.Tag, true);
            return compiler.Compile(template);
        }

        enum E_DATA_ROW
        {
            PART = 0,
            ATTRIBUTE = 1,
            TYPE = 2,
            NAME = 3,
        }

        enum E_ENUM_ROW
        {
            PART = 0,
            ATTRIBUTE = 1,
            NAME = 2
        }

        enum E_SINGLE_ENUM_COLUMN
        {
            PART = 0,
            ATTRIBUTE = 1,
            NAME = 2,
            VALUE = 3,
            DESC = 4,
        }

        enum E_SINGLE_CONST_COLUMN
        {
            PART = 0,
            ATTRIBUTE = 1,
            TYPE = 2,
            NAME = 3,
            VALUE = 4,
            DESC = 5,
        }

        readonly byte[] END_COLOR = { 255, 255, 0 };
        DateTag Tag = new DateTag();
        string excel_fpath = null;
        IWorkbook _reader = null;

        #endregion Predefined

        public ExcelClassGenerator(string excel_fpath)
        {
            this._reader = GetExcelDataReader(excel_fpath);
            this.excel_fpath = excel_fpath;
        }

        IWorkbook GetExcelDataReader(string excel_fpath)
        {
            using (var fileStream = File.Open(excel_fpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fileStream);
                // force evaulate! NPOI.XSSF.UserModel.XSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook)
                return workbook;
            }
        }

        int GetColumnMax(ISheet sheet)
        {
            var row = sheet.GetRow(0);
            if (row == null)
                return 0;

            for (int cell_index = 0; cell_index < row.LastCellNum; ++cell_index)
            {
                var cell = row.GetCell(cell_index);
                if (cell == null)
                    continue;

                if (cell.CellStyle != null
                    && cell.CellStyle.FillForegroundColorColor != null
                    && Enumerable.SequenceEqual(cell.CellStyle.FillForegroundColorColor.RGB, END_COLOR))
                {
                    return cell_index;
                }
            }
            return row.LastCellNum;
        }

        #region enum

        EnumDeclare GetEnumDeclareFromSingleSheet(ISheet sheet)
        {
            var ret = new EnumDeclare();
            var args = new List<EnumMember>();
            for (int rownum = 1; rownum <= sheet.LastRowNum; ++rownum)
            {
                var row = sheet.GetRow(rownum);
                var part_value = row.GetCell((int)E_SINGLE_ENUM_COLUMN.PART).StringOrNull();
                if (string.IsNullOrEmpty(part_value))
                    continue;

                var part = (E_PART)Enum.Parse(typeof(E_PART), part_value);
                var attribute = row.GetCell((int)E_SINGLE_ENUM_COLUMN.ATTRIBUTE).StringOrNull();
                var name = row.GetCell((int)E_SINGLE_ENUM_COLUMN.NAME).StringOrNull();
                var value = row.GetCell((int)E_SINGLE_ENUM_COLUMN.VALUE).StringOrNull();
                var desc = row.GetCell((int)E_SINGLE_ENUM_COLUMN.DESC).StringOrNull();

                string[] Attributes = null;
                if (!string.IsNullOrEmpty(attribute))
                {
                    Attributes = attribute.Split(';').ToArray();
                }
                args.Add(new EnumMember { Name = name, Value = value, Desc = desc, Attributes = Attributes });
            }
            ret.Name = sheet.SheetName.Remove(0, ENUM_SHEET_PREFIX.Length);
            ret.args = args.ToArray();
            return ret;
        }

        EnumDeclare[] GetEnumDeclares(ISheet sheet)
        {
            var ret = new List<EnumDeclare>();

            for (int rownum = 1; rownum <= sheet.LastRowNum; ++rownum)
            {
                var item = GetEnumDeclare(sheet, rownum);
                if (item != null)
                {
                    ret.Add(item);
                }
            }
            return ret.ToArray();
        }

        EnumDeclare GetEnumDeclare(ISheet sheet, int rownum)
        {
            var ret = new EnumDeclare();
            var row = sheet.GetRow(rownum);
            var part_cell = row.GetCell((int)E_ENUM_ROW.PART);
            if (part_cell == null)
                return null;

            var attribute_cell = row.GetCell((int)E_ENUM_ROW.ATTRIBUTE);
            var name_cell = row.GetCell((int)E_ENUM_ROW.NAME);
            if (string.IsNullOrEmpty(name_cell.StringCellValue))
                return null;

            var part = (E_PART)Enum.Parse(typeof(E_PART), part_cell.StringCellValue);

            string attribute = null;
            if (attribute_cell != null)
                attribute = attribute_cell.StringCellValue;

            List<string> args = new List<string>();

            for (int cellnum = (int)E_ENUM_ROW.NAME + 1; cellnum < GetColumnMax(sheet); ++cellnum)
            {
                var cell = row.GetCell(cellnum);
                if (cell == null)
                {
                    break;
                }
                if (string.IsNullOrEmpty(cell.StringCellValue))
                    break;
                args.Add(cell.StringCellValue);
            }

            return EnumDeclare.GenEnumDeclare(part, attribute, name_cell.StringCellValue, args.ToArray());
        }

        #endregion enum

        #region class

        ClassDeclare GetClaassDeclare(ISheet sheet)
        {
            var ret = new ClassDeclare { Name = sheet.SheetName };
            var lst = new List<ClassMember>();
            var max = GetColumnMax(sheet);
            for (int i = 0; i < max; ++i)
            {
                var item = GetMemberDeclare(sheet, i);
                if (item != null)
                {
                    lst.Add(item);
                }
            }
            ret.ClassMembers = lst.ToArray();
            return ret;
        }

        ClassMember GetMemberDeclare(ISheet sheet, int index)
        {
            var part_row = sheet.GetRow((int)E_DATA_ROW.PART);
            var part_cell = part_row.GetCell(index);
            if (part_cell == null)
            {
                return null;
            }

            var attribute_row = sheet.GetRow((int)E_DATA_ROW.ATTRIBUTE);

            var attribute_cell = attribute_row == null ? null : attribute_row.GetCell(index);
            var type_row = sheet.GetRow((int)E_DATA_ROW.TYPE);
            var type_cell = type_row.GetCell(index);
            var column_row = sheet.GetRow((int)E_DATA_ROW.NAME);
            var column_cell = column_row.GetCell(index);

            string name = column_cell.StringOrNull();
            string type = type_cell.StringOrNull();

            string attribute = string.Empty;
            if (attribute_cell != null)
            {
                attribute = attribute_cell.StringOrNull();
            }

            var part = (E_PART)Enum.Parse(typeof(E_PART), part_cell.StringCellValue);

            return ClassMember.GenClassMember(part, attribute, type, name);
        }

        #endregion class

        #region const

        ConstDeclare GetConstDeclareFromSingleSheet(ISheet sheet)
        {
            var ret = new ConstDeclare();
            var args = new List<ConstMember>();
            for (int rownum = 1; rownum <= sheet.LastRowNum; ++rownum)
            {
                var row = sheet.GetRow(rownum);
                if (row == null)
                {
                    continue;
                }
                var partvalue = row.GetCell((int)E_SINGLE_CONST_COLUMN.PART).StringOrNull();
                if (string.IsNullOrEmpty(partvalue))
                {
                    continue;
                }

                var part = (E_PART)Enum.Parse(typeof(E_PART), partvalue);
                var typevalue = row.GetCell((int)E_SINGLE_CONST_COLUMN.TYPE).StringOrNull();
                var attribute = row.GetCell((int)E_SINGLE_CONST_COLUMN.ATTRIBUTE).StringOrNull();
                var name = row.GetCell((int)E_SINGLE_CONST_COLUMN.NAME).StringCellValue;
                var value = row.GetCell((int)E_SINGLE_CONST_COLUMN.VALUE).StringOrNull();
                var desc = row.GetCell((int)E_SINGLE_CONST_COLUMN.DESC).StringOrNull();

                string[] Attributes = null;
                if (!string.IsNullOrEmpty(attribute))
                {
                    Attributes = attribute.Split(';').ToArray();
                }
                args.Add(new ConstMember { Part = part, Type = typevalue, Name = name, Value = value, Desc = desc, Attributes = Attributes });
            }
            ret.Name = sheet.SheetName.Remove(0, COST_SHEET_PREFIX.Length);
            ret.ConstMembers = args.ToArray();
            return ret;
        }

        #endregion const

        void WriteToFile(string path, string contents)
        {
            using (var sw = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                sw.Write(contents);
            }
        }
    }

    public class BasePart
    {
        public E_PART Part = E_PART.Common;

        public bool IsClient { get { return Part.HasFlag(E_PART.Client); } }
        public bool IsClientOnly { get { return Part == E_PART.Client; } }
        public bool IsServer { get { return Part.HasFlag(E_PART.Server); } }
        public bool IsServerOnly { get { return Part == E_PART.Server; } }
    }

    [Flags]
    public enum E_PART
    {
        Client = 1 << 0,
        Server = 1 << 1,
        Common = Client | Server,
    }

    #region ConstSheet

    public class ConstDeclare
    {
        public string Name;
        public ConstMember[] ConstMembers;
    }

    public class ConstMember : BasePart
    {
        public string[] Attributes;
        public string Type;
        public string Name;
        public string Value;
        public string Desc;
        public bool IsString { get { return Type == "string"; } }
    }

    #endregion ConstSheet

    #region EnumSheet

    public class EnumSheet
    {
        public EnumDeclare[] EnumDeclares;
    }

    public class EnumDeclare : BasePart
    {
        public string[] Attributes;
        public string Name;
        public EnumMember[] args;

        public static EnumDeclare GenEnumDeclare(E_PART part, string attribute, string name, string[] args)
        {
            string[] Attributes = null;
            if (!string.IsNullOrEmpty(attribute))
            {
                Attributes = attribute.Split(';').ToArray();
            }

            return new EnumDeclare { Part = part, Attributes = Attributes, Name = name, args = EnumMember.GenEnumMembers(args) };
        }
    }

    public class EnumMember : BasePart
    {
        public string[] Attributes;
        public string Name;
        public string Value;
        public string Desc;

        public static EnumMember[] GenEnumMembers(string[] args)
        {
            var ret = new List<EnumMember>();
            foreach (var arg in args)
            {
                ret.Add(new EnumMember { Name = arg });
            }
            return ret.ToArray();
        }
    }

    #endregion EnumSheet

    #region ClassSheet

    public class ClassDeclare
    {
        public string Name;
        public ClassMember[] ClassMembers;
    }

    public class ClassMember : BasePart
    {
        public string[] Attributes;
        public string Type;
        public string Name;

        public static ClassMember GenClassMember(E_PART part, string attribute, string type, string name)
        {
            string[] Attributes = null;
            if (!string.IsNullOrEmpty(attribute))
            {
                Attributes = attribute.Split(';').ToArray();
            }

            return new ClassMember { Part = part, Attributes = Attributes, Type = type, Name = name };
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
            var selfValue = Convert.ToInt32(self);
            var flagValue = Convert.ToInt32(flag);

            return (selfValue & flagValue) == flagValue;
        }
    }
}
