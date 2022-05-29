using System.Diagnostics;

namespace NF.Tools.DataFlow.Internal
{
    [DebuggerDisplay("<int2({x}, {y})>")]
    public record struct int2
    {
        public int x { get; init; }
        public int y { get; init; }

        public int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}