import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AboutComponent } from './about/about.component';
import { DbSampleComponent } from './db-sample/db-sample.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { TreeSampleComponent } from './tree-sample/tree-sample.component';


const routes: Routes = [
  {
    path: "",
    redirectTo: "/db",
    pathMatch: "full"
  },
  {
    path: "db",
    component: DbSampleComponent
  },
  {
    path: "tree",
    component: TreeSampleComponent
  },
  {
    path: "about",
    component: AboutComponent
  },
  {
    path: "**",
    component: PageNotFoundComponent
  }

];



@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
