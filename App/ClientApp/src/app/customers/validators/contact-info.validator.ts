import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
export function contactInfoValidator(type: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    switch (type) {
      case 'Fax':
      case 'Phone':
        if (control.value && !doesPhoneMatch(control.value.toString())) return { invalidPhone: true };
        break;
      case 'E-mail':
        if (control.value && !doesEmailMatch(control.value.toString())) return { invalidEmail: true };
        break;
      case 'Website':
        if (control.value && !doesUrlMatch(control.value.toString())) return { invalidUrl: true };
        break;
    }
    return null;
  };
}

function doesPhoneMatch(value: string): boolean {
  return value.match(/^\d{8,15}$/) !== null;
}

function doesEmailMatch(value: string): boolean {
  return value.match(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/) !== null;
}

function doesUrlMatch(value: string): boolean {
  return (
    value.match(/^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,}$/) !==
    null
  );
}
