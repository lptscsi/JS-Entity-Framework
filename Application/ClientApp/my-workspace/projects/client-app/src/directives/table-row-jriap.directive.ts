import { ChangeDetectorRef, Directive, Host, OnDestroy, OnInit, Self } from "@angular/core";
import { IErrors, Utils } from "jriapp-lib";
import { DomHandler } from "primeng/dom";
import { EditableRow } from 'primeng/table';
import { TableJriapDirective } from "projects/client-app/src/directives/table-jriap.directive";
import { IEntityItem } from "projects/jriapp-lib/src/public-api";
import { Subscription } from "rxjs";


@Directive({
  selector: "[pEditableRow][error-info]",
})
export class TableRowJriapDirective implements OnInit, OnDestroy  {
  private subscription: Subscription;

  constructor(
    private cdr: ChangeDetectorRef,
    @Host() @Self() public row: EditableRow,
    public tableJriap: TableJriapDirective
  ) {
    this.subscription = new Subscription();
    this.uniqueID = Utils.core.getNewID("dt-primeng");
  }

  private uniqueID: string;

  ngOnInit() {
    this.subscription.add(this.tableJriap.errors$.subscribe({
      next: (errs) => {
        const item: IEntityItem = this.row.data;
        const itemErrs: IErrors = !errs ? null : errs[item._key];
        if (!!itemErrs) {
          DomHandler.addClass(this.row.el.nativeElement, 'ng-invalid');
          // instead - it is better to add message service here or tooltip service
          // console.log("item errors", itemErrs);
        }
        else {
          DomHandler.removeClass(this.row.el.nativeElement, 'ng-invalid');
        }
      },
      error: (ex) => {
        console.error(ex);
      }
    }));
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

}
