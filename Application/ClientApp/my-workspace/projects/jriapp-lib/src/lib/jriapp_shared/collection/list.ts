/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import { ERRS } from "../../lang";
import { ValidationError } from "../errors";
import { IIndexer, IValidationError } from "../int";
import { Utils } from "../utils/utils";
import { ItemAspect } from "./aspect";
import { BaseCollection } from "./base";
import { COLL_CHANGE_OPER, COLL_CHANGE_REASON, COLL_CHANGE_TYPE, VALS_VERSION } from "./const";
import { ICollectionItem, IFieldInfo, IPropInfo } from "./int";
import { CollectionItem } from "./item";
import { CollUtils } from "./utils";

const utils = Utils, { Indexer } = utils.core, { format } = utils.str, { isArray } = utils.check,
  { walkField, initVals } = CollUtils, sys = utils.sys;

export interface IListItem extends ICollectionItem {
  readonly _aspect: ListItemAspect;
}

export class ListItemAspect extends ItemAspect {
  _setProp(name: string, val: any): void {
    if (this.isCancelling) {
      return;
    }
    let error: IValidationError;
    const coll = this.coll, item = this.item, fieldInfo = this.getFieldInfo(name),
      errors = coll.errors;
    if (this._getProp(name) !== val) {
      try {
        if (fieldInfo.isReadOnly && !(this.isNew && fieldInfo.allowClientDefault)) {
          throw new Error(ERRS.ERR_FIELD_READONLY);
        }
        this._setValue(name, val, VALS_VERSION.Current);
        sys.raiseProp(item, name);
        errors.removeError(item, name);
        const validationInfo = this._validateField(name);
        if (!!validationInfo && validationInfo.errors.length > 0) {
          throw new ValidationError([validationInfo], this);
        }
      } catch (ex) {
        if (utils.sys.isValidationError(ex)) {
          error = ex;
        } else {
          error = new ValidationError([
            { fieldName: name, errors: [ex.message] }
          ], this);
        }
        errors.addError(item, name, error.validations[0].errors);
        throw error;
      }
    }
  }
  _getProp(name: string): any {
    return this._getValue(name, VALS_VERSION.Current);
  }
  override toString(): string {
    if (!this.item) {
      return "ListItemAspect";
    }
    return this.item.toString() + "Aspect";
  }
  get list(): BaseList {
    return <BaseList>this.coll;
  }
}

export class ListItem extends CollectionItem<ListItemAspect> {

}

export abstract class BaseList<TItem extends IListItem = IListItem> extends BaseCollection<TItem> {
  private _fieldMap: IIndexer<IFieldInfo>;
  private _fieldInfos: IFieldInfo[];
  private _newKey: number;

  constructor(props: IPropInfo[]) {
    super();
    this._fieldMap = Indexer();
    this._fieldInfos = [];
    this._newKey = 0;

    if (!!props) {
      this._updateFieldMap(props);
    }
  }
  private _updateFieldMap(props: IPropInfo[]) {
    const self = this;
    if (!isArray(props) || props.length === 0) {
      throw new Error(format(ERRS.ERR_PARAM_INVALID, "props", props));
    }

    self._fieldMap = Indexer();
    self._fieldInfos = [];
    for (const prop of props) {
      const fldInfo = BaseCollection.getEmptyFieldInfo(prop.name);
      fldInfo.dataType = prop.dtype;
      self._fieldMap[prop.name] = fldInfo;
      self._fieldInfos.push(fldInfo);
      walkField(fldInfo, (fld, fullName) => {
        fld.dependents = null;
        fld.fullName = fullName;
      });
    }
  }
  protected override _clear(reason: COLL_CHANGE_REASON, oper: COLL_CHANGE_OPER) {
    super._clear(reason, oper);
    this._newKey = 0;
  }
  protected createItem(obj?: object): TItem {
    const isNew = !obj, vals: any = isNew ? initVals(this.getFieldInfos(), {}) : obj,
      key = this._getNewKey();
    const aspect = new ListItemAspect(this, vals, key, isNew);
    return aspect.item as TItem;
  }
  protected _refreshValues(item: TItem, obj?: object): void {
    const fields = this.getFieldInfos();

    fields.forEach((value) => {
      const name = value.fieldName;
      item._aspect._setProp(name, sys.getProp(obj, name));
    });
  }
  protected _getNewKey(): string {
    // client side item ID
    const key = "clkey_" + this._newKey;
    this._newKey += 1;
    return key;
  }
  // override
  protected _createNew(): TItem {
    return this.createItem(null);
  }
  // override
  getFieldMap(): IIndexer<IFieldInfo> {
    return this._fieldMap;
  }
  // override
  getFieldInfos(): IFieldInfo[] {
    return this._fieldInfos;
  }
  fillItems(objArray: object[], clearAll?: boolean) {
    const self = this, newItems: TItem[] = [], items: TItem[] = [];
    if (!objArray) {
      objArray = [];
    }

    try {
      if (!!clearAll) {
        this.clear();
      }

      for (const obj of objArray) {
        const item = self.createItem(obj), oldItem = self.getItemByKey(item._key);
        if (!oldItem) {
          self._appendItem(item);
          newItems.push(item);
          items.push(item);
          item._aspect._setIsAttached(true);
        } else {
          items.push(oldItem);
          this._refreshValues(oldItem, obj);
        }
      }

      if (newItems.length > 0) {
        this.objEvents.raiseProp("count");
      }
    } finally {
      this._onCollectionChanged({
        changeType: COLL_CHANGE_TYPE.Reset,
        reason: COLL_CHANGE_REASON.None,
        oper: COLL_CHANGE_OPER.Fill,
        items: items
      });
      this._onFillEnd({
        items: items,
        newItems: newItems,
        reason: COLL_CHANGE_REASON.None
      });
    }
    if (!!clearAll) {
      this.moveFirst();
    }
  }
  getNewItems(): TItem[] {
    return this.items.filter(function (item) {
      return item._aspect.isNew;
    });
  }
  resetStatus(): void {
    for (const item of this.items) {
      item._aspect._resetStatus();
    }
  }
  toArray(): object[] {
    return this.items.map((item) => {
      return item._aspect.vals;
    });
  }
  override toString(): string {
    return "BaseList";
  }
}


