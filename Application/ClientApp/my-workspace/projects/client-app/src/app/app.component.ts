import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Test Application';
  menuItems: MenuItem[] | undefined;

  constructor(
  ) { }

  async ngOnInit() {
    this.menuItems = [
      {
        label: 'Samples',
        icon: 'pi pi-palette',
        items: [
          {
            label: 'Database Sample',
            route: '/db'
          },
          {
            label: 'Tree Sample',
            route: '/tree'
          },
          {
            label: 'About',
            route: '/about'
          }
        ]
      }
    ];
  }
}
