/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import { IIndexer } from "../int";
import { Checks } from "./checks";
import { StringUtils } from "./strutils";

const { isHasProp, _undefined, isBoolean, isArray, isPlainObject, isNt, isString } = Checks,
    { format: formatStr, fastTrim: trim } = StringUtils, { getOwnPropertyNames, getOwnPropertyDescriptor, keys: objectKeys } = Object;
const UUID_CHARS: string[] = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".split("");
const NEWID_MAP: IIndexer<number> = Indexer();

function Indexer<T = any>(): IIndexer<T>
{
    return Object.create(null);
}

function clone(obj: any, target?: any): any {
    if (!obj) {
        return obj;
    }

    let res: any;

    if (isArray(obj)) {
        res = [];
        const len = obj.length;
        for (let i = 0; i < len; i += 1) {
            res.push(clone(obj[i], null));
        }
    } else if (isPlainObject(obj)) {
        // clone only plain objects
        res = target || {};
        const keys = getOwnPropertyNames(obj), len = keys.length;
        for (let i = 0; i < len; i += 1) {
            const p = keys[i];
            res[p] = clone(obj[p], null);
        }
    } else {
        res = obj;
    }

    return res;
}

function extend<T, U>(target: T, ...source: U[]): T & U {
    if (isNt(target)) {
        throw new TypeError("extend: Cannot convert first argument to object");
    }

    const to = Object(target);
    for (let i = 0; i < source.length; i++) {
        const nextSource = source[i] as IIndexer<any>;
        if (nextSource === _undefined || nextSource === null) {
            continue;
        }

        const keys = objectKeys(Object(nextSource)), len = keys.length;
        for (let nextIndex = 0; nextIndex < len; nextIndex++) {
            const nextKey = keys[nextIndex], desc = getOwnPropertyDescriptor(nextSource, nextKey);
            if (desc !== _undefined && desc.enumerable) {
                to[nextKey] = nextSource[nextKey];
            }
        }
    }
    return to;
}

function assignStrings<T extends U, U extends IIndexer<any>>(target: T, source: U): T {
    if (isNt(target)) {
        target = <any>{};
    }
    if (!isPlainObject(source)) {
        return target;
    }
   
    const keys = <(keyof U)[]>objectKeys(source), len = keys.length;
  
    for (let i = 0; i < len; i += 1) {
        const p = keys[i], tval = target[p], sval = source[p];
        if (isPlainObject(sval)) {
            target[p] = <any>assignStrings(tval, sval);
        } else if (isString(sval)) {
            target[p] = <any>sval;
        }
    }

    return target;
}

const ERR_OBJ_ALREADY_REGISTERED = "an Object with the name: {0} is already registered and can not be overwritten";

