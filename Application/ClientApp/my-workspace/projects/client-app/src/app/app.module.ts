import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextareaModule } from 'primeng/inputtextarea';


import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BindDirective } from 'projects/client-app/src/directives/bind.directive';
import { DataSourceDirective } from '../directives/datasource.directive';
import {
  TableJriapDirective
} from '../directives/table-jriap.directive';
import {
  TableRowJriapDirective
} from '../directives/table-row-jriap.directive';



@NgModule({
  declarations: [
    AppComponent,
    BindDirective,
    DataSourceDirective,
    TableJriapDirective,
    TableRowJriapDirective
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    TableModule,
    PaginatorModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    InputTextareaModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
