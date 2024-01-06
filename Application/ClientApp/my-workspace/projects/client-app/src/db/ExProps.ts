import * as RIAPP from "jriapp-lib";
import * as FOLDERBROWSER_SVC from "./folderDB";
import { BehaviorSubject, Observable } from 'rxjs';

export const infoType = "BASE_ROOT", utils = RIAPP.Utils;


class ChildDataView extends RIAPP.ChildDataView<FOLDERBROWSER_SVC.FileSystemObject> {
}

export class ExProps extends RIAPP.BaseObject {
  private _item: FOLDERBROWSER_SVC.FileSystemObject;
  private _childView: RIAPP.ChildDataView<FOLDERBROWSER_SVC.FileSystemObject>
  private _dbSet: FOLDERBROWSER_SVC.FileSystemObjectDb;
  private _dbContext: FOLDERBROWSER_SVC.DbContext;
  protected _clickTimeOut: any;
  private items: BehaviorSubject<FOLDERBROWSER_SVC.IFileSystemObject[]>;

  items$: Observable<FOLDERBROWSER_SVC.IFileSystemObject[]>;

  constructor(item: FOLDERBROWSER_SVC.FileSystemObject, dbContext: FOLDERBROWSER_SVC.DbContext) {
    super();
    this.items = new BehaviorSubject<FOLDERBROWSER_SVC.IFileSystemObject[]>([]);
    this.items$ = this.items.asObservable();
    this._item = item;
    this._dbContext = dbContext;
    this._childView = null;
    if (item.HasSubDirs) {
      this._childView = this.createChildView();
    }
    this._clickTimeOut = null;
    this._dbSet = <FOLDERBROWSER_SVC.FileSystemObjectDb>item._aspect.dbSet;
  }
  toggle(): void {
    const self = this;

    if (!self.childView)
      return;
    if (self.childView.count <= 0) {
      self.loadChildren();
    }
    else {
      self.childView.items.forEach((item) => {
        item._aspect.deleteItem();
      });
      self._dbSet.acceptChanges();
      self.refreshCss();
      self.items.next([]);
    }
  }
  click(): void {
    const self = this;
    if (!!self._clickTimeOut) {
      clearTimeout(self._clickTimeOut);
      self._clickTimeOut = null;
      self.objEvents.raise('dblclicked', { item: self._item });
    } else {
      self._clickTimeOut = setTimeout(function () {
        self._clickTimeOut = null;
        self.objEvents.raise('clicked', { item: self._item });
        self.toggle();
      }, 350);
    }
  }
  addOnClicked(fn: (sender: ExProps, args: { item: FOLDERBROWSER_SVC.FileSystemObject; }) => void, nmspace?: string) {
    this.objEvents.on('clicked', fn, nmspace);
  }
  offOnClicked(nmspace?: string) {
    this.objEvents.off('clicked', nmspace);
  }
  addOnDblClicked(fn: (sender: ExProps, args: { item: FOLDERBROWSER_SVC.FileSystemObject; }) => void, nmspace?: string) {
    this.objEvents.on('dblclicked', fn, nmspace);
  }
  offOnDblClicked(nmspace?: string) {
    this.objEvents.off('dblclicked', nmspace);
  }
  createChildView() {
    const self = this;
    let dvw = new ChildDataView(
      {
        association: self._dbContext.associations.getChildToParent(),
        parentItem: self._item,
        //we need to use refresh explicitly after the ChildDataView creation
        explicitRefresh: true
      });

    dvw.addOnFill((s, a) => {
      self.refreshCss();
      const children = dvw.items.map(item => {
        let vals = item._aspect.vals;
        utils.sys.setProp(vals, "exProp", item.ExtraProps);
        return vals as FOLDERBROWSER_SVC.IFileSystemObject;
      });
      this.items.next(children);
    });

    //explicit refresh with no async
    dvw.syncRefresh();
    return dvw;
  }
  loadChildren() {
    const self = this, query = self._dbSet.createReadChildrenQuery({ parentKey: self.item.Key, level: self.item.Level + 1, path: self.item.fullPath, includeFiles: false, infoType: infoType });
    query.isClearPrevData = false;
    let promise = query.load();
    return promise;
  }
  dispose() {
    if (this.getIsDisposed())
      return;
    this.setDisposing();
    const self = this;
    clearTimeout(self._clickTimeOut);
    if (!!this._childView) {
      this._childView.parentItem = null;
      this._childView.dispose();
      this._childView = null;
    }
    this._dbSet = null;
    this._dbContext = null;
    this._item = null;
    super.dispose();
  }
  refreshCss() {
    this.objEvents.raiseProp('css1');
    this.objEvents.raiseProp('css2');
  }
  get item() { return this._item; }
  get childView() { return this._childView; }
  get css1() {
    let children_css = this.item.HasSubDirs ? ' dynatree-has-children' : ''
    let folder_css = this.item.IsFolder ? ' dynatree-folder' : '';
    let css = '';
    if (!this._childView)
      css = 'dynatree-node dynatree-exp dynatree-ico-cf'; //dynatree-active
    else
      css = this._childView.count > 0 ? 'dynatree-node dynatree-exp-e dynatree-ico-ef' : 'dynatree-node dynatree-exp dynatree-ico-cf';
    /*
    if (!!this._childView)
        console.log(this._item.Name+ "   " + this._childView.count);
    */
    css += children_css;
    css += folder_css;
    return css;
  }
  get css2() {
    return this.item.HasSubDirs ? 'dynatree-expander' : 'dynatree-connector';
  }
}
