import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeModule } from './home/home.module';
import { OrdersModule } from './orders/orders.module';
import { CustomersModule } from './customers/customers.module';
import { MasterDataModule } from './master-data/master-data.module';
import { SharedModule } from './shared/shared.module';
import { AuthUserService } from './security/auth-user.service';
import { SecurityModule } from './security/security.module';
import { ErrorInterceptor } from './shared/interceptors/error.interceptor';
import { ApiInterceptor } from './shared/interceptors/api.interceptor';
import { LayoutModule } from '@angular/cdk/layout';

export function appInitFactory(appUserService: AuthUserService) {
  return async (): Promise<any> => {
    return appUserService.init();
  };
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HomeModule,
    OrdersModule,
    CustomersModule,
    MasterDataModule,
    SharedModule,
    HttpClientModule,
    SecurityModule,
    LayoutModule,
  ],
  providers: [
    AuthUserService,
    {
      provide: APP_INITIALIZER,
      useFactory: appInitFactory,
      deps: [AuthUserService],
      multi: true,
    },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ApiInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
