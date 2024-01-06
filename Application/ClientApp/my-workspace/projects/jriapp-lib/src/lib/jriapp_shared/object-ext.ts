/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import {
    IBaseObject, TErrorArgs
} from "./int";
import { OBJ_EVENTS, ObjState, ObjectEvents, dummyEvents, objSignature } from "./object";
import { Checks } from "./utils/checks";
import { ERROR } from "./utils/error";

const { isHasProp, isFunc } = Checks;

export type IObjectFactory<T extends IBaseObject> = {
  create(...args: any[]): T;
  extend<U extends IBaseObject>(properties: Partial<U> & { _init?: (...args: any[]) => void; },
    propertyDescriptors: PropertyDescriptorMap,
    fnAfterExtend: { (obj: any): void; } | null): IObjectFactory<U>;
  isPrototypeOf(obj: object): boolean;
};

/**
 * Analog of BaseObject but allows for dynamic types creation
 * for example: is used in DbSet class for dynamic entity types creation
 */
export const BaseObjectExt = {
  _init() {
    this._super = null;
    this._objState = ObjState.None;
    this._objEvents = null;
  },
  create(): IBaseObject {
    const instance: {
      _init: Function,
    } = Object.create(this);
 
    instance._init.apply(instance, arguments);
    Object.seal(instance);
    return instance as any;
  },
  extend(properties: object, propertyDescriptors: PropertyDescriptorMap, fnAfterExtend: { (obj: any): void; } | null = null): any {
    const pds: PropertyDescriptorMap = propertyDescriptors || {}, pdsProperties: string[] = Object.getOwnPropertyNames(pds);
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

        pds[propertyName] = Object.getOwnPropertyDescriptor(properties, propertyName);
      }
    }

    let obj = Object.create(this, pds);
   
    const pnames: string[] = Object.getOwnPropertyNames(obj);
    pnames.forEach(function (name) {
      let p = Object.getOwnPropertyDescriptor(obj, name);
      if (!!p.value && isFunc(p.value)) {
        let fn_str = p.value.toString();
        let fn: Function = obj[name];
        //this wrapping of the original function is only for the functions which use _super function calls
        if (rx_super.test(fn_str)) {
          const superProto: any = Object.getPrototypeOf(obj);
          if (name == 'dispose') {
            p.value = function () {
              const old = this._super;
              this._super = superProto[name];
              try {
                this._objState = ObjState.Disposing;
                return fn.apply(this, arguments);
              }
              finally {
                this._super = old;
              }
            };
          }
          else {
            p.value = function () {
              const old = this._super;
              this._super = superProto[name];
              try {
                return fn.apply(this, arguments);
              }
              finally {
                this._super = old;
              }
            };
          }
          Object.defineProperty(obj, name, p);
        }
        else if (name == 'dispose') {
          p.value = function () {
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
    //console.log("BASEOBJ DISPOSE CALLED");
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
  },
  get __objSig() {
    return objSignature;
  }
} as IObjectFactory<IBaseObject>;

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
    _init(arg1: string, arg2: number): void {
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
        return this._arg1 + " " + this._val;
      }
    },
    arg2: {
      get() {
        return this._arg2;
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
