import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';

export interface CustomerPerson {
  firstName?: string;
  lastName?: string;
  middleName?: string;
  ssn?: string;
}

export interface CustomerCompany {
  code?: string;
  name?: string;
}

export interface Customer extends CustomerPerson, CustomerCompany {
  id: number;
  customerId: number;
  active: boolean;
  created: Date;
  createdBy: string;
  updated: Date;
  updatedBy: string;
  rowVersion: any;
  customerRowVersion: any;
  customerAddresses: CustomerAddress[];
  customerContactInfos: ContactInfo[];
}

export interface CustomerAddress {
  id: number;
  customerId: number;
  isPrimary: boolean;
  address: string;
  postArea: string;
  zipCode: string;
  countryId: number;
  cityId: number;
  created: Date;
  createdBy: string;
  updated: Date;
  updatedBy: string;
  rowVersion: any;
}

export interface ContactInfo {
  id: number;
  customerId: number;
  type: string;
  value: string;
  prefix?: string;
  rowVersion: any;
  isEditing?: boolean;
}

export const customerResolver: ResolveFn<Customer> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
  http = inject(HttpClient)
): Observable<Customer> => {
  const id = route.params['id'];
  if (!id) throw new Error('No id provided');
  return http.get<Customer>('customer?type=id&value=' + id);
};
