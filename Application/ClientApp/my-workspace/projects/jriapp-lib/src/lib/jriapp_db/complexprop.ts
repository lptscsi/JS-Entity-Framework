/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import {
    BaseObject,
    LocaleERRS as ERRS,
    IBaseObject,
    IErrorNotification,
    IValidationInfo,
    TEventHandler,
    Utils
} from "../jriapp_shared";
import { IFieldInfo } from "../jriapp_shared/collection/int";
import { EntityAspect } from "./entity_aspect";

const utils = Utils, { format } = utils.str;

export interface IBaseComplexProperty extends IErrorNotification {
  getName(): string;
  _addDisposable(obj: IBaseObject): void;
  _getFullPath(path: string): string;
  setValue(fullName: string, value: any): void;
  getValue(fullName: string): any;
  getFieldInfo(): IFieldInfo;
  getProperties(): IFieldInfo[];
  getEntity(): EntityAspect;
  getFullPath(name: string): string;
  getPropertyByName(name: string): IFieldInfo;
}

export interface IChildComplexProperty extends IBaseComplexProperty {
  getParent(): IBaseComplexProperty;
  getRootProperty(): IBaseComplexProperty;
}

export abstract class BaseComplexProperty extends BaseObject implements IBaseComplexProperty {
  private _name: string;

  constructor(name: string) {
    super();
    this._name = name;
  }
  getName(): string {
    return this._name;
  }
  abstract _addDisposable(obj: IBaseObject): void;
  abstract _getFullPath(path: string): string;
  abstract setValue(fullName: string, value: any): void;
  abstract getValue(fullName: string): any;
  abstract getFieldInfo(): IFieldInfo;
  abstract getProperties(): IFieldInfo[];
  abstract getFullPath(name: string): string;
  abstract getEntity(): EntityAspect;
  getPropertyByName(name: string): IFieldInfo {
    const arrProps = this.getProperties().filter((f) => { return f.fieldName === name; });
    if (!arrProps || arrProps.length !== 1) {
      throw new Error(format(ERRS.ERR_ASSERTION_FAILED, "arrProps.length === 1"));
    }
    return arrProps[0];
  }
  getIsHasErrors(): boolean {
    return this.getEntity().getIsHasErrors();
  }
  addOnErrorsChanged(fn: TEventHandler<BaseComplexProperty, any>, nmspace?: string, context?: any): void {
    const self = this;
    this.getEntity().addOnErrorsChanged(function (this: any, _, args) { fn.apply(this, [self, args]); }, nmspace, context);
  }
  offOnErrorsChanged(nmspace?: string): void {
    this.getEntity().offOnErrorsChanged(nmspace);
  }
  getFieldErrors(fieldName: string): IValidationInfo[] {
    const name = this.getFullPath(fieldName);
    return this.getEntity().getFieldErrors(name);
  }
  getAllErrors(): IValidationInfo[] {
    return this.getEntity().getAllErrors();
  }
  getIErrorNotification(): IErrorNotification {
    return this;
  }
}

export class RootComplexProperty extends BaseComplexProperty {
  private _entity: EntityAspect;

  constructor(name: string, owner: EntityAspect) {
    super(name);
    this._entity = owner;
    this._entity._addDisposable(this);
  }

  _addDisposable(obj: IBaseObject): void {
    this._entity._addDisposable(obj);
  }
  _getFullPath(path: string): string {
    return this.getName() + "." + path;
  }
  setValue(fullName: string, value: any): void {
    this._entity._setFieldVal(fullName, value);
  }
  getValue(fullName: string): any {
    return this._entity._getFieldVal(fullName);
  }
  getFieldInfo(): IFieldInfo {
    return this._entity.getFieldInfo(this.getName());
  }
  getProperties(): IFieldInfo[] {
    return this.getFieldInfo().nested;
  }
  getEntity(): EntityAspect {
    return this._entity;
  }
  getFullPath(name: string): string {
    return this.getName() + "." + name;
  }
}

export class ChildComplexProperty extends BaseComplexProperty implements IChildComplexProperty {
  private _parent: BaseComplexProperty;

  constructor(name: string, parent: BaseComplexProperty) {
    super(name);
    this._parent = parent;
    this._parent._addDisposable(this);
  }
  _addDisposable(obj: IBaseObject): void {
    this._parent._addDisposable(obj);
  }
  _getFullPath(path: string): string {
    return this._parent._getFullPath(this.getName() + "." + path);
  }
  setValue(fullName: string, value: any): void {
    this.getEntity()._setFieldVal(fullName, value);
  }
  getValue(fullName: string): any {
    return this.getEntity()._getFieldVal(fullName);
  }
  getFieldInfo(): IFieldInfo {
    const name = this.getName();
    return this._parent.getPropertyByName(name);
  }
  getProperties(): IFieldInfo[] {
    return this.getFieldInfo().nested;
  }
  getFullPath(name: string): string {
    return this._parent._getFullPath(this.getName() + "." + name);
  }
  getEntity(): EntityAspect {
    return this.getRootProperty().getEntity();
  }
  getParent(): BaseComplexProperty {
    return this._parent;
  }
  getRootProperty(): BaseComplexProperty {
    let parent = this._parent;
    while (!!parent && (parent instanceof ChildComplexProperty)) {
      parent = (<ChildComplexProperty>parent).getParent();
    }
    if (!parent || !(parent instanceof RootComplexProperty)) {
      throw new Error(format(ERRS.ERR_ASSERTION_FAILED, "parent instanceof RootComplexProperty"));
    }
    return parent;
  }
}
