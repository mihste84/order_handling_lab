import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CustomerPageComponent } from './customer-page/customer-page.component';
import { EditCustomerPageComponent } from './edit-customer-page/edit-customer-page.component';
import { customerResolver } from './customer.resolver';

const routes: Routes = [
  {
    path: '',
    component: CustomerPageComponent,
  },
  {
    path: ':id',
    component: EditCustomerPageComponent,
    resolve: { customer: customerResolver },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CustomerRoutingModule {}
