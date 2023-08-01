import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { CityDto, MasterDataDto } from '../../shared/resolvers/master-data.resolver';
import { CacheService } from '../../shared/services/cache.service';
import { contactInfoValidator } from '../validators/contact-info.validator';

enum FormSection {
  CustomerInfo = 1,
  AddressInfo = 2,
  ContactInfo = 3,
}

export interface NewCustomer {
  customerType: string;
  firstName: string;
  lastName: string;
  middleName: string;
  ssn: string;
  name: string;
  code: string;
  isCompany: boolean;
  customerAddresses: {
    address: string;
    isPrimary: boolean;
    postArea: string;
    zipCode: string;
    countryId: number;
    cityId: number;
  }[];
  contactInfo: {
    type: string;
    prefix?: string;
    value: string;
  }[];
}

@Component({
  selector: 'app-new-customer-form',
  templateUrl: './new-customer-form.component.html',
  styleUrls: ['./new-customer-form.component.css'],
})
export class NewCustomerFormComponent {
  @Output() public onAddNewCustomer = new EventEmitter<NewCustomer>();
  public FormSection = FormSection;
  public currentSection = FormSection.CustomerInfo;
  public isCompany: boolean = false;
  public form: FormGroup = this.fb.group({
    customerType: 'person',
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    middleName: ['', [Validators.maxLength(50)]],
    ssn: ['', [Validators.required, Validators.pattern(/^\d{8}-[a-zA-Z0-9]{4,5}$/)]],
    name: [{ value: '', disabled: true }, [Validators.required, Validators.maxLength(50)]],
    code: [{ value: '', disabled: true }, [Validators.required, Validators.maxLength(20)]],
    isCompany: [false, [Validators.required]],
    customerAddresses: this.fb.array([this.addAddress(true)]),
    contactInfo: this.fb.array([this.addContactInfo('Phone')]),
  });
  public selectedCities: CityDto[] = [];

  get addresses(): FormArray {
    return this.form.get('customerAddresses') as FormArray;
  }

  get contactInfo(): FormArray {
    return this.form.get('contactInfo') as FormArray;
  }

  constructor(private fb: FormBuilder, public cache: CacheService) {}

  public onCustomerTypeChange(customerType: string): void {
    this.isCompany = customerType === 'company';
    !this.isCompany ? this.enablePersonControls() : this.enableCompanyControls();
  }

  public onSubmit(): void {
    if (this.form.invalid) return;
    let value = { ...this.form.value } as NewCustomer;
    value.contactInfo = value.contactInfo.map((_: { type: string; prefix?: string; value: string }) => ({
      type: _.type,
      value: _.type === 'Phone' || _.type === 'Fax' ? _.prefix + _.value : _.value,
    }));
    this.onAddNewCustomer.emit(value);
  }

  public onPreviousPage(): void {
    if (this.currentSection > 1) this.currentSection = this.currentSection - 1;
  }

  public onNextPage(): void {
    if (this.currentSection < 3) this.currentSection = this.currentSection + 1;
  }

  public onCountryChange(countryId: number, control: AbstractControl<any, any> | null): void {
    this.selectedCities = this.cache.masterData?.cities.filter((_) => _.countryId == countryId) ?? [];
    control?.setValue('');
    control?.enable();
  }

  public onAddSecondaryAddress(): void {
    if (this.addresses.length >= 3) return;
    this.addresses.push(this.addAddress(false));
  }

  public onRemoveSecondaryAddress(index: number): void {
    this.addresses.removeAt(index);
  }

  public onAddContactInfo(): void {
    if (this.contactInfo.length >= 5) return;
    this.contactInfo.push(this.addContactInfo('Phone'));
  }

  public onRemoveContactInfo(index: number): void {
    this.contactInfo.removeAt(index);
  }

  public onChangeContactInfoType(type: string, control: AbstractControl<any, any> | null): void {
    control?.setValue('');
    control?.setValidators([Validators.required, contactInfoValidator(type)]);
    control?.updateValueAndValidity();
  }

  private enableCompanyControls(): void {
    this.form.get('code')?.enable();
    this.form.get('name')?.enable();
    this.form.get('firstName')?.disable();
    this.form.get('lastName')?.disable();
    this.form.get('middleName')?.disable();
    this.form.get('ssn')?.disable();
    this.form.get('isCompany')?.patchValue(true);
  }

  private enablePersonControls(): void {
    this.form.get('code')?.disable();
    this.form.get('name')?.disable();
    this.form.get('firstName')?.enable();
    this.form.get('lastName')?.enable();
    this.form.get('middleName')?.enable();
    this.form.get('ssn')?.enable();
    this.form.get('isCompany')?.patchValue(false);
  }

  private addAddress(isPrimary: boolean): FormGroup {
    return this.fb.group({
      address: ['', [Validators.required, Validators.maxLength(200)]],
      isPrimary: [isPrimary, [Validators.required]],
      postArea: ['', [Validators.required, Validators.maxLength(50)]],
      zipCode: ['', [Validators.required, Validators.maxLength(10)]],
      countryId: ['', [Validators.required]],
      cityId: [{ value: '', disabled: true }, [Validators.required]],
    });
  }

  private addContactInfo(type: string): FormGroup {
    return this.fb.group({
      type: [type, [Validators.required]],
      prefix: [''],
      value: ['', [Validators.required, contactInfoValidator(type)]],
    });
  }
}
