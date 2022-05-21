using System.Diagnostics;

namespace NF.Tools.DataFlow.CodeGen.Internal
{
    [DebuggerDisplay("<int2({x}, {y})>")]
    public record struct int2(int x, int y);
}