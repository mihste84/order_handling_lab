import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ContactInfo } from '../customer.resolver';
import { faTrash, faPencil } from '@fortawesome/free-solid-svg-icons';

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
}
