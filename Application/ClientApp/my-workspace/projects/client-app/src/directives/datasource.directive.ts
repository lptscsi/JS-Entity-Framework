import { Directive, forwardRef, InjectionToken, Input, OnChanges, OnDestroy, SimpleChanges } from "@angular/core";
import { Subject } from "rxjs";

export const DATASOURCE = new InjectionToken<DataSourceDirective>(
  'jriapp framework datasource'
);

export const datasourceBinding: any = {
  provide: DATASOURCE,
  useExisting: forwardRef(() => DataSourceDirective)
};

@Directive({
  selector: "[datasource]",
  providers: [datasourceBinding]
})
export class DataSourceDirective implements OnDestroy, OnChanges  {

  readonly changes$ = new Subject<void>();

  @Input('datasource') dataSource: any;

  constructor(
  ) {
  }

  ngOnDestroy() {
    this.changes$.complete();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.changes$.next();
  }
}
