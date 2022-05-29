using DotLiquid;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace NF.Tools.DataFlow.Internal
{
    public class EnumSheet : Drop
    {
        public class ContentCell_Enum : Drop
        {
            public string Attr { get; init; }
            public string Name { get; init; }
            public string Value { get; init; }
            public string Desc { get; init; }
            public ContentCell_Enum(in string Attr, in string Name, in string Value, in string Desc)
            {
                this.Attr = Attr;
                this.Name = Name;
                this.Value = Value;
                this.Desc = Desc;
            }
        }
        public SheetInfo SheetInfo { get; init; }
        public ContentCell_Enum[] Contents { get; init; }
        public Dictionary<ReservedCell.E_RESERVED, ReservedCell> reserved_dic { get; init; }

        // ==============================================================
        private static Dictionary<string, ReservedCell.E_RESERVED> CONTENT_DIC = new Dictionary<string, ReservedCell.E_RESERVED>()
        {
            {nameof(ReservedCell.E_RESERVED.ATTR), ReservedCell.E_RESERVED.ATTR },
            {nameof(ReservedCell.E_RESERVED.NAME), ReservedCell.E_RESERVED.NAME },
            {nameof(ReservedCell.E_RESERVED.VALUE), ReservedCell.E_RESERVED.VALUE },
            {nameof(ReservedCell.E_RESERVED.DESC), ReservedCell.E_RESERVED.DESC },
        };

        public static EnumSheet GetOrNull(in SheetInfo sheetInfo)
        {
            int contentsStartRowIndex = -1;
            Dictionary<ReservedCell.E_RESERVED, ReservedCell> reservedDic = new();

            for (int y = 0; y < 4; ++y)
            {
                IRow row = sheetInfo.sheet.GetRow(y);

                ICell cell = row.GetCell(0);
                string cellValue = cell.StringOrNull();
                if (cellValue == null)
                {
                    continue;
                }

                if (cellValue == nameof(ReservedCell.E_RESERVED.TABLE))
                {
                    ICell ccell = row.GetCell(1);
                    string ccellValue = ccell.StringOrNull();
                    if (ccellValue == null)
                    {
                        return null;
                    }
                    reservedDic.Add(ReservedCell.E_RESERVED.TABLE,
                        new ReservedCell
                        {
                            Reserved = ReservedCell.E_RESERVED.TABLE,
                            Position = new int2(0, y),
                            CellString = ccellValue
                        });
                    continue;
                }

                if (cellValue == nameof(ReservedCell.E_RESERVED.TATTR))
                {
                    ICell ccell = row.GetCell(1);
                    string ccellValue = ccell.StringOrNull();
                    if (ccellValue == null)
                    {
                        return null;
                    }
                    reservedDic.Add(ReservedCell.E_RESERVED.TATTR,
                        new ReservedCell
                        {
                            Reserved = ReservedCell.E_RESERVED.TATTR,
                            Position = new int2(0, y),
                            CellString = ccellValue
                        });
                    continue;
                }

                if (cellValue == nameof(ReservedCell.E_RESERVED.TDESC))
                {
                    ICell ccell = row.GetCell(1);
                    string ccellValue = ccell.StringOrNull();
                    if (ccellValue == null)
                    {
                        return null;
                    }
                    reservedDic.Add(ReservedCell.E_RESERVED.TDESC,
                        new ReservedCell
                        {
                            Reserved = ReservedCell.E_RESERVED.TDESC,
                            Position = new int2(0, y),
                            CellString = ccellValue
                        });
                    continue;
                }
                if (CONTENT_DIC.ContainsKey(cellValue))
                {
                    for (int x = 0; x < sheetInfo.ColumnMax; ++x)
                    {
                        ICell ccell = row.GetCell(x);
                        string ccellValue = ccell.StringOrNull();
                        if (ccellValue == null)
                        {
                            return null;
                        }
                        if (ccellValue.StartsWith('_'))
                        {
                            continue;
                        }
                        if (!CONTENT_DIC.TryGetValue(ccellValue, out ReservedCell.E_RESERVED e))
                        {
                            return null;
                        }
                        reservedDic.Add(e,
                            new ReservedCell
                            {
                                Reserved = e,
                                Position = new int2(x, y)
                            });
                    }
                    contentsStartRowIndex = y + 1;
                    break;
                }
            }

            if (contentsStartRowIndex == -1)
            {
                return null;
            }

            // fill contents
            ContentCell_Enum[] cs = new ContentCell_Enum[sheetInfo.RowMax - contentsStartRowIndex];
            for (int y = contentsStartRowIndex; y < sheetInfo.RowMax; ++y)
            {
                IRow row = sheetInfo.sheet.GetRow(y);
                string attr = null;
                string name = null;
                string val = null;
                string desc = null;
                foreach (ReservedCell v in reservedDic.Values)
                {
                    switch (v.Reserved)
                    {
                        case ReservedCell.E_RESERVED.ATTR:
                            {
                                ICell cell = row.GetCell(v.Position.x);
                                attr = cell.StringOrNull();
                            }
                            break;
                        case ReservedCell.E_RESERVED.NAME:
                            {
                                ICell cell = row.GetCell(v.Position.x);
                                name = cell.StringOrNull();
                            }
                            break;
                        case ReservedCell.E_RESERVED.VALUE:
                            {
                                ICell cell = row.GetCell(v.Position.x);
                                val = cell.StringOrNull();
                            }
                            break;
                        case ReservedCell.E_RESERVED.DESC:
                            {
                                ICell cell = row.GetCell(v.Position.x);
                                desc = cell.StringOrNull();
                            }
                            break;
                        default:
                            break;
                    }
                }
                cs[y - contentsStartRowIndex] = new ContentCell_Enum(attr, name, val, desc);
            }

            EnumSheet ret = new EnumSheet
            {
                SheetInfo = sheetInfo,
                Contents = cs,
                reserved_dic = reservedDic,
            };
            return ret;
        }
    }
}