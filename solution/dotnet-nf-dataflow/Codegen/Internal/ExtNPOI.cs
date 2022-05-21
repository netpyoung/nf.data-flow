using NPOI.SS.UserModel;

namespace NF.Tools.DataFlow.CodeGen.Internal
{
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
}