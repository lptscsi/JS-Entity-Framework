/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import { IBaseObject, TErrorArgs } from "./int";
import { OBJ_EVENTS, ObjState, ObjectEvents, dummyEvents, OBJ_SIGNATURE } from "./object";
import { Checks } from "./utils/checks";
import { ERROR } from "./utils/error";

const { isHasProp, isFunc } = Checks;

type ExtractInitArgs<T> = T extends { _init: (...args: infer Args) => void } ? Args : any[];

// Enforces that getters and setters inside the descriptor map know what 'this' is
export type TypedPropertyDescriptorMap<U, Context> = {
    [P in keyof U]?: {
        configurable?: boolean;
        enumerable?: boolean;
        writable?: boolean;
        value?: U[P];
        get?(this: Context): U[P];
        set?(this: Context, v: U[P]): void;
    };
} & {
    // Allows defining arbitrary tracking fields that are not part of the public interface
    [key: string]: PropertyDescriptor & ThisType<Context>;
};

export type IObjectFactory<T extends IBaseObject, Parent = IBaseObject> = {
    create(...args: ExtractInitArgs<T>): T;
    
    extend<U extends Parent & IBaseObject>(
        properties: Partial<U> & {
            _init?: (...args: any[]) => void;
        } & ThisType<U & { _super: Parent }>,
        // Swapped PropertyDescriptorMap for our custom typed map
        propertyDescriptors: TypedPropertyDescriptorMap<U, U & { _super: Parent }>,
        fnAfterExtend?: ((obj: U) => void) | null
    ): IObjectFactory<U, T>;

    isPrototypeOf(obj: object): boolean;
};

interface IImplInternal extends IBaseObject {
    _super: any;
    _objState: ObjState;
    _objEvents: ObjectEvents | null;
    _typename?: string;
    _init(...args: any[]): void;
    objEvents: ObjectEvents;
    handleError(error: any, source: any): boolean;
    create(...args: any[]): any;
    extend(properties: any, propertyDescriptors: any, fnAfterExtend?: any): any;
    isPrototypeOf(obj: object): boolean;
    toString(): string;
}

