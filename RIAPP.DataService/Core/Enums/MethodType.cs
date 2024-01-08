using System;

namespace RIAPP.DataService.Core.Types
{

    [Flags]
    public enum MethodType
    {
        None = 0,
        Query = 1,
        Invoke = 2,
        Insert = 4,
        Update = 8,
        Delete = 16,
        Validate = 32,
        Refresh = 64
    }
}
