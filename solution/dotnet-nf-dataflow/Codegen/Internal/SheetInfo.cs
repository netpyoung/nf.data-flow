using DotLiquid;
using NPOI.SS.UserModel;
using System.Text.RegularExpressions;

namespace NF.Tools.DataFlow.CodeGen.Internal
{
    public class SheetInfo : Drop
    {
        public enum E_TYPE
        {
            INVALID,
            CONST,
            ENUM,
            CLASS
        }

        public ISheet sheet { get; init; }
        public string sheet_name { get; init; }
        public string sheet_namespace { get; init; }
        public E_TYPE type { get; init; }
        public int row_max { get; init; }
        public int column_max { get; init; }

        public SheetInfo(in ISheet sheet, in string refinedSheetName, in string sheetNamespace, in E_TYPE type, in int rowMax, in int columnMax)
        {
            this.sheet = sheet;
            sheet_name = refinedSheetName;
            sheet_namespace = sheetNamespace;
            this.type = type;
            row_max = rowMax;
            column_max = columnMax;
        }
        // ================================================

        static readonly Regex rx = new Regex(@"^(@|!)?(?<name>\w+)", RegexOptions.Compiled);
        static bool IsConstSheetName(in string sheetName) => sheetName.StartsWith("!");
        static bool IsEnumSheetName(in string sheetName) => sheetName.StartsWith("@");
        static bool IsIgnoreSheetName(in string sheetName) => sheetName.StartsWith("#");

        public static bool TryGetSheetInfo(in ISheet sheet, in CodeGeneratorOptions opt, out SheetInfo outInfo)
        {
            string sheetName = sheet.SheetName;
            SheetInfo.E_TYPE sheetInfoType = GetSheetInfoType(sheetName);
            if (sheetInfoType == SheetInfo.E_TYPE.INVALID)
            {
                outInfo = default(SheetInfo);
                return false;
            }
            int columMax = GetColumnCount(sheet);
            int rowMax = GetRowCount(sheet);
            string refinedSheetName = RefinedSheetNameOrNull(sheetName);
            string sheetNamespace = opt.Namespace;
            outInfo = new SheetInfo(sheet, refinedSheetName, sheetNamespace, sheetInfoType, rowMax, columMax);
            return true;
        }

        internal static string RefinedSheetNameOrNull(in string sheetName)
        {
            MatchCollection matches = rx.Matches(sheetName);
            if (matches.Count != 1)
            {
                return null;
            }
            Match match = matches[0];
            GroupCollection groups = match.Groups;
            string value = groups["name"].Value;
            return value;
        }

        static E_TYPE GetSheetInfoType(in string sheetName)
        {
            if (IsIgnoreSheetName(sheetName))
            {
                return E_TYPE.INVALID;
            }
            if (IsConstSheetName(sheetName))
            {
                return E_TYPE.CONST;
            }
            if (IsEnumSheetName(sheetName))
            {
                return E_TYPE.ENUM;
            }
            return E_TYPE.CLASS;
        }

        internal static int GetColumnCount(in ISheet sheet)
        {
            // 첫row에서 마지막 column을 찾는다
            IRow row = sheet.GetRow(0);
            if (row == null)
            {
                return -1;
            }

            for (int cellIndex = 0; cellIndex <= row.LastCellNum; ++cellIndex)
            {
                ICell cell = row.GetCell(cellIndex);
                if (cell == null)
                {
                    continue;
                }

                if (cell.CellType == CellType.String)
                {
                    string value = cell.StringCellValue;
                    if (value == "&END")
                    {
                        return cellIndex;
                    }
                }
            }
            return -1;
        }

        internal static int GetRowCount(in ISheet sheet)
        {
            // row를 내려가며 마지막 row를 찾는다.
            for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; ++rowIndex)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row == null)
                {
                    return -1;
                }
                ICell cell = row.GetCell(0);
                if (cell == null)
                {
                    continue;
                }
                if (cell.CellType == CellType.String)
                {
                    string value = cell.StringCellValue;
                    if (value == "&END")
                    {
                        return rowIndex;
                    }
                }
            }
            return -1;
        }
    }
}