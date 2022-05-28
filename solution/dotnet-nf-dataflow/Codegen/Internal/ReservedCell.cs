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
        public E_RESERVED Reserved { get; init; }
        public int2 Position { get; init; }
        public string CellString { get; init; }
    }
}