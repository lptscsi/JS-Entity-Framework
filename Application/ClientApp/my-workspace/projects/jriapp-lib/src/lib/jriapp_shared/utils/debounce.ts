/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import { IDisposable, TFunc } from "../int";
import { ERROR } from "./error";
import { Utils } from "./utils";

const error = ERROR, win = window, { check } = Utils;

export class Debounce implements IDisposable {
  private _timer: any;
  private _interval: number;
  private _fn: TFunc;
  private _promise: Promise<void>;
  private _resolve: () => void;
  private _reject: (err: any) => void;

  constructor(interval: number = 0) {
    this._timer = null;
    this._interval = interval;
    this._fn = null;
  }
  dispose(): void {
    this.cancel();
    this._timer = void 0;
  }
  enque(fn: TFunc): Promise<void> {
    if (!fn) {
      throw new Error("Debounce enque: Invalid function argument");
    }
    if (this.getIsDisposed()) {
      return Promise.resolve();
    }

    // the last wins
    this._fn = fn;

    let promise: Promise<void> = this._promise;

    if (!promise) {
      this._promise = new Promise((resolve_it, reject_it) => {
        this._resolve = resolve_it;
        this._reject = reject_it;
      });
      promise = this._promise;
    }


    if (!!this._timer) {
      clearTimeout(this._timer);
      this._timer = null;
    }

    if (!this._timer) {
      const callback = () => {
        const resolve = this._resolve,
          reject = this._reject;

        this._promise = null;
        this._reject = null;
        this._resolve = null;

        const fn = this._fn;
        this._timer = null;
        this._fn = null;

        if (!!fn) {
          try {
            const res = fn();
            if (!!res && check.isThenable(res)) {
              res.then(
                () => {
                  if (!!resolve) {
                    resolve();
                  }
                }
                , (err) => {
                  if (!!reject) {
                    reject(err);
                  }

                  error.handleError(win, err, win);
                });
            }
            else {
              if (!!resolve) {
                resolve();
              }
            }
          } catch (err) {
            if (!!reject) {
              reject(err);
            }

            error.handleError(win, err, win);
          }
        }
      };

      this._timer = setTimeout(callback, this._interval);
    }

    return promise;
  }
  cancel(): void {
    const reject = this._reject;

    this._promise = null;
    this._reject = null;
    this._resolve = null;

    if (!!this._timer) {
      clearTimeout(this._timer);
    }
    if (!!reject) {
      reject("debounce cancel");
    }
    this._timer = null;
    this._fn = null;
  }
  get interval(): number {
    return this._interval;
  }
  getIsDisposed(): boolean {
    return this._timer === void 0;
  }
}
