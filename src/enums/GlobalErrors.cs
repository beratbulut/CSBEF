using System;

namespace CSBEF.enums
{
    [Flags]
    public enum GlobalErrors
    {
        TechnicalError,
        ModelValidationFail,
        BeforeReturnedError,
        AfterReturnedError,
        InvokerReturnedError,
        DataNotFound,
        DbNoEffectedRows,
        ArgsIsNull
    }
}