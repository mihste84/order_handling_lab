import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomePageComponent } from './home-page/home-page.component';
import { masterDataResolver } from '../shared/resolvers/master-data.resolver';

const routes: Routes = [
  {
    path: '',
    component: HomePageComponent,
    resolve: { masterData: () => masterDataResolver() },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HomeRoutingModule {}
