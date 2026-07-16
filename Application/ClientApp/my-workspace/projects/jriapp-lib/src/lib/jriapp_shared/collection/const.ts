export const enum DATE_CONVERSION_ENUM { None = 0, ServerLocalToClientLocal = 1, UtcToClientLocal = 2 }

export type DATE_CONVERSION = keyof typeof DATE_CONVERSION_ENUM;

export const enum DATA_TYPE_ENUM {
    None = 0,
    String = 1,
    Bool = 2,
    Integer = 3,
    Decimal = 4,
    Float = 5,
    DateTime = 6,
    Date = 7,
    Time = 8,
    Guid = 9,
    Binary = 10
}

export type DATA_TYPE = keyof typeof DATA_TYPE_ENUM;

export const enum FIELD_TYPE_ENUM { None = 0, ClientOnly = 1, Calculated = 2, Navigation = 3, RowTimeStamp = 4, Object = 5, ServerCalculated = 6 }

export type FIELD_TYPE = keyof typeof FIELD_TYPE_ENUM;

export const enum SORT_ORDER_ENUM { ASC = 0, DESC = 1 }

export type SORT_ORDER = keyof typeof SORT_ORDER_ENUM;

export const enum FILTER_TYPE_ENUM { Equals = 0, Between = 1, StartsWith = 2, EndsWith = 3, Contains = 4, Gt = 5, Lt = 6, GtEq = 7, LtEq = 8, NotEq = 9 }

export type FILTER_TYPE = keyof typeof FILTER_TYPE_ENUM;

export const enum COLL_CHANGE_TYPE_ENUM { Remove = 0, Add = 1, Reset = 2, Remap = 3 }

export type COLL_CHANGE_TYPE = keyof typeof COLL_CHANGE_TYPE_ENUM;

export const enum COLL_CHANGE_REASON_ENUM { None = 0, PageChange = 1, Sorting = 2, Refresh = 3 }

export type COLL_CHANGE_REASON = keyof typeof COLL_CHANGE_REASON_ENUM;

export const enum COLL_CHANGE_OPER_ENUM { None = 0, Fill = 1, AddNew = 2, Remove = 3, Commit = 4, Sort = 5 }

export type COLL_CHANGE_OPER = keyof typeof COLL_CHANGE_OPER_ENUM;

export const enum ITEM_STATUS_ENUM { None = 0, Added = 1, Updated = 2, Deleted = 3 }

export type ITEM_STATUS = keyof typeof ITEM_STATUS_ENUM;

export const enum VALS_VERSION_ENUM { Current = 0, Temporary = 1, Original= 2 }

export type VALS_VERSION = keyof typeof VALS_VERSION_ENUM;
