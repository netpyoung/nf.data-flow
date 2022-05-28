using DotLiquid;
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace NF.Tools.DataFlow.CodeGen.Internal
{
    public class ClassSheet : Drop
    {
        public class ContentCell_Class : Drop
        {
            public E_PART part { get; init; }
            public string attr { get; init; }
            public string type { get; init; }
            public string name { get; init; }
            public string desc { get; init; }
            public ContentCell_Class(in E_PART Part, in string Attr, in string Type, in string Name, in string Desc)
            {
                this.part = Part;
                this.attr = Attr;
                this.type = Type;
                this.name = Name;
                this.desc = Desc;
            }
        }

        public SheetInfo sheet_info { get; init; }
        public string sheet_namespace { get; init; }
        public ContentCell_Class[] contents { get; init; }
        public Dictionary<ReservedCell.E_RESERVED, ReservedCell> reserved_dic { get; init; }
        public Dictionary<ReservedCell.E_RESERVED, ReservedCell> reserved_dic2 { get; init; }

        // ==============================================================
        public static ClassSheet GetOrNull(in SheetInfo sheetInfo)
        {
            int contentsStartRowIndex = -1;
            Dictionary<ReservedCell.E_RESERVED, ReservedCell> reservedDic = new();
            Dictionary<ReservedCell.E_RESERVED, ReservedCell> reservedDic2 = new();

            for (int y = 0; y < sheetInfo.row_max; ++y)
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

                // ====================================
                if (cellValue == nameof(ReservedCell.E_RESERVED.ATTR))
                {
                    reservedDic2.Add(ReservedCell.E_RESERVED.ATTR,
                        new ReservedCell
                        {
                            Reserved = ReservedCell.E_RESERVED.ATTR,
                            Position = new int2(0, y),
                        });
                    continue;
                }
                if (cellValue == nameof(ReservedCell.E_RESERVED.TYPE))
                {
                    reservedDic2.Add(ReservedCell.E_RESERVED.TYPE,
                        new ReservedCell
                        {
                            Reserved = ReservedCell.E_RESERVED.TYPE,
                            Position = new int2(0, y),
                        });
                    continue;
                }
                if (cellValue == nameof(ReservedCell.E_RESERVED.NAME))
                {
                    reservedDic2.Add(ReservedCell.E_RESERVED.NAME,
                        new ReservedCell
                        {
                            Reserved = ReservedCell.E_RESERVED.NAME,
                            Position = new int2(0, y),
                        });
                    continue;
                }
                if (cellValue == nameof(ReservedCell.E_RESERVED.DESC))
                {
                    reservedDic2.Add(ReservedCell.E_RESERVED.DESC,
                        new ReservedCell
                        {
                            Reserved = ReservedCell.E_RESERVED.DESC,
                            Position = new int2(0, y),
                        });
                    continue;
                }
                if (cellValue == nameof(ReservedCell.E_RESERVED.PART))
                {
                    reservedDic2.Add(ReservedCell.E_RESERVED.PART,
                        new ReservedCell
                        {
                            Reserved = ReservedCell.E_RESERVED.PART,
                            Position = new int2(0, y),
                        });
                    continue;
                }
                if (cellValue == nameof(ReservedCell.E_RESERVED.VALUE))
                {
                    contentsStartRowIndex = y;
                    break;
                }
            }

            if (contentsStartRowIndex == -1)
            {
                return null;
            }

            ContentCell_Class[] cs = new ContentCell_Class[sheetInfo.column_max - 1];
            for (int x = 1; x < sheetInfo.column_max; ++x)
            {
                E_PART part = E_PART.Common;
                string attr = null;
                string type = null;
                string name = null;
                string value = null;
                string desc = null;
                foreach (KeyValuePair<ReservedCell.E_RESERVED, ReservedCell> c in reservedDic2)
                {
                    int y = c.Value.Position.y;
                    IRow row = sheetInfo.sheet.GetRow(y);
                    ICell cell = row.GetCell(x);
                    switch (c.Key)
                    {
                        case ReservedCell.E_RESERVED.PART:
                            {
                                part = cell.StringOrNull() switch
                                {
                                    "Client" => E_PART.Client,
                                    "Server" => E_PART.Server,
                                    _ => E_PART.Common,
                                };
                            }
                            break;
                        case ReservedCell.E_RESERVED.ATTR:
                            {
                                attr = cell.StringOrNull();
                            }
                            break;
                        case ReservedCell.E_RESERVED.TYPE:
                            {
                                type = cell.StringOrNull();
                            }
                            break;
                        case ReservedCell.E_RESERVED.NAME:
                            {
                                name = cell.StringOrNull();
                            }
                            break;
                        case ReservedCell.E_RESERVED.VALUE:
                            {
                                value = cell.StringOrNull();
                            }
                            break;
                        case ReservedCell.E_RESERVED.DESC:
                            {
                                desc = cell.StringOrNull();
                            }
                            break;
                        default:
                            break;
                    }
                }
                cs[x - 1] = new ContentCell_Class(part, attr, type, name, desc);
            }
            ClassSheet ret = new ClassSheet
            {
                sheet_info = sheetInfo,
                contents = cs,
                reserved_dic = reservedDic,
                reserved_dic2 = reservedDic2,
            };
            return ret;
        }
    }
}