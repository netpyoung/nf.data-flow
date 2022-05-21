using System;

namespace NF.Tools.DataFlow.CodeGen.Internal
{
    [Flags]
    public enum E_PART
    {
        Client = 1 << 0,
        Server = 1 << 1,
        Common = Client | Server
    }
}