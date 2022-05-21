using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace NF.Tools.DataFlow.CodeGen.Internal
{
    public readonly record struct WorkbookInfo(IWorkbook Excel, ClassSheet[] ClassSheets, EnumSheet[] EnumSheets, ConstSheet[] ConstSheets)
    {
        public static bool TryGetWorkbookInfo(IWorkbook excel, CodeGeneratorOptions opt, out WorkbookInfo outInfo)
        {
            List<ClassSheet> ClassSheets = new(excel.NumberOfSheets);
            List<EnumSheet> EnumSheets = new(excel.NumberOfSheets);
            List<ConstSheet> ConstSheets = new(excel.NumberOfSheets);

            for (int i = 0; i < excel.NumberOfSheets; ++i)
            {
                ISheet sheet = excel.GetSheetAt(i);
                if (!SheetInfo.TryGetSheetInfo(sheet, opt, out SheetInfo sheetInfo))
                {
                    continue;
                }

                switch (sheetInfo.type)
                {
                    case SheetInfo.E_TYPE.CONST:
                        {
                            ConstSheet x = ConstSheet.GetOrNull(sheetInfo);
                            if (x == null)
                            {
                                outInfo = default(WorkbookInfo);
                                return false;
                            }
                            ConstSheets.Add(x);
                        }
                        break;
                    case SheetInfo.E_TYPE.ENUM:
                        {
                            EnumSheet x = EnumSheet.GetOrNull(sheetInfo);
                            if (x == null)
                            {
                                outInfo = default(WorkbookInfo);
                                return false;
                            }
                            EnumSheets.Add(x);
                        }
                        break;
                    case SheetInfo.E_TYPE.CLASS:
                        {
                            ClassSheet x = ClassSheet.GetOrNull(sheetInfo);
                            if (x == null)
                            {
                                outInfo = default(WorkbookInfo);
                                return false;
                            }
                            ClassSheets.Add(x);
                        }
                        break;
                    default:
                        break;
                }
            }
            outInfo = new WorkbookInfo(
                excel,
                ClassSheets.ToArray(),
                EnumSheets.ToArray(),
                ConstSheets.ToArray()
            );
            return true;
        }
    }
}