const impl: IImplInternal = {
    _super: null,
    _objState: ObjState.None,
    _objEvents: null,

    _init() {
        this._super = null;
        this._objState = ObjState.None;
        this._objEvents = null;
    },

    create(this: any, ...args: any[]): IBaseObject {
        const instance = Object.create(this);
        (instance as any)[OBJ_SIGNATURE] = true;
        if (isFunc(instance._init)) {
            instance._init.apply(instance, args);
        }
        Object.seal(instance);
        return instance;
    },

    extend(
        this: any,
        properties: Record<string, any>,
        propertyDescriptors: PropertyDescriptorMap,
        fnAfterExtend: ((obj: any) => void) | null = null
    ): any {
        const pds: PropertyDescriptorMap = propertyDescriptors || {};
        const pdsProperties: string[] = Object.getOwnPropertyNames(pds);
        const rx_super = /\._super\s*\(/;

        pdsProperties.forEach(function (name) {
            let pd: PropertyDescriptor = pds[name];
            if (pd['enumerable'] === undefined) {
                pd['enumerable'] = true;
            }
            if (pd['configurable'] === undefined) {
                pd['configurable'] = false;
            }
        });

        if (!!properties) {
            const simpleProperties = Object.getOwnPropertyNames(properties);
            for (let i = 0, len = simpleProperties.length; i < len; ++i) {
                const propertyName = simpleProperties[i];
                if (pds.hasOwnProperty(propertyName)) {
                    continue;
                }
                pds[propertyName] = Object.getOwnPropertyDescriptor(properties, propertyName)!;
            }
        }

        let obj = Object.create(this, pds);
        const pnames: string[] = Object.getOwnPropertyNames(obj);

        pnames.forEach(function (name) {
            let p = Object.getOwnPropertyDescriptor(obj, name)!;
            if (!!p.value && isFunc(p.value)) {
                let fn_str = p.value.toString();
                let fn: Function = obj[name];

                if (rx_super.test(fn_str)) {
                    const superProto: any = Object.getPrototypeOf(obj);
                    if (name === 'dispose') {
                        p.value = function (this: any) {
                            const self: any = this;
                            const old: any = self._super;
                            self._super = superProto[name];
                            try {
                                self._objState = ObjState.Disposing;
                                return fn.apply(self, arguments);
                            } finally {
                                self._super = old;
                            }
                        };
                    } else {
                        p.value = function (this: any) {
                            const self: any = this;
                            const old: any = self._super;
                            self._super = superProto[name];
                            try {
                                return fn.apply(self, arguments);
                            } finally {
                                self._super = old;
                            }
                        };
                    }
                    Object.defineProperty(obj, name, p);
                } else if (name === 'dispose') {
                    p.value = function (this: any) {
                        this._objState = ObjState.Disposing;
                        return fn.apply(this, arguments);
                    };
                    Object.defineProperty(obj, name, p);
                }
            }
        });

        if (!!fnAfterExtend) {
            fnAfterExtend(obj);
        }
        return obj;
    },

    isPrototypeOf(obj: object): boolean {
        return Object.prototype.isPrototypeOf.call(this, obj);
    },

    isHasProp(prop: string): boolean {
        return isHasProp(this, prop);
    },

    handleError(error: any, source: any): boolean {
        if (ERROR.checkIsDummy(error)) {
            return true;
        }
        if (!error.message) {
            error = new Error("Error: " + error);
        }
        const args: TErrorArgs = { error: error, source: source, isHandled: false };
        this.objEvents.raise(OBJ_EVENTS.error, args);
        let isHandled = args.isHandled;
        if (!isHandled) {
            isHandled = ERROR.handleError(this, error, source);
        }
        return isHandled;
    },

    getIsDisposed(): boolean {
        return this._objState === ObjState.Disposed;
    },

    getIsStateDirty(): boolean {
        return this._objState !== ObjState.None;
    },

    dispose(): void {
        if (this._objState === ObjState.Disposed) {
            return;
        }
        try {
            if (!!this._objEvents) {
                this._objEvents.raise(OBJ_EVENTS.disposed, {});
                this._objEvents.off();
                this._objEvents = null;
            }
        } finally {
            this._objState = ObjState.Disposed;
        }
    },

    toString() {
        return !this._typename ? 'RIAPP Object' : this._typename;
    },

    get objEvents() {
        if (this._objState === ObjState.Disposed) {
            return dummyEvents;
        }
        if (!this._objEvents) {
            this._objEvents = new ObjectEvents(this);
        }
        return this._objEvents;
    }
};

export const BaseObjectExt = impl as unknown as IObjectFactory<IBaseObject>;

/*
interface ITestObj extends IBaseObject {
  readonly arg1: string;
  readonly arg2: number;
  testFunc(): number;
  readonly test: string;
}

interface ITestObjFactory extends IObjectFactory<ITestObj>
{
  create(arg1: string, arg2: number): ITestObj;
}

export const TestObj: ITestObjFactory = BaseObjectExt.extend<ITestObj>(
  {
    _init(this: any, arg1: string, arg2: number): void {
      this._super();
      this._val = "YYYYYYYYYYYY";
      this._arg1 = arg1;
      this._arg2 = arg2;
    },
    testFunc() {
      return 2;
    },
    dispose(): void {
      console.log("TESTOBJ DISPOSE CALLED");
      this._super();
    },
    get test() {
      return 'test ' + this.getIsStateDirty();
    }
  },
  {
    arg1: {
      get() {
        return this['_arg1'] + " " + this['_val'];
      }
    },
    arg2: {
      get() {
        return this['_arg2'];
      }
    }
  },
  (obj: any) =>
  {
    //console.log("after extend", obj);
  }
);

let tttt = TestObj.create("ffffffffff", 10);
console.log(tttt.getIsDisposed());
tttt.dispose();
console.log(tttt.getIsDisposed());

*/