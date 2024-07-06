/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
export const enum FLAGS { None = 0, Changed = 1, Setted = 2, Refreshed = 4 }

export const enum REFRESH_MODE_ENUM { None = 0, RefreshCurrent = 1, MergeIntoCurrent = 2, CommitChanges = 3 }

export type REFRESH_MODE = keyof typeof REFRESH_MODE_ENUM;

export const enum DELETE_ACTION_ENUM { NoAction = 0, Cascade = 1, SetNulls = 2 }

export type DELETE_ACTION = keyof typeof DELETE_ACTION_ENUM;

export const enum DATA_OPER_ENUM { None, Submit, Query, Invoke, Refresh, Init }

export type DATA_OPER = keyof typeof DATA_OPER_ENUM;
