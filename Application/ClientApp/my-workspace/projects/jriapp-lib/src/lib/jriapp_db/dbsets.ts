/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import { BaseObject, LocaleERRS as ERRS, IBaseObject, IIndexer, Lazy, TEventHandler, Utils } from "../jriapp_shared";
import { DbSet } from "./dbset";
import { IDbSetConstuctorOptions } from "./int";

const utils = Utils, { Indexer, forEach, toArray } = utils.core, { format } = utils.str;

const enum DBSETS_EVENTS {
  DBSET_CREATING = "dbset_creating"
}

export type TDbSetCreatingArgs = { name: string; options: IDbSetConstuctorOptions; dbSet: DbSet | null; };

export class DbSets extends BaseObject {
  private _dbSets: IIndexer<Lazy<DbSet>>;
  private _dbSetOptions: IIndexer<IDbSetConstuctorOptions>;
  private _initializedDbSets: IIndexer<DbSet>;
  private _count: number;

  constructor() {
    super();
    this._initializedDbSets = Indexer();
    this._dbSets = Indexer();
    this._dbSetOptions = Indexer();
    this._count = 0;
  }
  override dispose(): void {
    if (this.getIsDisposed()) {
      return;
    }
    this.setDisposing();
   
    forEach(this._initializedDbSets, (name, dbSet) => {
      dbSet.dispose();
    });
    this._count = 0;
    this._initializedDbSets = Indexer();
    this._dbSets = Indexer();
    super.dispose();
  }
  protected _onDbSetInitialized(name: string, dbSet: DbSet): void {
    this._initializedDbSets[name] = dbSet;
    dbSet.objEvents.onProp("isHasChanges", (sender) => {
      sender.dbContext._getInternal().onDbSetHasChangesChanged(sender);
    });
  }
  _addDbSetOptions(name: string, options: IDbSetConstuctorOptions): void {
     this._dbSetOptions[name] = options;
  }
  _createDbSet(name: string, factory: (options: IDbSetConstuctorOptions) => DbSet): void {
    const self = this;
  
    if (!!self._dbSets[name]) {
      throw new Error(utils.str.format("DbSet: {0} is already created", name));
    }
    self._dbSets[name] = new Lazy<DbSet>(() => {
      const options = self._dbSetOptions[name];
      const args: TDbSetCreatingArgs = { name: name, options: options, dbSet: null };
      self.objEvents.raise(DBSETS_EVENTS.DBSET_CREATING, args);
      const res = !args.dbSet ? factory(options) : args.dbSet;
      self._onDbSetInitialized(name, res);
      return res;
    });
    this._count += 1;
  }
  addOnDbSetCreating(fn: TEventHandler<this, TDbSetCreatingArgs>, nmspace?: string, context?: IBaseObject): void {
    this.objEvents.on(DBSETS_EVENTS.DBSET_CREATING, fn, nmspace, context);
  }
  offOnDbSetCreating(nmspace?: string): void {
    this.objEvents.off(DBSETS_EVENTS.DBSET_CREATING, nmspace);
  }
  get count() {
    return this._count;
  }
  get dbSetNames(): string[] {
    return Object.keys(this._dbSets);
  }
  get arrDbSets(): DbSet[] {
    return toArray(this._initializedDbSets);
  }
  isDbSetExists(name: string): boolean {
    const res = this._dbSets[name];
    return !!res;
  }
  removeDbSet(name: string, removeOptions: boolean = false): boolean {
    const res = this._dbSets[name];
    if (!res) {
      return false;
    }
  
    const dbSet = this._initializedDbSets[name];
    if (!!dbSet){
      dbSet.dispose();
      delete this._initializedDbSets[name];
    }
    delete this._dbSets[name];
    if (removeOptions) {
      delete this._dbSetOptions[name];
    }
    this._count -= 1;
    return true;
  }
  findDbSet(name: string): DbSet|null {
    const res = this._dbSets[name];
    if (!res) {
      return null;
    }
    return res.Value;
  }
  getDbSet(name: string): DbSet {
    const dbSet = this.findDbSet(name);
    if (!dbSet) {
      throw new Error(format(ERRS.ERR_DBSET_NAME_INVALID!, name));
    }
    return dbSet;
  }
}
