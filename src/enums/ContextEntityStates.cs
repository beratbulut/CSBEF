using System;

namespace CSBEF.enums
{
    [Flags]
    public enum ContextEntityStates
    {
        Unknown = -1,
        Detached = 0,
        Unchanged = 1,
        Deleted = 2,
        Modified = 3,
        Added = 4
    }
}