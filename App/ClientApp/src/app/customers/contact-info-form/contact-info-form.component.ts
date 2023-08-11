import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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
export class ContactInfoFormComponent implements OnInit {
  @Input() contactInfo?: ContactInfo;
  @Output() public onFormSubmit = new EventEmitter<ContactInfoModel>();

  public form!: FormGroup;
  constructor(private fb: FormBuilder, public cache: CacheService) {}

  public onSubmit(): void {
    if (this.form.invalid) return;
    let value = { ...this.form.value } as ContactInfoModel;
    value.value = value.type === 'Phone' || value.type === 'Fax' ? value.prefix + value.value : value.value;
    this.onFormSubmit.emit(value);
  }

  public onChangeContactInfoType(type: string, control: AbstractControl<any, any> | null): void {
    control?.setValue('');
    control?.setValidators([Validators.required, contactInfoValidator(type)]);
    control?.updateValueAndValidity();
  }

  ngOnInit(): void {
    const intialType = this.contactInfo?.type ?? 'Phone';
    this.form = this.fb.group({
      id: [this.contactInfo?.id],
      customerId: [this.contactInfo?.customerId],
      rowVersion: [this.contactInfo?.rowVersion],
      type: [intialType, [Validators.required]],
      prefix: [this.contactInfo?.prefix ?? ''],
      value: [this.contactInfo?.value ?? '', [Validators.required, contactInfoValidator(intialType)]],
    });
  }
}
