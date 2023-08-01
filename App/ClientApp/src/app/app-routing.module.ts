import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './security/login/login.component';
import { LogoutComponent } from './security/logout/logout.component';
import { WelcomePageComponent } from './shared/components/welcome-page/welcome-page.component';
import { NotFoundPageComponent } from './shared/components/not-found-page/not-found-page.component';
import { redirectAuthenticatedGuard, redirectWelcomeGuard } from './security/authentication.guard';

const authenticatedRoutes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'home',
    loadChildren: () => import('./home/home.module').then((m) => m.HomeModule),
  },
  {
    path: 'customers',
    loadChildren: () => import('./customers/customers.module').then((m) => m.CustomersModule),
  },
  {
    path: 'orders',
    loadChildren: () => import('./orders/orders.module').then((m) => m.OrdersModule),
  },
  {
    path: 'master-data',
    loadChildren: () => import('./master-data/master-data.module').then((m) => m.MasterDataModule),
  },
];

const routes: Routes = [
  {
    path: '',
    redirectTo: '/app/home',
    pathMatch: 'full',
  },
  {
    path: 'app',
    canActivate: [() => redirectAuthenticatedGuard()],
    children: authenticatedRoutes,
  },
  {
    path: 'welcome',
    canActivate: [() => redirectWelcomeGuard()],
    component: WelcomePageComponent,
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'logout',
    component: LogoutComponent,
  },
  {
    path: '**',
    component: NotFoundPageComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { bindToComponentInputs: true })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
