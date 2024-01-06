/*
  Generated from: /FolderBrowserService/typescript on 2024-01-03 at 20:13
  Don't make manual changes here, they will be lost when this interface will be regenerated!
*/

import * as RIAPP from "jriapp-lib";
import { ExProps } from "./ExProps";


export interface ISvcMethods {
}

export interface IFileSystemObject {
  readonly Key: string;
  readonly ParentKey: string | null;
  readonly Name: string;
  readonly Level: number;
  readonly HasSubDirs: boolean;
  readonly IsFolder: boolean;
}

export class FileSystemObject extends RIAPP.Entity {
  get Key(): string { return this._aspect._getFieldVal('Key'); }
  get ParentKey(): string | null { return this._aspect._getFieldVal('ParentKey'); }
  get Name(): string { return this._aspect._getFieldVal('Name'); }
  get Level(): number { return this._aspect._getFieldVal('Level'); }
  get HasSubDirs(): boolean { return this._aspect._getFieldVal('HasSubDirs'); }
  get IsFolder(): boolean { return this._aspect._getFieldVal('IsFolder'); }
  get fullPath(): string | null { return this._aspect._getCalcFieldVal('fullPath'); }
  get ExtraProps(): ExProps { return this._aspect._getCalcFieldVal('ExtraProps'); }
  get Parent(): FileSystemObject { return this._aspect._getNavFieldVal('Parent'); }
  set Parent(v: FileSystemObject) { this._aspect._setNavFieldVal('Parent', v); }
  get Children(): FileSystemObject[] { return this._aspect._getNavFieldVal('Children'); }
  override toString() {
    return 'FileSystemObject';
  }
}

export class FileSystemObjectDb extends RIAPP.DbSet<FileSystemObject>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new FileSystemObject(aspect);
    super(opts);
  }
  findEntity(key: string): FileSystemObject {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'FileSystemObjectDb';
  }
  createReadAllQuery(args?: {
    includeFiles: boolean;
    infoType: string;
  }): RIAPP.DataQuery<FileSystemObject> {
    var query = this.createQuery('ReadAll');
    query.params = args;
    return query;
  }
  createReadChildrenQuery(args?: {
    parentKey: string;
    level: number;
    path: string;
    includeFiles: boolean;
    infoType: string;
  }): RIAPP.DataQuery<FileSystemObject> {
    var query = this.createQuery('ReadChildren');
    query.params = args;
    return query;
  }
  createReadRootQuery(args?: {
    includeFiles: boolean;
    infoType: string;
  }): RIAPP.DataQuery<FileSystemObject> {
    var query = this.createQuery('ReadRoot');
    query.params = args;
    return query;
  }
  defineFullPathField(getFunc: (item: FileSystemObject) => string | null) { this.defineCalculatedField('fullPath', getFunc); }
  defineExtraPropsField(getFunc: (item: FileSystemObject) => ExProps) { this.defineCalculatedField('ExtraProps', getFunc); }
}

export interface IAssocs {
  getChildToParent: () => RIAPP.Association;
}


export class DbSets extends RIAPP.DbSets {
  constructor() {
    super();
    this._createDbSet("FileSystemObject", (options) => new FileSystemObjectDb(options));
  }
  get FileSystemObject() { return <FileSystemObjectDb>this.getDbSet("FileSystemObject"); }
}
export class DbContext extends RIAPP.DbContext<ISvcMethods, IAssocs, DbSets>
{
  protected override _provideDbSets(): DbSets {
    return new DbSets();
  }
}
