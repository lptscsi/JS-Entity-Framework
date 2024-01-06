/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import { DUMY_ERROR } from "./consts";
import { SysUtils } from "./utils/sysutils";
import { CoreUtils } from "./utils/coreutils";
import { IValidationInfo, IValidationError } from "./int";
import { ERRS, STRS } from "../lang";

const sys = SysUtils, { Indexer } = CoreUtils;

export class BaseError {
  private _message: string;

  constructor(message?: string) {
    this._message = message || "Error";
  }
  toString() {
    return this._message;
  }
  get isDummy() {
    return false;
  }
  get message() {
    return this._message;
  }
}

export class DummyError extends BaseError {
  private _origError: any;

  constructor(originalError: any) {
    super(DUMY_ERROR);
    this._origError = originalError;
  }
  override get isDummy(): boolean {
    return true;
  }
  get origError(): any {
    return this._origError;
  }
}

export class AbortError extends BaseError {
  private _reason: string;

  constructor(reason?: string) {
    super(DUMY_ERROR);
    this._reason = reason || "Operation Aborted";
  }
  override get isDummy(): boolean {
    return true;
  }
  get reason(): string {
    return this._reason;
  }
}

export class AggregateError extends BaseError {
  private _errors: any[];

  constructor(errors?: any[]) {
    super("AggregateError");
    this._errors = errors || [];
  }
  get errors(): any[] {
    return this._errors;
  }
  get count(): number {
    return this._errors.length;
  }
  override get message(): string {
    const hashMap: {
      [name: string]: any;
    } = Indexer();
    for (const err of this._errors) {
      if (!!err) {
        let str = "";
        if (err instanceof AggregateError) {
          str = (<AggregateError>err).message;
        } else if (err instanceof Error) {
          str = (<Error>err).message;
        } else if (!!err.message) {
          str = "" + err.message;
        } else {
          str = "" + err;
        }

        hashMap[str] = "";
      }
    }

    let msg = "";
    const errs = Object.keys(hashMap);

    for (const err of errs) {
      if (!!msg) {
        msg += "\r\n";
      }
      msg += "" + err;
    }

    if (!msg) {
      msg = "Aggregate Error";
    }
    return msg;
  }
  override toString(): string {
    return "AggregateError: " + "\r\n" + this.message;
  }
}


sys.isValidationError = (obj: any): obj is IValidationError => {
  return (!!obj && obj instanceof ValidationError);
};

export class ValidationError extends BaseError implements IValidationError {
  private _validations: IValidationInfo[];
  private _item: any;

  constructor(validations: IValidationInfo[], item: any) {
    let message = ERRS.ERR_VALIDATION + "\r\n";
    validations.forEach(function (err, i) {
      if (i > 0) {
        message = message + "\r\n";
      }
      if (!!err.fieldName) {
        message = message + " " + STRS.TEXT.txtField + ": '" + err.fieldName + "'  " + err.errors.join(", ");
      } else {
        message = message + err.errors.join(", ");
      }
    });
    super(message);
    this._validations = validations;
    this._item = item;
  }
  get item(): any {
    return this._item;
  }
  get validations(): IValidationInfo[] {
    return this._validations;
  }
}
