import { IBaseObject, IConverter } from "../../int";

/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */

export const enum BINDING_MODE {
  OneTime = 0,
  OneWay = 1,
  TwoWay = 2,
  BackWay = 3
}

export type TBindingMode = "OneTime" | "OneWay" | "TwoWay" | "BackWay";

// --Binding interfaces
export type TBindingOptions = {
  targetPath: string;
  sourcePath?: string;
  target?: IBaseObject;
  source?: any;
  isSourceFixed?: boolean;
  mode?: BINDING_MODE;
  converter?: IConverter;
  converterParam?: any;
};

// the result of parsing a data binding expression - typically all properties are strings here
export type TBindingInfo = {
  targetPath: string;
  sourcePath?: string;
  to?: string;
  target?: any;
  source?: any;
  mode?: TBindingMode;
  converter?: any;
  converterParam?: any;
};

export interface IBinding extends IBaseObject {
  target: IBaseObject;
  source: IBaseObject;
  targetPath: string[];
  sourcePath: string[];
  sourceValue: any;
  targetValue: any;
  readonly isSourceFixed: boolean;
  readonly mode: BINDING_MODE;
  readonly converter: IConverter;
  readonly param: any;
  isDisabled: boolean;
  updateTarget(): void;
  updateSource(): void;
}

export const enum TOKEN {
    DELIMETER1 = ":",
    DELIMETER2 = "=",
    COMMA = ",",
    THIS = "this.",
    PARAM = "param",
    TARGET_PATH = "targetPath",
    BIND = "bind",
    GET = "get",
    DATE = "date",
    INJECT = "inject"
}

export const enum TAG {
    NONE = "",
    LITERAL = "0",
    BIND = "1",
    GET = "2",
    DATE = "3",
    INJECT ="4",
    BRACE = "5",
    INDEXER = "6"
}

export const enum PARSE_TYPE {
    NONE = 0,
    BINDING = 1
}

export const enum DATES {
    NOW = "now",
    TODAY = "today",
    TOMORROW = "tomorrow",
    YESTERDAY = "yesterday"
}

export interface IKeyVal {
    tag?: TAG;
    key: string;
    val: any;
}
