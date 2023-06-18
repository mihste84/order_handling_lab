import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Customer, CustomerCompany, CustomerPerson } from '../customer.resolver';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription, debounceTime, map } from 'rxjs';

@Component({
  selector: 'app-customer-info-form',
  templateUrl: './customer-info-form.component.html',
  styleUrls: ['./customer-info-form.component.css'],
})
export class CustomerInfoFormComponent implements OnInit, OnDestroy {
  @Input({ required: true }) public customer?: CustomerPerson | CustomerCompany;
  @Input() public isCompany?: boolean;
  @Output() public onUpdateCustomerInfo = new EventEmitter<CustomerPerson | CustomerCompany>();
  public form: FormGroup = this.fb.group({});
  private subscription?: Subscription;
  constructor(private fb: FormBuilder) {}

  public onSubmit(): void {
    if (this.form.invalid) return;

    this.onUpdateCustomerInfo.emit(this.form.value);
  }

  private getCompanyControls(): FormGroup {
    const customer = this.customer as CustomerCompany;
    return this.fb.group({
      customerType: 'Company',
      name: [customer?.name, [Validators.required, Validators.maxLength(50)]],
      code: [customer?.code, [Validators.required, Validators.maxLength(20)]],
    });
  }

  private getPersonControls(): FormGroup {
    const customer = this.customer as CustomerPerson;
    return this.fb.group({
      customerType: 'Person',
      firstName: [customer?.firstName, [Validators.required, Validators.maxLength(50)]],
      lastName: [customer?.lastName, [Validators.required, Validators.maxLength(50)]],
      middleName: [customer?.middleName, [Validators.maxLength(50)]],
      ssn: [customer?.ssn, [Validators.required, Validators.pattern(/^\d{8}-[a-zA-Z0-9]{4,5}$/)]],
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  ngOnInit(): void {
    this.form = this.isCompany ? this.getCompanyControls() : this.getPersonControls();

    this.subscription = this.form.valueChanges.pipe(debounceTime(1000)).subscribe(() => {
      this.onSubmit();
    });
  }
}
