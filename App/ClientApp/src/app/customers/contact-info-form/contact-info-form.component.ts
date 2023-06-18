import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { contactInfoValidator } from '../validators/contact-info.validator';
import { CacheService } from '../../shared/services/cache.service';
import { ContactInfo } from '../customer.resolver';

export interface ContactInfoModel {
  id?: number;
  customerId?: number;
  type: string;
  value: string;
  prefix?: string;
}

@Component({
  selector: 'app-contact-info-form',
  templateUrl: './contact-info-form.component.html',
  styleUrls: ['./contact-info-form.component.css'],
})
export class ContactInfoFormComponent {
  @Input() contactInfo?: ContactInfo;
  @Output() public onAddContactInfo = new EventEmitter<ContactInfoModel>();

  public form: FormGroup = this.fb.group({
    type: ['Phone', [Validators.required]],
    prefix: [''],
    value: ['', [Validators.required, contactInfoValidator('Phone')]],
  });
  constructor(private fb: FormBuilder, public cache: CacheService) {}

  public onSubmit(): void {
    if (this.form.invalid) return;
    let value = { ...this.form.value } as ContactInfoModel;
    value.type === 'Phone' || value.type === 'Fax' ? value.prefix + value.value : value.value,
      this.onAddContactInfo.emit(value);
  }

  public onChangeContactInfoType(type: string, control: AbstractControl<any, any> | null): void {
    control?.setValue('');
    control?.setValidators([Validators.required, contactInfoValidator(type)]);
    control?.updateValueAndValidity();
  }
}
