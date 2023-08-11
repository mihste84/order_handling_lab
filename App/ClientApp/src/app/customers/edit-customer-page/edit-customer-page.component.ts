import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ContactInfo, Customer, CustomerCompany, CustomerPerson } from '../customer.resolver';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { SqlResult } from '../../shared/models/types';
import { NotificationService } from '../../shared/services/notification.service';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { ContactInfoFormComponent, ContactInfoModel } from '../contact-info-form/contact-info-form.component';
import { ConfirmComponent } from 'src/app/shared/components/confirm/confirm.component';

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

  public async onAddContactInfoCallback() {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(ContactInfoFormComponent, undefined, 'Add contact info');
    ref?.instance.onFormSubmit.subscribe(async (model: ContactInfoModel) => {
      model.customerId = this.customer.id;
      const postUser$ = this.http.post<SqlResult>('ContactInfo', model);
      const res = await firstValueFrom(postUser$);
      if (!this.customer.customerContactInfos) this.customer.customerContactInfos = [];
      this.customer.customerContactInfos.push(this.getContactInfoFromModel(model, res));
      this.sharedModal!.hideModal();
      this.notifications.addNotification(`Contact info added successfully.`, 'Contact info added');
    });
  }
  public async onDeleteContactInfoCallback(info: ContactInfo) {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');

    const ref = this.sharedModal.showModalWithComponent(
      ConfirmComponent,
      [
        {
          name: 'message',
          value: `Are you sure you want to delete customer info with value "${info.prefix ?? ''}${info.value}"?`,
        },
        { name: 'returnObject', value: info },
      ],
      `Delete customer info`
    );

    ref?.instance.onConfirm.subscribe(async (c: ContactInfo) => {
      const deleteUser$ = this.http.delete('ContactInfo/' + c.id);
      await firstValueFrom(deleteUser$);
      this.customer.customerContactInfos = this.customer.customerContactInfos.filter((_) => _.id !== c.id);
      this.notifications.addNotification(
        `Contact info value ${c.prefix ?? ''}${c.value} deleted successfully.`,
        'Contact info deleted'
      );
      this.sharedModal!.hideModal();
    });

    ref?.instance.onCancel.subscribe(() => {
      this.sharedModal!.hideModal();
    });
  }

  public async onEditContactInfoCallback(info: ContactInfo) {
    if (!this.sharedModal) throw new Error('Shared modal is not initialized.');
    const ref = this.sharedModal.showModalWithComponent(
      ContactInfoFormComponent,
      [
        {
          name: 'contactInfo',
          value: info,
        },
      ],
      'Edit contact info'
    );
    ref?.instance.onFormSubmit.subscribe(async (model: ContactInfoModel) => {
      const putUser$ = this.http.put<SqlResult>('ContactInfo', model);
      const res = await firstValueFrom(putUser$);
      this.customer.customerContactInfos = this.customer.customerContactInfos.map((_) =>
        _.id !== info.id ? _ : this.getContactInfoFromModel(model, res)
      );
      this.sharedModal!.hideModal();
      this.notifications.addNotification(`Contact info edited successfully.`, 'Contact info edited');
    });
  }

  private getContactInfoFromModel(model: ContactInfoModel, res: SqlResult): ContactInfo {
    model.value = model.value?.replace(model.prefix ?? '', '');

    return {
      id: res.id,
      customerId: this.customer.id,
      rowVersion: res.rowVersion,
      type: model.type,
      prefix: model.prefix,
      value: model.value,
    };
  }

  ngOnInit(): void {
    if (!this.customer) this.router.navigate(['/app/customers']);
    this.isCompany = !!this.customer?.code;
  }
}
