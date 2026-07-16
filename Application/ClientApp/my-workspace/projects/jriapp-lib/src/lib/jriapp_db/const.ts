/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
export enum FLAGS { None = 0, Changed = 1, Setted = 2, Refreshed = 4 }
export enum REFRESH_MODE { NONE = 0, RefreshCurrent = 1, MergeIntoCurrent = 2, CommitChanges = 3 }
export enum DELETE_ACTION { NoAction = 0, Cascade = 1, SetNulls = 2 }
export enum DATA_OPER { None, Submit, Query, Invoke, Refresh, Init }
