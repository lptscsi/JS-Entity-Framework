import { HttpClient } from '@angular/common/http';
import { Injectable, Optional, SkipSelf } from '@angular/core';
import {
  IPromise, IQueryResult, IStatefulPromise, Utils,
  IRowData, IColumn, DataView, BaseObject
} from 'jriapp-lib';
import { DbContext, Product, ProductCategory, ProductCategoryDb } from "../db/adwDB";

const utils = Utils;

export interface IOptions {
  service_url: string;
}

type IQueryResponse = {
  columns: IColumn[];
  rows: IRowData[];
  dbSetName: string;
  totalCount: string | null;
};

type IStaticData = {
  productModelData: IQueryResponse;
  productCategoryData: IQueryResponse;
};

export class Filter extends BaseObject {
  readonly dbContext: DbContext;

  private _parentCategoryId: number = null;
  private _childCategoryId: number = null;
  private _parentCategories: DataView<ProductCategory>;
  private _childCategories: DataView<ProductCategory>;

  constructor(dbContext: DbContext) {
    super();
    this.dbContext = dbContext;
    const self = this;

    //filters top level product categories
    this._parentCategories = new DataView<ProductCategory>(
      {
        dataSource: this.dbContext.dbSets.ProductCategory,
        fn_sort: function (a, b) { return a.ProductCategoryId - b.ProductCategoryId; },
        fn_filter: function (item) { return item.ParentProductCategoryId === null; }
      });

    //filters product categories which have parent category
    this._childCategories = new DataView<ProductCategory>(
      {
        dataSource: this.dbContext.dbSets.ProductCategory,
        fn_sort: function (a, b) { return a.ProductCategoryId - b.ProductCategoryId; },
        fn_filter: function (item) { return item.ParentProductCategoryId !== null && item.ParentProductCategoryId == self.parentCategoryId; }
      });
  }

  reset() {
    this.parentCategoryId = null;
    this.childCategoryId = null;
  }

  get parentCategoryId() {  return this._parentCategoryId;  }
  set parentCategoryId(v) {
    if (this._parentCategoryId != v) {
      this._parentCategoryId = v;
      this.objEvents.raiseProp('parentCategoryId');
      this._childCategories.refreshSync();
    }
  }
  get childCategoryId() { return this._childCategoryId; }
  set childCategoryId(v) {
    if (this._childCategoryId != v) {
      this._childCategoryId = v;
      this.objEvents.raiseProp('childCategoryId');
    }
  }

  get dbSets() { return this.dbContext.dbSets; }
  get parentCategories() { return this._parentCategories; }
  get childCategories() { return this._childCategories; }
  get productModels() { return this.dbSets.ProductModel; }
  get productCategories() { return this.dbSets.ProductCategory; }
}

@Injectable({
  providedIn: 'root',
})
export class AdwService {
  private _dbContext: DbContext;
  private _uniqueID: string;
  private _initPromise: Promise<any>;
  private _filter: Filter = null;

  constructor(
    @Optional() @SkipSelf() existingService: AdwService,
    private readonly http: HttpClient
  ) {
    if (existingService) {
      throw Error(
        'The service has already been provided in the app. Avoid providing it again in child modules'
      );
    }

    const self = this, options: IOptions = { service_url: "/RIAppDemoServiceEF" };
    self._uniqueID = utils.core.getNewID();
    self._dbContext = new DbContext();

    self._initPromise = self.dbContext.initialize({
      serviceUrl: options.service_url,
      http: http
    }).then(() => {
      this.dbSet.addOnEndEdit(function (_s, args) {
        // NOOP
      }, self.uniqueID);

      this.dbSet.addOnFill(function (_s, args) {
        // NOOP
      }, self.uniqueID);

      this.dbContext.objEvents.onProp('isHasChanges', function (_s, data) {
        // NOOP
      }, self.uniqueID);

      this.dbContext.addOnError(function (s, args) {
        console.error(args.error);
      }, self.uniqueID);
    });
  }

  async loadStaticData() {
    const self = this;

    this._filter = new Filter(this.dbContext);
   

    const url = this.dbContext.getUrl('static');
    const promise = new Promise<IStaticData>((resolve, reject) => {
      this.http.get(url).subscribe(
        (res) => resolve(res as unknown as IStaticData), // success path
        err => reject(err) // error path
      );
    });
    const staticData = await promise;

    // subscribe to the view refreshed event
    this.filter.parentCategories.addOnViewRefreshed((_s, args) => {
      if (this.filter.parentCategories.currentItem) {
        do {
          const parentItem = this.filter.parentCategories.currentItem;
          this.filter.parentCategoryId = parentItem.ProductCategoryId;

          console.log(`parentCategory: ${this.filter.parentCategoryId}) ${parentItem.Name}`, this.filter.childCategories.items);
        } while (this.filter.parentCategories.moveNext());
      }

    }, this.uniqueID);

    this.dbContext.dbSets.ProductModel.fillData(staticData.productModelData);
    this.dbContext.dbSets.ProductCategory.fillData(staticData.productCategoryData);
  }

  load(pageIndex: number = 0, pageSize: number): IStatefulPromise<IQueryResult<Product>> {
    const self = this, query = self.dbSet.createReadProductQuery({ param1: [0], param2: "test" });
    query.isClearPrevData = true;
    query.isPagingEnabled = true;
    query.pageIndex = pageIndex;
    query.pageSize = pageSize;
    query
      .orderBy('Name')
      .thenBy('SellStartDate', 'DESC');
    let promise = query.load();
    return promise;
  }

  pageChanged(pageIndex: number = 0, pageSize: number) {
    const self = this;
    self.dbSet.pageSize = pageSize;
    self.dbSet.pageIndex = pageIndex;
  }

  sortChanged(field: string, order: number): IStatefulPromise<IQueryResult<Product>>  {
    const self = this, query = self.dbSet.query;
    if (!!field) {
      query.clearSort();
      query.pageIndex = 0;
      query.orderBy(field, order > 0 ? 'DESC' : 'ASC');
      // console.log(query.sortInfo);
    }
    else {
      query
        .orderBy('Name')
        .thenBy('SellStartDate', 'DESC');
    }
    return query.load();
  }

  submit(): IPromise<any> {
    const self = this;
    return self.dbContext.submitChanges();
  }
  get initPromise() {
    return this._initPromise;
  }
  get dbContext() { return this._dbContext; }
  get dbSet() { return this._dbContext.dbSets.Product; }
  get currentItem(): Product {
    return this.dbSet.currentItem;
  }
  get isHasChanges() {
    return this.dbContext.isHasChanges;
  }
  get uniqueID() {
    return this._uniqueID;
  }
  get filter() {
    return this._filter;
  }
}