// basic utils
export class CoreUtils {
    static getNewID(prefix: string = "*"): string {
        const id = NEWID_MAP[prefix] || 0;
        NEWID_MAP[prefix] = id + 1;
        return (prefix === "*") ? id.toString(36) : (prefix + "_" + id.toString(36));
    }
    static readonly getTimeZoneOffset = (() => {
        const dt = new Date(), tz = dt.getTimezoneOffset();
        return () => tz;
    })();
    static readonly hasProp = isHasProp;
    static setValue(root: any, namePath: string, val: any, checkOverwrite: boolean = false): void {
        const parts: string[] = namePath.split("."), len = parts.length;
        let parent = root;
        for (let i = 0; i < len - 1; i += 1) {
            // create a property if it doesn't exist
            if (!parent[parts[i]]) {
                parent[parts[i]] = Indexer();
            }
            parent = parent[parts[i]];
        }
        // the last part is the name itself
        const n = parts[len - 1];
        if (!!checkOverwrite && (parent[n] !== _undefined)) {
            throw new Error(formatStr(ERR_OBJ_ALREADY_REGISTERED, namePath));
        }
        parent[n] = val;
    }
    static getValue(root: any, namePath: string): any {
        const parts = namePath.split(".");
        let res: any, parent = root;
        for (let i = 0; i < parts.length; i += 1) {
            res = parent[parts[i]];
            if (res === _undefined) {
                return null;
            }
            parent = res;
        }
        return res;
    }
    static removeValue(root: any, namePath: string): any {
        const parts = namePath.split(".");
        let parent = root;
        for (let i = 0; i < parts.length - 1; i += 1) {
            if (!parent[parts[i]]) {
                return null;
            }
            parent = parent[parts[i]];
        }
        // the last part is the object name itself
        const n = parts[parts.length - 1], val = parent[n];
        if (val !== _undefined) {
            delete parent[n];
        }

        // returns deleted value
        return val;
    }
    static uuid(len?: number, radix?: number): string {
        let i: number;
        const chars = UUID_CHARS, uuid: string[] = [], rnd = Math.random;
        radix = radix || chars.length;

        if (!!len) {
            // Compact form
            for (i = 0; i < len; i += 1) { uuid[i] = chars[0 | rnd() * radix]; }
        } else {
            // rfc4122, version 4 form
            let r: number;

            // rfc4122 requires these characters
            uuid[8] = uuid[13] = uuid[18] = uuid[23] = "-";
            uuid[14] = "4";

            // Fill in random data.  At i===19 set the high bits of clock sequence as
            // per rfc4122, sec. 4.1.5
            for (i = 0; i < 36; i += 1) {
                if (!uuid[i]) {
                    r = 0 | rnd() * 16;
                    uuid[i] = chars[(i === 19) ? (r & 0x3) | 0x8 : r & 0xf];
                }
            }
        }

        return uuid.join("");
    }
    static parseBool(a: any): boolean {
        if (isBoolean(a)) {
            return a;
        }
        const v = trim(a).toLowerCase();
        if (v === "false") {
            return false;
        } else if (v === "true") {
            return true;
        } else {
            throw new Error(formatStr("parseBool, argument: {0} is not a valid boolean string", a));
        }
    }
    static round(num: number, decimals: number): number {
        return parseFloat(num.toFixed(decimals));
    }
    static readonly clone: (obj: any, target?: any) => any = clone;
    static merge<S, T>(source: S, target?: T): S & T {
        if (!target) {
            target = <any>{};
        }
        if (!source) {
            return <any>target;
        }
        return extend(target, source);
    }
    static readonly extend: <T, U>(target: T, ...source: U[]) => T & U = extend;
    static memoize<R>(fn: () => R): () => R;
    static memoize<T1, R>(fn: (A1: T1) => R): (A1: T1) => R;
    static memoize<T1, T2, R>(fn: (A1: T1, A2: T2) => R): (A1: T1, A2: T2) => R;
    static memoize<T1, T2, T3, R>(fn: (A1: T1, A2: T2, A3: T3) => R): (A1: T1, A2: T2, A3: T3) => R;
    static memoize<T>(fn: (...args: any[]) => T): (...args: any[]) => T {
        let memo = Indexer();
        return (...args: any[]) => {
            let key = "__dummy";
            if (!!args && args.length > 0) {
                key = args.join(':');
            } 

            if (key in memo) {
                return memo[key];
            } else {
                memo[key] = fn(...args);
                return memo[key];
            }
        };
    }
    static readonly Indexer: <T = any>() => IIndexer<T> = Indexer;
    static forEach<T>(map: IIndexer<T>, fn: (name: string, val: T) => void): void {
        if (!map) {
            return;
        }
        for (let key in map) {
            fn(key, map[key]);
        }
    }
    static forEachMap<T, U>(map: IIndexer<T>, fnMap: (val: T) => U, fnSort: { (a: T, b: T): number; } | null = null): U[] {
        if (!map) {
            return [];
        }
        let arr: T[] = [];

        for (let key in map) {
            arr.push(map[key]);
        }
        if (!!fnSort) {
            arr.sort(fnSort);
        }
        return arr.map(val => fnMap(val));
    }
    static toArray<T>(map: IIndexer<T>): T[] {
        const r: T[] = [];
        if (!map) {
            return r;
        }
        for (const key in map) {
            r.push(map[key]);
        }
        return r;
    }
    static readonly assignStrings: <T extends U, U extends IIndexer<any>>(target: T, source: U) => T = assignStrings;
    static pipe<T, R>(fn1: (...args: T[]) => R, ...fns: Array<(a: R) => R>): (...args: T[]) => R
    {
        const piped = fns.reduce((prevFn, nextFn) => (value: R) => nextFn(prevFn(value)),
            value => value
        );
        return (...args: T[]) => piped(fn1(...args));
    }
    static compose<R>(fn1: (a: R) => R, ...fns: Array<(a: R) => R>): (a: R) => R {
        return fns.reduce((prevFn, nextFn) => value => prevFn(nextFn(value)), fn1);
    }
}
