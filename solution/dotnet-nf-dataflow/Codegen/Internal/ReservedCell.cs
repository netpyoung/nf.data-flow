using System.Diagnostics;

namespace NF.Tools.DataFlow.CodeGen.Internal
{
    [DebuggerDisplay("<ReservedCell({Reserved}|{CellString}|{Position})>")]
    public class ReservedCell
    {
        public enum E_RESERVED
        {
            TABLE,
            TATTR,
            TDESC,
            PART,
            ATTR,
            TYPE,
            NAME,
            VALUE,
            DESC,
        }
        public E_RESERVED Reserved { get; set; }
        public int2 Position { get; set; }
        public string CellString { get; set; }
    }
}