using DotLiquid;
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace NF.Tools.DataFlow.Internal
{
    public class ConstSheet : Drop
    {
        public class ContentCell_Const : Drop
        {
            public E_PART Part { get; init; }
            public string Attr { get; init; }
            public string Type { get; init; }
            public bool IsRawString { get; init; }
            public string Name { get; init; }
            public string Value { get; init; }
            public string Desc { get; init; }

            public ContentCell_Const(in E_PART Part, in string Attr, in string originType, in string Name, in string Value, in string Desc)
            {
                this.Part = Part;
                this.Attr = Attr;
                if (originType == "\"string")
                {
                    this.Type = originType.Substring(1);
                    this.IsRawString = true;
                }
                else
                {
                    this.Type = originType;
                    this.IsRawString = false;
                }
                this.Name = Name;
                this.Value = Value;
                this.Desc = Desc;
            }
        }
        public SheetInfo SheetInfo { get; init; }
        public ContentCell_Const[] Contents { get; init; }
        public Dictionary<ReservedCell.E_RESERVED, ReservedCell> reserved_dic { get; init; }

        // ==============================================================

        private static HashSet<string> CONTENT_HashSet = new HashSet<string>
        {
            nameof(ReservedCell.E_RESERVED.PART),
            nameof(ReservedCell.E_RESERVED.TYPE),
            nameof(ReservedCell.E_RESERVED.ATTR),
            nameof(ReservedCell.E_RESERVED.NAME),
            nameof(ReservedCell.E_RESERVED.VALUE),
            nameof(ReservedCell.E_RESERVED.DESC)
        };
        private static Dictionary<string, ReservedCell.E_RESERVED> CONTENT_DIC = new Dictionary<string, ReservedCell.E_RESERVED>()
        {
            {nameof(ReservedCell.E_RESERVED.PART), ReservedCell.E_RESERVED.PART },
            {nameof(ReservedCell.E_RESERVED.TYPE), ReservedCell.E_RESERVED.TYPE },
            {nameof(ReservedCell.E_RESERVED.ATTR), ReservedCell.E_RESERVED.ATTR },
            {nameof(ReservedCell.E_RESERVED.NAME), ReservedCell.E_RESERVED.NAME },
            {nameof(ReservedCell.E_RESERVED.VALUE), ReservedCell.E_RESERVED.VALUE },
            {nameof(ReservedCell.E_RESERVED.DESC), ReservedCell.E_RESERVED.DESC },
        };

        public static ConstSheet GetOrNull(in SheetInfo sheetInfo)
        {
            int contentsStartRowIndex = -1;
            Dictionary<ReservedCell.E_RESERVED, ReservedCell> reservedDic = new();
            Dictionary<ReservedCell.E_RESERVED, ReservedCell> reservedDic2 = new();

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
                        if (ccellValue == nameof(ReservedCell.E_RESERVED.PART))
                        {
                            reservedDic2.Add(ReservedCell.E_RESERVED.PART,
                                new ReservedCell
                                {
                                    Reserved = ReservedCell.E_RESERVED.PART,
                                    Position = new int2(x, 0),
                                    CellString = null
                                });
                            continue;
                        }
                        if (ccellValue == nameof(ReservedCell.E_RESERVED.TYPE))
                        {
                            reservedDic2.Add(ReservedCell.E_RESERVED.TYPE,
                                new ReservedCell
                                {
                                    Reserved = ReservedCell.E_RESERVED.TYPE,
                                    Position = new int2(x, 0),
                                    CellString = null
                                });
                            continue;
                        }
                        if (ccellValue == nameof(ReservedCell.E_RESERVED.ATTR))
                        {
                            reservedDic2.Add(ReservedCell.E_RESERVED.ATTR,
                                new ReservedCell
                                {
                                    Reserved = ReservedCell.E_RESERVED.ATTR,
                                    Position = new int2(x, 0),
                                    CellString = null
                                });
                            continue;
                        }
                        if (ccellValue == nameof(ReservedCell.E_RESERVED.NAME))
                        {
                            reservedDic2.Add(ReservedCell.E_RESERVED.NAME,
                                new ReservedCell
                                {
                                    Reserved = ReservedCell.E_RESERVED.NAME,
                                    Position = new int2(x, 0),
                                    CellString = null
                                });
                            continue;
                        }
                        if (ccellValue == nameof(ReservedCell.E_RESERVED.VALUE))
                        {
                            reservedDic2.Add(ReservedCell.E_RESERVED.VALUE,
                                new ReservedCell
                                {
                                    Reserved = ReservedCell.E_RESERVED.VALUE,
                                    Position = new int2(x, 0),
                                    CellString = null
                                });
                            continue;
                        }
                        if (ccellValue == nameof(ReservedCell.E_RESERVED.DESC))
                        {
                            reservedDic2.Add(ReservedCell.E_RESERVED.DESC,
                                new ReservedCell
                                {
                                    Reserved = ReservedCell.E_RESERVED.DESC,
                                    Position = new int2(x, 0),
                                    CellString = null
                                });
                            continue;
                        }
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
            ContentCell_Const[] cs = new ContentCell_Const[sheetInfo.RowMax - contentsStartRowIndex];
            for (int y = contentsStartRowIndex; y < sheetInfo.RowMax; ++y)
            {
                IRow row = sheetInfo.sheet.GetRow(y);
                E_PART part = E_PART.Common;
                string attr = null;
                string type_ = null;
                string name = null;
                string val = null;
                string desc = null;
                foreach (ReservedCell v in reservedDic2.Values)
                {
                    switch (v.Reserved)
                    {
                        case ReservedCell.E_RESERVED.PART:
                            {
                                ICell cell = row.GetCell(v.Position.x);
                                part = cell.StringOrNull() switch
                                {
                                    "Client" => E_PART.Client,
                                    "Server" => E_PART.Server,
                                    _ => E_PART.Common,
                                };
                            }
                            break;
                        case ReservedCell.E_RESERVED.TYPE:
                            {
                                ICell cell = row.GetCell(v.Position.x);
                                type_ = cell.StringOrNull();
                            }
                            break;
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
                cs[y - contentsStartRowIndex] = new ContentCell_Const(part, attr, type_, name, val, desc);
            }

            ConstSheet ret = new ConstSheet
            {
                SheetInfo = sheetInfo,
                Contents = cs,
                reserved_dic = reservedDic,
            };
            return ret;
        }
    }
}