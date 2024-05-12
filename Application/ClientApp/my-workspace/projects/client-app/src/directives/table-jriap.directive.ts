import { ChangeDetectorRef, Directive, Host, Input, OnChanges, OnDestroy, Optional, Self, SimpleChanges } from "@angular/core";
import { DbSet, IErrors, Utils } from "jriapp-lib";
import { Table } from 'primeng/table';
import { IEntityItem } from "projects/jriapp-lib/src/public-api";
import { BehaviorSubject, Observable } from "rxjs";


@Directive({
    selector: "p-table[dbset]",
})
export class TableJriapDirective implements OnChanges, OnDestroy  {
  @Input({ alias: 'dbset', required: true }) dbSet: DbSet

  constructor(
    private cdr: ChangeDetectorRef,
    @Host() @Self() @Optional() public dt: Table) {

    this.uniqueID = Utils.core.getNewID("dt-primeng");
    this._dbSetErrorsSubject = new BehaviorSubject(null);

    (function (old) {
      dt.saveRowEdit = function (rowData: any, rowElement: HTMLTableRowElement) {
        // if row has validation errors - then don't stop editing
        if (rowData && (rowData as IEntityItem)._aspect.getIsHasErrors()) {
          //NOOP
        }
        else {
          // invoke the original method
          return old.apply(this, arguments);
        }
      };
    })(dt.saveRowEdit);

  }

  private uniqueID: string;
  private _dbSetErrorsSubject: BehaviorSubject<{
    [key: string]: IErrors;
  } | null>;

  get datakey(): string {
    return this.dt.dataKey;
  }

  get errors$() {
    return this._dbSetErrorsSubject as Observable<{
      [key: string]: IErrors;
    } | null>;
  }

  ngOnDestroy(): void {
    if (!!this.dbSet) {
      this.dbSet.objEvents.offNS(this.uniqueID);
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['dbSet']) {
      this.updateDbSet();
    }
  }

  private updateDbSet() {
    if (!this.dt) {
      return;
    }

    if (!!this.dbSet) {
      this.dbSet.objEvents.offNS(this.uniqueID);
    }

    this.dbSet.addOnCollChanged((s, a) => {
      this.dt.value = this.dbSet.items;
      this.cdr.markForCheck();
    }, this.uniqueID);

    this.dbSet.addOnBeginEdit((s, a) => {
      this.dt.initRowEdit(a.item);
    }, this.uniqueID);

    this.dbSet.addOnEndEdit((s, a) => {
      if (a.isCanceled) {
        this.dt.cancelRowEdit(a.item);
      }
      else {
        this.dt.cancelRowEdit(a.item);
      }
    }, this.uniqueID);


    this.dbSet.addOnErrorsChanged((s, a) => {
      const itemsWithErrors = this.dbSet.errors.getItemsWithErrors();
      const errs: { [key: string]: IErrors } = Object.create(null);
      let cnt = 0;
      for (var item of itemsWithErrors) {
        const itemErrors: IErrors = this.dbSet.errors.getErrors(item);
        errs[item._key] = itemErrors;
        ++cnt;
      }
      this._dbSetErrorsSubject.next(cnt > 0 ? errs : null);
    }, this.uniqueID);
  }
}
