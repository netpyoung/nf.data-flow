using NF.Tools.DataFlow.Internal;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NF.Tools.DataFlow
{
    public class ExcelLoader
    {
        Dictionary<string, (WorkbookInfo, ClassSheet)> _dic = new Dictionary<string, (WorkbookInfo, ClassSheet)>();

        public ExcelLoader(in WorkbookInfo[] workbookInfos)
        {
            foreach (ref readonly WorkbookInfo wri in workbookInfos.AsSpan())
            {
                foreach (ClassSheet classSheet in wri.ClassSheets)
                {
                    _dic[classSheet.SheetInfo.SheetName] = (wri, classSheet);
                }
            }
        }

        public List<object> GetDataListOrNull(in Type type, string sheetName)
        {
            if (!_dic.TryGetValue(sheetName, out (WorkbookInfo, ClassSheet) wf))
            {
                return null;
            }
            ref readonly WorkbookInfo info = ref wf.Item1;
            IWorkbook excel = info.Excel;
            IFormulaEvaluator evaluator = excel.GetCreationHelper().CreateFormulaEvaluator();

            ClassSheet classSheet = info.ClassSheets.FirstOrDefault(x => x.SheetInfo.SheetName == sheetName);
            if (classSheet == null)
            {
                return null;
            }
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            MemberInfo[] members = type.GetFields(bindingFlags)
                .Cast<MemberInfo>()
                .Concat(type.GetProperties(bindingFlags))
                .ToArray();
            Dictionary<string, MemberInfo> memberDic = members.ToDictionary(x => x.Name, x => x);

            SheetInfo sheetInfo = classSheet.SheetInfo;
            int nameRowIndex = -1;
            foreach (KeyValuePair<ReservedCell.E_RESERVED, ReservedCell> x in classSheet.reserved_dic2)
            {
                if (x.Value.Reserved == ReservedCell.E_RESERVED.NAME)
                {
                    nameRowIndex = x.Value.Position.y;
                }
            }
            if (nameRowIndex == -1)
            {
                return null;
            }

            Dictionary<string, int> field_indexed_dic = new Dictionary<string, int>();
            IRow nameRow = sheetInfo.sheet.GetRow(nameRowIndex);
            for (int x = 0; x < sheetInfo.ColumnMax; ++x)
            {
                ICell cell = nameRow.GetCell(x);
                cell.SetCellType(CellType.String);
                string val = cell.StringCellValue;
                if (memberDic.ContainsKey(val))
                {
                    field_indexed_dic.Add(val, cell.ColumnIndex);
                }
            }

            List<object> ret = new List<object>(sheetInfo.RowMax - classSheet.ContentsStartRowIndex);
            for (int y = classSheet.ContentsStartRowIndex; y < sheetInfo.RowMax; ++y)
            {
                IRow row = sheetInfo.sheet.GetRow(y);

                object item = Activator.CreateInstance(type);
                foreach ((string cachedFieldName, int cachedColumnIdx) in field_indexed_dic)
                {
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
                                object value = this.GetValue(cell, ((FieldInfo)member).FieldType, evaluator);
                                if (value == null)
                                {
                                    continue;
                                }
                                ((FieldInfo)member).SetValue(item, value);
                            }
                            break;
                        case MemberTypes.Property:
                            {
                                object value = this.GetValue(cell, ((PropertyInfo)member).PropertyType, evaluator);
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
        #region dirty methods

        private object GetValue(in ICell cell, in Type type, in IFormulaEvaluator evaluator)
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

            if (type == typeof(DateTime))
            {
                return DateTime.Parse(this.GetStringVal(cell, evaluator));
            }

            if (type == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Parse(this.GetStringVal(cell, evaluator));
            }

            if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(this.GetStringVal(cell, evaluator));
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

        private void DebugLogICell(in ICell cell, in Type type, in Exception e)
        {
            Console.Error.WriteLine(e);
            Console.Error.WriteLine($"{cell.Sheet.SheetName}: {cell.RowIndex + 1}/{cell.ColumnIndex + 1} | {cell}({type})");
        }

        private string GetStringVal(in ICell cell, in IFormulaEvaluator evaluator)
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
