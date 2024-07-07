import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import * as FOLDER_DB from "../../db/folderDB";
import { FolderService, IFileSystemObject } from '../../services/folder.service';

@Component({
  selector: 'app-tree-sample',
  templateUrl: './tree-sample.component.html',
  styleUrls: ['./tree-sample.component.scss']
})
export class TreeSampleComponent implements OnInit {
  items$: Observable<IFileSystemObject[]>;
  count$: Observable<number>;
  initialized$: Observable<boolean>;

  constructor(
    private folderService: FolderService,
  ) { }

  async ngOnInit() {
    const initialized = new BehaviorSubject<boolean>(false);
    this.initialized$ = initialized;
    await this.folderService.initPromise;
    initialized.next(true);

    this.items$ = this.folderService.items$;
    this.count$ = this.folderService.count$;
    this.folderService.loadRootFolder();
  }

  onItemClicked(event: Event, item: FOLDER_DB.IFileSystemObject) {
    (item as any).exProp.click();
    // alert("Clicked: " + item.Key);
    event?.preventDefault();
  }

  //#endregion

}
