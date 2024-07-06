using System;

namespace RIAPP.DataService.Core.Types
{
    [Flags]
    public enum ValueFlags: int
    {
        None = 0,
        Changed = 1,
        Setted = 2,
        Refreshed = 4
    }
}
