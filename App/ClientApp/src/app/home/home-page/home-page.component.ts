import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MasterDataDto } from '../../shared/resolvers/master-data.resolver';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css'],
})
export class HomePageComponent implements OnInit {
  @Input({ required: true }) public readonly masterData?: MasterDataDto;
  constructor(route: ActivatedRoute) {
    this.masterData = route.snapshot.data['masterData'];
  }
  ngOnInit(): void {
    console.log(this.masterData);
  }
}
