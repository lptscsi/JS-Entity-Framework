import { Component, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { DbSet, Utils } from "jriapp-lib";
import { SortEvent } from 'primeng/api';
import { PaginatorState } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { Product } from 'projects/client-app/src/db/adwDB';
import { BehaviorSubject, Observable } from 'rxjs';
import { dateConverter, dateTimeConverter, decimalConverter } from "../../logic/converter";
import { AdwService } from '../../services/adw.service';

const utils = Utils;

interface PageEvent {
  first: number;
  rows: number;
  page: number;
  pageCount: number;
}

@Component({
  selector: 'app-db-sample',
  templateUrl: './db-sample.component.html',
  styleUrls: ['./db-sample.component.scss']
})
export class DbSampleComponent implements OnInit {
  readonly decimalConverter = decimalConverter;
  readonly dateTimeConverter = dateTimeConverter;
  readonly dateConverter = dateConverter;

  @ViewChild('dt') dt: Table;

  initialized$: Observable<boolean>;

  metaKey: boolean = true;

  get products(): DbSet<Product> {
    return this.adwService.dbSet;
  }

  get productsCount(): number {
    return this.adwService.dbSet.totalCount;
  }

  get selectedProduct() {
    return this.products.currentItem;
  }

  set selectedProduct(v: Product) {
    this.products.currentItem = v;
  }

  constructor(
    private adwService: AdwService,
    readonly fb: UntypedFormBuilder
    // private cdRef: ChangeDetectorRef
  ) { }

  async ngOnInit() {
    this.productForm = this.fb.group({
      Name: [null, []],
      ListPrice: [null, []],
      SellStartDate: [null, []],
    });
    const initialized = new BehaviorSubject<boolean>(false);
    this.initialized$ = initialized;
    await this.adwService.initPromise;
    initialized.next(true);

    await this.adwService.loadStaticData();
    await this.adwService.load(0, this.pageSize);
  }

  pageIndex: number = 0;
  pageSize: number = 50;
  offset: number = 0;


  customSort(event: SortEvent) {
    const field = event.field;
    const order = event.order;
    //console.log("sort", event);
    this.adwService.sortChanged(field, order).finally(() => {
      this.pageIndex = 0;
      this.offset = 0;
    });
  }

  onPageChange(event: PaginatorState) {
    this.offset = event.first;
    this.pageSize = event.rows;
    this.pageIndex = event.page;
    this.adwService.pageChanged(this.pageIndex, this.pageSize);
  }

  onRowEditInit(product: Product) {
    product._aspect.beginEdit()
  }

  onRowEditSave(product: Product) {
    if (!product._aspect.getIsHasErrors()) {
      const isSuccess = product._aspect.endEdit();
    }
  }

  onRowEditCancel(product: Product, index: number) {
    product._aspect.cancelEdit();
  }

  // currently not used
  /*
  strToDate(str: string) {
    if (utils.check.isNt(str)) {
      return null;
    }
    return utils.dates.strToDate(str, 'YYYY-MM-DD');
  }
  */

  get currentProduct(): Product {
    return this.adwService.currentItem;
  }
  get isHasChanges() {
    return this.adwService.isHasChanges;
  }
  onPreviousProduct() {
    this.adwService.dbSet.movePrev();
  }
  onNextProduct() {
    this.adwService.dbSet.moveNext();
  }
  onReject() {
    this.adwService.dbSet.cancelEdit();
    return this.adwService.dbContext.rejectChanges();
  }
  onSubmit() {
    this.adwService.dbSet.endEdit();
    return this.adwService.dbContext.submitChanges();
  }

  //#region Forms

  public productForm: UntypedFormGroup;

  //#endregion

}
