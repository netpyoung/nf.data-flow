using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ToolDataClassGenerator
{
    class ExcelLoader
    {
        private readonly byte[] END_COLOR = { 255, 255, 0 };

        readonly IWorkbook _reader;
        readonly IFormulaEvaluator _evaluator;

        public ExcelLoader(string excel_fpath)
        {
            this._reader = GetExcelDataReader(excel_fpath);
            this._evaluator = _reader.GetCreationHelper().CreateFormulaEvaluator();
        }

        public List<object> GetDataList(Type type, string sheet_name)
        {
            List<object> ret = new List<object>();

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            var members = type.GetFields(bindingFlags).Cast<MemberInfo>().Concat(type.GetProperties(bindingFlags)).ToArray();
            var member_dic = new Dictionary<string, MemberInfo>();
            foreach (var field in members)
            {
                member_dic.Add(field.Name, field);
            }

            ISheet sheet = this._reader.GetSheet(sheet_name);

            var field_indexed_dic = new Dictionary<string, int>();
            foreach (ICell cell in sheet.GetRow(3))
            {
                cell.SetCellType(CellType.String);
                var val = cell.StringCellValue;
                if (member_dic.ContainsKey(val))
                {
                    field_indexed_dic.Add(val, cell.ColumnIndex);
                }
            }

            for (int i = 4; i < sheet.LastRowNum; ++i)
            {
                var row = sheet.GetRow(i);
                var cell0 = row.GetCell(0);
                if (cell0 == null)
                    break;

                if (cell0.CellStyle != null
                    && cell0.CellStyle.FillForegroundColorColor != null
                    && Enumerable.SequenceEqual(cell0.CellStyle.FillForegroundColorColor.RGB, END_COLOR))
                {
                    break;
                }

                var item = Activator.CreateInstance(type);
                foreach (var kb in field_indexed_dic)
                {
                    var cached_field_name = kb.Key;
                    var cached_column_idx = kb.Value;

                    ICell cell = row.GetCell(cached_column_idx);
                    MemberInfo member = member_dic[cached_field_name];

                    if (cell == null)
                        continue;

                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            {
                                var value = GetValue(cell, ((FieldInfo)member).FieldType, this._evaluator);
                                if (value == null)
                                {
                                    continue;
                                }
                            ((FieldInfo)member).SetValue(item, value);
                            }

                            break;

                        case MemberTypes.Property:
                            {
                                var value = GetValue(cell, ((PropertyInfo)member).PropertyType, this._evaluator);
                                if (value == null)
                                {
                                    continue;
                                }
                            ((PropertyInfo)member).SetValue(item, value, null);
                            }

                            break;
                    }
                }
                ret.Add(item);
            }
            return ret;
        }

        public List<T> GetDataList<T>(string sheet_name)
        {
            List<T> ret = new List<T>();
            Type type = typeof(T);

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            var members = type.GetFields(bindingFlags).Cast<MemberInfo>().Concat(type.GetProperties(bindingFlags)).ToArray();
            var member_dic = new Dictionary<string, System.Reflection.MemberInfo>();
            foreach (var field in members)
            {
                member_dic.Add(field.Name, field);
            }

            ISheet sheet = this._reader.GetSheet(sheet_name);

            var field_indexed_dic = new Dictionary<string, int>();
            foreach (ICell cell in sheet.GetRow(0))
            {
                cell.SetCellType(CellType.String);
                var val = cell.StringCellValue;
                if (member_dic.ContainsKey(val))
                {
                    field_indexed_dic.Add(val, cell.ColumnIndex);
                }
            }

            for (int i = 1; i < sheet.LastRowNum; ++i)
            {
                var row = sheet.GetRow(i);
                var cell0 = row.GetCell(0);
                if (cell0 == null)
                    break;

                if (cell0.CellStyle != null
                    && cell0.CellStyle.FillForegroundColorColor != null
                    && Enumerable.SequenceEqual(cell0.CellStyle.FillForegroundColorColor.RGB, END_COLOR))
                {
                    break;
                }

                var item = (T)Activator.CreateInstance(type);
                foreach (var kb in field_indexed_dic)
                {
                    var cached_field_name = kb.Key;
                    var cached_column_idx = kb.Value;

                    ICell cell = row.GetCell(cached_column_idx);
                    MemberInfo member = member_dic[cached_field_name];

                    if (cell == null)
                        continue;

                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            {
                                var value = GetValue(cell, ((FieldInfo)member).FieldType, this._evaluator);
                                if (value == null)
                                {
                                    continue;
                                }
                            ((FieldInfo)member).SetValue(item, value);
                            }

                            break;

                        case MemberTypes.Property:
                            {
                                var value = GetValue(cell, ((PropertyInfo)member).PropertyType, this._evaluator);
                                if (value == null)
                                {
                                    continue;
                                }
                            ((PropertyInfo)member).SetValue(item, value, null);
                            }

                            break;
                    }
                }
                ret.Add(item);
            }
            return ret;
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

        #region dirty methods

        object GetValue(ICell cell, Type type, IFormulaEvaluator evaluator)
        {
            if (cell == null)
                return null;
            if (cell.CellType == CellType.Blank)
                return null;

            if (type == typeof(string))
            {
                if (cell.CellType == CellType.Numeric)
                {
                    var cell_val = cell.NumericCellValue;
                    var converted_val = Convert.ToInt32(cell.NumericCellValue);

                    if (converted_val != cell_val)
                    {
                        return cell_val.ToString();
                    }
                    else
                    {
                        return converted_val.ToString();
                    }
                }
                else
                {
                    cell.SetCellType(CellType.String);
                    if (string.IsNullOrEmpty(cell.StringCellValue))
                        return "";
                    return cell.StringCellValue;
                }
            }

            if (type == typeof(System.Single))
            {
                if (cell.CellType == CellType.Numeric)
                {
                    return System.Convert.ToSingle(cell.NumericCellValue);
                }
                try
                {
                    return System.Convert.ToSingle(GetStringVal(cell, evaluator));
                }
                catch (System.Exception e)
                {
                    DebugLogICell(cell, type, e);
                    return 0;
                }
            }

            if (type == typeof(int))
            {
                if (cell.CellType == CellType.Numeric)
                {
                    return System.Convert.ToInt32(cell.NumericCellValue);
                }
                try
                {
                    return System.Convert.ToInt32(GetStringVal(cell, evaluator));
                }
                catch (System.Exception e)
                {
                    DebugLogICell(cell, type, e);
                    return 0;
                }
            }

            if (type == typeof(double))
                return System.Convert.ToDouble(GetStringVal(cell, evaluator));

            if (type == typeof(long))
                return System.Convert.ToDouble(GetStringVal(cell, evaluator));

            if (type == typeof(bool))
                return System.Convert.ToBoolean(GetStringVal(cell, evaluator));
            if (type.IsEnum)
            {
                //return 5;
                //return System.Convert.ToInt32(System.Convert.ToDouble(GetStringVal(cell, evaluator)));
                cell.SetCellType(CellType.String);
                return Convert.ToInt32(Enum.Parse(type, cell.StringCellValue));
            }

            return null;
        }

        void DebugLogICell(ICell cell, Type type, System.Exception e)
        {
            var msg = string.Format("{0}: {1}/{2} | {3}({4})", cell.Sheet.SheetName, cell.RowIndex + 1, cell.ColumnIndex + 1, cell, type);
            Console.Error.WriteLine(msg);
            Console.Error.WriteLine(e);
        }

        string GetStringVal(ICell cell, IFormulaEvaluator evaluator)
        {
            switch (cell.CellType)
            {
                case CellType.Formula:
                    switch (cell.CachedFormulaResultType)
                    {
                        case CellType.Numeric:
                            return cell.NumericCellValue.ToString();

                        case CellType.String:
                            return cell.StringCellValue;

                        default:
                            return evaluator.Evaluate(cell).FormatAsString();
                    }

                case CellType.String:
                    return cell.StringCellValue;

                default:
                    cell.SetCellType(CellType.String);
                    return cell.StringCellValue;
            }
        }

        #endregion dirty methods
    }
}