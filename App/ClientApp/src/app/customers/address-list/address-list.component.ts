import { Component, Input } from '@angular/core';
import { CustomerAddress } from '../customer.resolver';
import { faCheck, faPencil, faTrash } from '@fortawesome/free-solid-svg-icons';
import { CacheService } from 'src/app/shared/services/cache.service';

@Component({
  selector: 'app-address-list',
  templateUrl: './address-list.component.html',
  styleUrls: ['./address-list.component.css'],
})
export class AddressListComponent {
  @Input({ required: true }) public customerAddresses!: CustomerAddress[];
  public faCheck = faCheck;
  public faPencil = faPencil;
  public faTrash = faTrash;

  constructor(private cache: CacheService) {}

  public getCountryName(id: number) {
    return this.cache.masterData?.countries.find((c) => c.id === id)?.name ?? 'Unknown';
  }

  public getCityName(id: number) {
    return this.cache.masterData?.cities.find((c) => c.id === id)?.name ?? 'Unknown';
  }
}
