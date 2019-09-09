using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace NF.Tool.DataFlow
{
    internal class ExcelLoader
    {
        private readonly IFormulaEvaluator _evaluator;

        private readonly IWorkbook _reader;
        private readonly byte[] END_COLOR = {255, 255, 0};

        public ExcelLoader(string excel_fpath)
        {
            this._reader = this.GetExcelDataReader(excel_fpath);
            this._evaluator = this._reader.GetCreationHelper().CreateFormulaEvaluator();
        }

        public List<object> GetDataList(Type type, string sheetName)
        {
            List<object> ret = new List<object>();

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            MemberInfo[] members = type.GetFields(bindingFlags).Cast<MemberInfo>()
                .Concat(type.GetProperties(bindingFlags)).ToArray();
            Dictionary<string, MemberInfo> memberDic = new Dictionary<string, MemberInfo>();
            foreach (MemberInfo field in members)
            {
                memberDic.Add(field.Name, field);
            }

            ISheet sheet = this._reader.GetSheet(sheetName);
            if (sheet == null)
            {
                return ret;
            }

            Dictionary<string, int> field_indexed_dic = new Dictionary<string, int>();
            foreach (ICell cell in sheet.GetRow(3))
            {
                if (cell.CellStyle != null
                    && cell.CellStyle.FillForegroundColorColor != null
                    && cell.CellStyle.FillForegroundColorColor.RGB.SequenceEqual(this.END_COLOR))
                {
                    break;
                }
                cell.SetCellType(CellType.String);
                string val = cell.StringCellValue;
                if (memberDic.ContainsKey(val))
                {
                    field_indexed_dic.Add(val, cell.ColumnIndex);
                }
            }

            for (int i = 4; i < sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);
                ICell cell0 = row.GetCell(0);
                if (cell0 == null)
                {
                    break;
                }

                if (cell0.CellStyle != null
                    && cell0.CellStyle.FillForegroundColorColor != null
                    && cell0.CellStyle.FillForegroundColorColor.RGB.SequenceEqual(this.END_COLOR))
                {
                    break;
                }

                object item = Activator.CreateInstance(type);
                foreach (KeyValuePair<string, int> kb in field_indexed_dic)
                {
                    string cachedFieldName = kb.Key;
                    int cachedColumnIdx = kb.Value;

                    ICell cell = row.GetCell(cachedColumnIdx);
                    MemberInfo member = memberDic[cachedFieldName];

                    if (cell == null)
                    {
                        continue;
                    }

                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                        {
                            object value = this.GetValue(cell, ((FieldInfo) member).FieldType, this._evaluator);
                            if (value == null)
                            {
                                continue;
                            }

                            ((FieldInfo) member).SetValue(item, value);
                        }

                            break;

                        case MemberTypes.Property:
                        {
                            object value = this.GetValue(cell, ((PropertyInfo) member).PropertyType, this._evaluator);
                            if (value == null)
                            {
                                continue;
                            }

                            ((PropertyInfo) member).SetValue(item, value, null);
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

            MemberInfo[] members = type.GetFields(bindingFlags).Cast<MemberInfo>()
                .Concat(type.GetProperties(bindingFlags)).ToArray();
            Dictionary<string, MemberInfo> memberDic = new Dictionary<string, MemberInfo>();
            foreach (MemberInfo field in members)
            {
                memberDic.Add(field.Name, field);
            }

            ISheet sheet = this._reader.GetSheet(sheet_name);

            Dictionary<string, int> fieldIndexedDic = new Dictionary<string, int>();
            foreach (ICell cell in sheet.GetRow(0))
            {
                cell.SetCellType(CellType.String);
                string val = cell.StringCellValue;
                if (memberDic.ContainsKey(val))
                {
                    fieldIndexedDic.Add(val, cell.ColumnIndex);
                }
            }

            for (int i = 1; i < sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);
                ICell cell0 = row.GetCell(0);
                if (cell0 == null)
                {
                    break;
                }

                if (cell0.CellStyle != null
                    && cell0.CellStyle.FillForegroundColorColor != null
                    && cell0.CellStyle.FillForegroundColorColor.RGB.SequenceEqual(this.END_COLOR))
                {
                    break;
                }

                T item = (T) Activator.CreateInstance(type);
                foreach (KeyValuePair<string, int> kb in fieldIndexedDic)
                {
                    string cached_field_name = kb.Key;
                    int cached_column_idx = kb.Value;

                    ICell cell = row.GetCell(cached_column_idx);
                    MemberInfo member = memberDic[cached_field_name];

                    if (cell == null)
                    {
                        continue;
                    }

                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                        {
                            object value = this.GetValue(cell, ((FieldInfo) member).FieldType, this._evaluator);
                            if (value == null)
                            {
                                continue;
                            }

                            ((FieldInfo) member).SetValue(item, value);
                        }

                            break;

                        case MemberTypes.Property:
                        {
                            object value = this.GetValue(cell, ((PropertyInfo) member).PropertyType, this._evaluator);
                            if (value == null)
                            {
                                continue;
                            }

                            ((PropertyInfo) member).SetValue(item, value, null);
                        }

                            break;
                    }
                }

                ret.Add(item);
            }

            return ret;
        }

        private IWorkbook GetExcelDataReader(string excel_fpath)
        {
            using (FileStream fileStream = File.Open(excel_fpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XSSFWorkbook workbook = new XSSFWorkbook(fileStream);
                // force evaulate! NPOI.XSSF.UserModel.XSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook)
                return workbook;
            }
        }

        #region dirty methods

        private object GetValue(ICell cell, Type type, IFormulaEvaluator evaluator)
        {
            if (cell == null)
            {
                return null;
            }

            if (cell.CellType == CellType.Blank)
            {
                return null;
            }

            if (type == typeof(string))
            {
                if (cell.CellType == CellType.Numeric)
                {
                    double cellVal = cell.NumericCellValue;
                    int convertedVal = Convert.ToInt32(cell.NumericCellValue);

                    if (convertedVal != cellVal)
                    {
                        return cellVal.ToString();
                    }

                    return convertedVal.ToString();
                }

                cell.SetCellType(CellType.String);
                if (string.IsNullOrEmpty(cell.StringCellValue))
                {
                    return "";
                }

                return cell.StringCellValue;
            }

            if (type == typeof(float))
            {
                if (cell.CellType == CellType.Numeric)
                {
                    return Convert.ToSingle(cell.NumericCellValue);
                }

                try
                {
                    return Convert.ToSingle(this.GetStringVal(cell, evaluator));
                }
                catch (Exception e)
                {
                    this.DebugLogICell(cell, type, e);
                    return 0;
                }
            }

            if (type == typeof(int))
            {
                if (cell.CellType == CellType.Numeric)
                {
                    return Convert.ToInt32(cell.NumericCellValue);
                }

                try
                {
                    return Convert.ToInt32(this.GetStringVal(cell, evaluator));
                }
                catch (Exception e)
                {
                    this.DebugLogICell(cell, type, e);
                    return 0;
                }
            }

            if (type == typeof(double))
            {
                return Convert.ToDouble(this.GetStringVal(cell, evaluator));
            }

            if (type == typeof(long))
            {
                return Convert.ToDouble(this.GetStringVal(cell, evaluator));
            }

            if (type == typeof(bool))
            {
                return Convert.ToBoolean(this.GetStringVal(cell, evaluator));
            }

            if (type.IsEnum)
            {
                //return 5;
                //return System.Convert.ToInt32(System.Convert.ToDouble(GetStringVal(cell, evaluator)));
                cell.SetCellType(CellType.String);
                return Convert.ToInt32(Enum.Parse(type, cell.StringCellValue));
            }

            return null;
        }

        private void DebugLogICell(ICell cell, Type type, Exception e)
        {
            Console.Error.WriteLine(e);
            Console.Error.WriteLine($"{cell.Sheet.SheetName}: {cell.RowIndex + 1}/{cell.ColumnIndex + 1} | {cell}({type})");
        }

        private string GetStringVal(ICell cell, IFormulaEvaluator evaluator)
        {
            switch (cell.CellType)
            {
                case CellType.Formula:
                    switch (cell.CachedFormulaResultType)
                    {
                        case CellType.Numeric:
                            return cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);

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
