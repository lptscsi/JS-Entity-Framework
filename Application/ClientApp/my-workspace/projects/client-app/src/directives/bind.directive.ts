import { ChangeDetectorRef, Directive, Inject, Input, OnChanges, OnDestroy, OnInit, Optional, SimpleChanges } from "@angular/core";
import { NgControl } from "@angular/forms";
import { BaseObject, Binding, getBindingOptions, IValidatable, IValidationInfo, TBindingInfo, Utils } from "jriapp-lib";
import { IConverter } from "projects/client-app/src/logic/converter";
import { Subscription } from "rxjs";
import { DATASOURCE, DataSourceDirective } from "./datasource.directive";

const queue = Utils.queue;
class BindTarget extends BaseObject implements IValidatable {
  private readonly ngControl: NgControl;
  private readonly changeDetectorRef: ChangeDetectorRef;
  private readonly subscription: Subscription = new Subscription();
  private _validationErrors: IValidationInfo[];

  constructor(control: NgControl,
    changeDetectorRef: ChangeDetectorRef) {
    super();
    this.ngControl = control;
    this.changeDetectorRef = changeDetectorRef;
    this._validationErrors = [];

    if (!!this.ngControl?.valueChanges) {
      this.subscription.add(this.ngControl.valueChanges.subscribe((v) => {
        this.objEvents.raiseProp("value");
      }));
    }
  }

  get value(): any {
    return this.ngControl.control.value;
  }

  set value(v: any) {
    if (this.value !== v) {
      this.ngControl.control.setValue(v);
      // probably not needed here, because NgControl handles this (but it does not hurt)
      this.changeDetectorRef.markForCheck();
    }
  }

  override dispose(): void {
    this.subscription.unsubscribe();
    super.dispose();
  }

  get validationErrors(): IValidationInfo[] {
    return this._validationErrors;
  }

  set validationErrors(v: IValidationInfo[]) {
    this._validationErrors = v;
    if (this._validationErrors.length > 0) {
      const errors = this._validationErrors.reduce((prev, e) => {
        if (!prev[e.fieldName]) {
          prev[e.fieldName] = [];
        }
        prev[e.fieldName] = [...prev[e.fieldName], ...e.errors]
        return prev;
      }, {});

      this.ngControl.control.setErrors(errors);
    }
    else {
      this.ngControl.control.setErrors(null);
    }
  }
}

export type TBindingMode = "OneTime" | "OneWay" | "TwoWay" | "BackWay";

@Directive({
  selector: "[bind]"
})
export class BindDirective implements OnDestroy, OnInit, OnChanges {
  private readonly subscription: Subscription = new Subscription();
  private readonly ngControl: NgControl = null;
  private readonly dataSourceDirective: DataSourceDirective | null;

  private target: BindTarget | null = null;
  private binding: Binding | null = null;

  get dataSource(): any {
    return this.dataSourceDirective?.dataSource ?? null;
  }

  @Input('bind') path: string = "";

  @Input('bindMode') mode: TBindingMode = "OneWay";

  @Input('converter') converter: IConverter = null;

  @Input('converterParam') converterParam: any = null;

  constructor(
    @Optional() @Inject(NgControl) control: NgControl,
    @Inject(DATASOURCE) dataSourceDirective: DataSourceDirective,
    @Optional() @Inject(ChangeDetectorRef) private changeDetectorRef?: ChangeDetectorRef | null,
  ) {
    if (!control) {
      throw new Error(
        `NgControl not injected in ${this.constructor.name}!\n Use [(ngModel)] or [formControl] or formControlName for correct work.`,
      );
    }

    this.ngControl = control;
    this.dataSourceDirective = dataSourceDirective ?? null;
  }

  ngOnInit() {
    this.target = new BindTarget(this.ngControl, this.changeDetectorRef);
    queue.enque(() => {
      this.binding = this.bindDataSource();
      if (!!this.dataSourceDirective) {
        this.subscription.add(this.dataSourceDirective.changes$.subscribe(() => {
          if (!!this.binding) {
            this.binding.source = this.dataSource;
          }
        }));
      }
    });
  }
 
  ngOnDestroy() {
    this.binding?.dispose();
    this.binding = null;

    this.target?.dispose();
    this.target = null;
    this.subscription.unsubscribe();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.target || !this.binding) {
      return;
    }
    if (changes["converterParam"]) {
      this.binding.param = this.converterParam;
    }

    if (changes["converter"]) {
      this.binding.converter = this.converter;
    }
  }

  public get controlName(): string | null {
    return this.ngControl?.name?.toString() ?? null;
  }

  private bindDataSource(): Binding {
    let bindInfo: TBindingInfo = {
      targetPath: "value",
      sourcePath: this.path,
      source: this.dataSource,
      target: this.target,
      mode: this.mode,
      converter: this.converter,
      converterParam: this.converterParam
    };
    const bindOptions = getBindingOptions(bindInfo, this.target, this.dataSource);
    
    return new Binding(bindOptions);
  }
}
