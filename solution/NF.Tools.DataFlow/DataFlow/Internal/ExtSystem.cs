using System;

namespace NF.Tools.DataFlow.Internal
{
    public static class ExtSystem
    {
        public static bool HasFlag(this Enum self, in Enum flag)
        {
            int selfValue = Convert.ToInt32(self);
            int flagValue = Convert.ToInt32(flag);

            return (selfValue & flagValue) == flagValue;
        }
    }
}