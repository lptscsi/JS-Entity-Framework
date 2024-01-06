/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import { BaseObject } from "../object";
import { ItemAspect } from "./aspect";
import { ICollectionItem } from "./int";

export class CollectionItem<TAspect extends ItemAspect> extends BaseObject implements ICollectionItem {
  private __aspect: TAspect;

  constructor(aspect: TAspect) {
    super();
    this.__aspect = aspect;
  }
  override dispose(): void {
    if (this.getIsDisposed()) {
      return;
    }
    this.setDisposing();
    const aspect = this.__aspect;
    if (!aspect.getIsStateDirty()) {
      aspect.dispose();
    }
    super.dispose();
  }
  get _aspect(): TAspect {
    return this.__aspect;
  }
  get _key(): string {
    return this.__aspect.key;
  }
  override toString(): string {
    return "CollectionItem";
  }
}
