import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AppRoutingModule } from './app-routing.module'; // Import the routing module
//import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http'; // Import provideHttpClient and withInterceptorsFromDi

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent // Ensure this is declared
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
