import { Component, EventEmitter, Input, Output } from '@angular/core';
import { SearchCustomer } from '../customer-page/customer-page.component';
import { SortResult } from '../../shared/components/table-header/table-header.component';
import { faEllipsisVertical } from '@fortawesome/free-solid-svg-icons';
import { NgxPopperjsContentComponent } from 'ngx-popperjs';

@Component({
  selector: 'app-customer-table',
  templateUrl: './customer-table.component.html',
  styleUrls: ['./customer-table.component.css'],
})
export class CustomerTableComponent {
  @Input({ required: true }) public customers: SearchCustomer[] = [];
  @Input() public currentSortColumnName?: string;
  @Input() public currentSortDirection?: string;
  @Output() public onSort = new EventEmitter<SortResult>();
  @Output() public onDeleteCustomer = new EventEmitter<SearchCustomer>();

  public faEllipsisVertical = faEllipsisVertical;
  public tableHeaders = [
    { name: 'Id', title: '#', isSortable: true },
    { name: 'FirstName', title: 'First name', isSortable: true },
    { name: 'LastName', title: 'Last name', isSortable: true },
    { name: 'MiddleName', title: 'Middle name', isSortable: true },
    { name: 'Ssn', title: 'SSN', isSortable: true },
    { name: 'Code', title: 'Org. code', isSortable: true },
    { name: 'Name', title: 'Org. name', isSortable: true },
    { name: 'Active', title: 'Is active?', isSortable: true },
    { name: 'Created', title: 'Created date', isSortable: true },
    { name: 'CreatedBy', title: 'Created by', isSortable: true },
    { name: 'Updated', title: 'Updated date', isSortable: true },
    { name: 'UpdatedBy', title: 'Updated by', isSortable: true },
  ];

  public onRowClick(action: string, item: SearchCustomer, list: NgxPopperjsContentComponent) {
    switch (action) {
      case 'Delete':
        this.onDeleteCustomer.emit(item);
        break;
    }

    list.hide();
  }
}
