import { Injectable } from '@angular/core';
import { MasterDataDto } from '../resolvers/master-data.resolver';
import { SearchCustomer } from 'src/app/customers/customer-page/customer-page.component';
import { DynamicSearchQuery } from '../models/dynamic-search-query';

export interface CustomerPage {
  searchModel: DynamicSearchQuery;
  customers: SearchCustomer[];
  totalCustomers: number;
}

@Injectable({
  providedIn: 'root',
})
export class CacheService {
  public customerPage?: CustomerPage;

  get masterData(): MasterDataDto | undefined {
    var masterData = sessionStorage.getItem('masterData');
    if (!masterData) return;
    return JSON.parse(masterData);
  }

  set masterData(masterData: MasterDataDto | undefined) {
    sessionStorage.setItem('masterData', JSON.stringify(masterData));
  }
}
