/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import { ERRS } from "../../lang";
import { Utils } from "../utils/utils";
import { BaseCollection } from "./base";
import { IPropInfo } from "./int";
import { BaseList, IListItem, ListItemAspect } from "./list";
import { CollUtils } from "./utils";

const utils = Utils, { format } = utils.str, { isNt } = utils.check, sys = utils.sys, collUtils = CollUtils;

sys.getItemByProp = (obj: any, prop: string) => {
  if (obj instanceof BaseDictionary) {
    return (<BaseDictionary>obj).getItemByKey(prop);
  } else if (obj instanceof BaseCollection) {
    return (<BaseCollection>obj).getItemByPos(parseInt(prop, 10));
  } else {
    return null;
  }
};


export abstract class BaseDictionary<TItem extends IListItem = IListItem> extends BaseList<TItem> {
  private _keyName: string;

  constructor(keyName: string, props: IPropInfo[]) {
    if (!keyName) {
      throw new Error(format(ERRS.ERR_PARAM_INVALID, "keyName", keyName));
    }
    super(props);
    this._keyName = keyName;
    const keyFld = this.getFieldInfo(keyName);
    if (!keyFld) {
      throw new Error(format(ERRS.ERR_DICTKEY_IS_NOTFOUND, keyName));
    }
    keyFld.isPrimaryKey = 1;
  }
  // override
  protected override createItem(obj?: object): TItem {
    const isNew = !obj, vals: any = isNew ? collUtils.initVals(this.getFieldInfos(), {}) : obj;
    let key: string;
    if (isNew) {
      key = this._getNewKey();
    } else {
      if (isNt(vals[this._keyName])) {
        throw new Error(format(ERRS.ERR_DICTKEY_IS_EMPTY, this._keyName));
      }
      key = "" + vals[this._keyName];
    }
    const aspect = new ListItemAspect(this, vals, key, isNew);
    return aspect.item as TItem;
  }
  // override
  protected override _onItemAdded(item: TItem): void {
    super._onItemAdded(item);
    const key = (<any>item)[this._keyName], self = this;
    if (isNt(key)) {
      throw new Error(format(ERRS.ERR_DICTKEY_IS_EMPTY, this._keyName));
    }
    const oldkey = item._key, newkey = "" + key;
    if (oldkey !== newkey) {
      self._remapItem(oldkey, newkey, item);
      this._onCollectionChanged({
        changeType: 'Remap',
        reason: 'None',
        oper: 'Commit',
        items: [item],
        old_key: oldkey,
        new_key: newkey
      });
    }
    this.objEvents.raiseProp(<any>`[${item._key}]`);
  }
  // override
  protected override _onRemoved(item: TItem): void {
    const key = (<any>item)[this._keyName];
    super._onRemoved(item);
    this.objEvents.raiseProp(<any>`[${key}]`);
  }
  get keyName(): string {
    return this._keyName;
  }
  override toString(): string {
    return "BaseDictionary";
  }
}
