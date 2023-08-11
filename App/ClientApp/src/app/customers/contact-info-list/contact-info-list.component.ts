import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { ContactInfo } from '../customer.resolver';
import { faTrash, faPencil, faPhone, faFax, faAt, faGlobe } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-contact-info-list',
  templateUrl: './contact-info-list.component.html',
  styleUrls: ['./contact-info-list.component.css'],
})
export class ContactInfoListComponent {
  @Input({ required: true }) public contactInfo!: ContactInfo[];
  @Output() public onDeleteContactInfo = new EventEmitter<ContactInfo>();
  @Output() public onEditContactInfo = new EventEmitter<ContactInfo>();
  @Output() public onAddContactInfo = new EventEmitter<void>();
  public faTrash = faTrash;
  public faPencil = faPencil;
  public faPhone = faPhone;
  public faFax = faFax;
  public faAt = faAt;
  public faGlobe = faGlobe;
}
