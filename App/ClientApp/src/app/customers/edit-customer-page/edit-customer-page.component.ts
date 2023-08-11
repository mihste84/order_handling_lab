import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ContactInfo, Customer, CustomerCompany, CustomerPerson } from '../customer.resolver';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { SqlResult } from '../../shared/models/types';
import { NotificationService } from '../../shared/services/notification.service';
import { ModalComponent } from '../../shared/components/modal/modal.component';

@Component({
  selector: 'app-edit-customer-page',
  templateUrl: './edit-customer-page.component.html',
  styleUrls: ['./edit-customer-page.component.css'],
})
export class EditCustomerPageComponent implements OnInit {
  @Input({ required: true }) public customer!: Customer;
  @ViewChild('sharedModal') private sharedModal?: ModalComponent;

  public isCompany: boolean = false;

  constructor(private router: Router, private http: HttpClient, private notifications: NotificationService) {}

  public getCustomerName(): string | undefined {
    return this.isCompany
      ? `${this.customer?.name} (${this.customer?.code})`
      : `${this.customer?.firstName} ${this.customer?.middleName ?? ''} ${this.customer?.lastName}`;
  }

  public async onUpdateCustomerInfoCallback(info: CustomerPerson | CustomerCompany): Promise<void> {
    const body = {
      id: this.customer?.id,
      isActive: this.customer?.active,
      rowVersion: this.customer?.rowVersion,
      isCompany: this.isCompany,
      ...info,
    };
    const req = this.http.put<SqlResult>('customer', body);
    const res = await firstValueFrom(req);
    this.customer.rowVersion = res.rowVersion;
    this.notifications.addNotification(`Customer info updated successfully.`, 'Customer info updated');

    if (this.isCompany) {
      const cInfo = info as CustomerCompany;
      this.customer.name = cInfo.name;
      this.customer.code = cInfo.code;
      this.customer.rowVersion = res.rowVersion;
    } else {
      const cInfo = info as CustomerPerson;
      this.customer.firstName = cInfo.firstName;
      this.customer.middleName = cInfo.middleName;
      this.customer.lastName = cInfo.lastName;
      this.customer.ssn = cInfo.ssn;
      this.customer.rowVersion = res.rowVersion;
    }
  }

  public async onAddContactInfoCallback() {}
  public async onDeleteContactInfoCallback(info: ContactInfo) {}
  public async onEditContactInfoCallback(info: ContactInfo) {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    // const ref = this.sharedModal.showModalWithComponent(
    //   ConfirmComponent,
    //   [
    //     { name: 'message', value: `Are you sure you want to delete customer with ID "${customer.id}"?` },
    //     { name: 'returnObject', value: customer },
    //   ],
    //   `Delete customer #${customer.id}`
    // );
    // ref?.instance.onConfirm.subscribe(async (c: SearchCustomer) => {
    //   await this.onDeleteCustomer(c);
    //   this.sharedModal!.hideModal();
    // });

    // ref?.instance.onCancel.subscribe(() => {
    //   this.sharedModal!.hideModal();
    // });
  }

  ngOnInit(): void {
    if (!this.customer) this.router.navigate(['/app/customers']);
    this.isCompany = !!this.customer?.code;
  }
}
