import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { SearchTypes } from '../../shared/components/dynamic-search/dynamic-search.component';
import { CacheService } from '../../shared/services/cache.service';
import { DynamicSearchQuery } from '../../shared/models/dynamic-search-query';
import { SearchResults, SqlResult } from '../../shared/models/types';
import { firstValueFrom } from 'rxjs';
import { faPlus, faTimes } from '@fortawesome/free-solid-svg-icons';
import { SortResult } from '../../shared/components/table-header/table-header.component';
import { NewCustomer } from '../new-customer-form/new-customer-form.component';
import { AuthUserService } from '../../security/auth-user.service';
import { NotificationService } from '../../shared/services/notification.service';
import { ConfirmComponent } from '../../shared/components/confirm/confirm.component';
import { ModalComponent } from '../../shared/components/modal/modal.component';

export interface SearchCustomer {
  id: number;
  firstName: string;
  lastName: string;
  middleName: string;
  ssn: string;
  code: string;
  name: string;
  active: boolean;
  created: Date;
  createdBy: string;
  updated: Date;
  updatedBy: string;
}

@Component({
  selector: 'app-customer-page',
  templateUrl: './customer-page.component.html',
  styleUrls: ['./customer-page.component.css'],
})
export class CustomerPageComponent implements OnInit, OnDestroy {
  public searchModel: DynamicSearchQuery = new DynamicSearchQuery({
    searchItems: [
      { name: 'FirstName', distplayName: 'First name', type: SearchTypes.Text },
      { name: 'LastName', distplayName: 'Last name', type: SearchTypes.Text },
      { name: 'MiddleName', distplayName: 'Middle name', type: SearchTypes.Text },
      { name: 'Ssn', distplayName: 'SSN', type: SearchTypes.Text },
      { name: 'Code', distplayName: 'Org. code', type: SearchTypes.Text },
      { name: 'Name', distplayName: 'Org. name', type: SearchTypes.Text },
      {
        name: 'Active',
        distplayName: 'Is active',
        type: SearchTypes.Number,
        listValues: [
          { value: '1', displayName: 'Yes' },
          { value: '0', displayName: 'No' },
        ],
        isValueSelectable: true,
      },
      { name: 'Phone', distplayName: 'Phone', type: SearchTypes.Number, handleAutomatically: false },
      { name: 'Email', distplayName: 'E-mail', type: SearchTypes.Text, handleAutomatically: false },
      {
        name: 'CityId',
        distplayName: 'City',
        type: SearchTypes.Number,
        listValues: this.cache.masterData?.cities.map((c) => ({ value: c.id.toString(), displayName: c.name })),
        isValueSelectable: true,
        handleAutomatically: false,
      },
      {
        name: 'CountryId',
        distplayName: 'Country',
        type: SearchTypes.Number,
        listValues: this.cache.masterData?.countries.map((c) => ({ value: c.id.toString(), displayName: c.name })),
        isValueSelectable: true,
        handleAutomatically: false,
      },
    ],
  });
  public faPlus = faPlus;
  public faTimes = faTimes;
  public customers: SearchCustomer[] = [];
  public totalCustomers: number = 0;
  @ViewChild('sharedModal') private sharedModal?: ModalComponent;

  constructor(
    private http: HttpClient,
    private cache: CacheService,
    private auth: AuthUserService,
    private notifications: NotificationService
  ) {}

  public onSortCallback(sortResult: SortResult) {
    this.searchModel.orderBy = sortResult.columnName;
    this.searchModel.orderDirection = sortResult.direction;
  }

  public async onPageChangeCallback(page: number) {
    if (page < 1) return;
    await this.search(page);
  }

  public async onAddNewCustomerCallback(customer: NewCustomer, modal: ModalComponent) {
    const req = this.http.post<SqlResult>('customer', customer);
    const res = await firstValueFrom(req);
    const newCustomer: SearchCustomer = {
      id: res.id,
      firstName: customer.firstName,
      lastName: customer.lastName,
      middleName: customer.middleName,
      ssn: customer.ssn,
      code: customer.code,
      name: customer.name,
      active: true,
      created: new Date(),
      createdBy: this.auth.user?.userName ?? 'System',
      updated: new Date(),
      updatedBy: this.auth.user?.userName ?? 'System',
    };
    this.customers.unshift(newCustomer);
    this.totalCustomers++;
    this.notifications.addNotification(`Customer with ID "${res.id}" was added successfully.`, 'Customer added');
    modal.hideModal();
  }

  public async search(overridePage?: number) {
    if (overridePage) this.searchModel.page = overridePage - 1;
    const params = this.searchModel.getQueryParams();
    const req = this.http.get<SearchResults<SearchCustomer>>('customer/search', { params });
    const first = await firstValueFrom(req);
    this.customers = first.items ?? [];
    this.totalCustomers = first.totalCount ?? 0;
  }

  public async onDeleteCustomerCallback(customer: SearchCustomer) {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(
      ConfirmComponent,
      [
        { name: 'message', value: `Are you sure you want to delete customer with ID "${customer.id}"?` },
        { name: 'returnObject', value: customer },
      ],
      `Delete customer #${customer.id}`
    );
    ref?.instance.onConfirm.subscribe(async (c: SearchCustomer) => {
      await this.onDeleteCustomer(c);
      this.sharedModal!.hideModal();
    });

    ref?.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }

  private async onDeleteCustomer(customer: SearchCustomer) {
    const req = this.http.delete(`customer/${customer.id}`);
    await firstValueFrom(req);
    this.customers = this.customers.filter((c) => c.id !== customer.id);
    this.notifications.addNotification(
      `Customer with ID "${customer.id}" was deleted successfully.`,
      'Customer deleted'
    );
  }

  ngOnDestroy(): void {
    this.cache.customerPage = {
      searchModel: this.searchModel,
      customers: this.customers,
      totalCustomers: this.totalCustomers,
    };
  }
  ngOnInit(): void {
    if (!this.cache.customerPage) return;
    this.searchModel = this.cache.customerPage.searchModel;
    this.customers = this.cache.customerPage.customers;
    this.totalCustomers = this.cache.customerPage.totalCustomers;
  }
}
