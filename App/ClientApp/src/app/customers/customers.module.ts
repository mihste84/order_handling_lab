import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerPageComponent } from './customer-page/customer-page.component';
import { CustomerRoutingModule } from './customer-routing.module';
import { CustomerTableComponent } from './customer-table/customer-table.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgxPopperjsModule } from 'ngx-popperjs';
import { NewCustomerFormComponent } from './new-customer-form/new-customer-form.component';
import { EditCustomerPageComponent } from './edit-customer-page/edit-customer-page.component';
import { RouterModule } from '@angular/router';
import { CustomerInfoFormComponent } from './customer-info-form/customer-info-form.component';
import { ContactInfoListComponent } from './contact-info-list/contact-info-list.component';
import { ContactInfoFormComponent } from './contact-info-form/contact-info-form.component';
import { AddressListComponent } from './address-list/address-list.component';
import { AddressFormComponent } from './address-form/address-form.component';

@NgModule({
  declarations: [
    CustomerPageComponent,
    CustomerTableComponent,
    NewCustomerFormComponent,
    EditCustomerPageComponent,
    CustomerInfoFormComponent,
    ContactInfoListComponent,
    ContactInfoFormComponent,
    AddressListComponent,
    AddressFormComponent,
  ],
  imports: [
    CommonModule,
    CustomerRoutingModule,
    NgxPopperjsModule,
    SharedModule,
    FormsModule,
    FontAwesomeModule,
    ReactiveFormsModule,
    RouterModule,
  ],
})
export class CustomersModule {}
