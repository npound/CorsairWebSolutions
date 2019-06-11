import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';

import { HomeComponent } from './home/home.component';
import { NavComponent } from './nav/nav.component';
import { ContactComponent } from './contact/contact.component';
import { OpportunitiesComponent } from './opportunities/opportunities.component';
import { FooterComponent } from './footer/footer.component';
import { EmailService } from './services/email.service';
import { CardComponent } from './home/card/card.component';
import { AuthService } from './services/auth.service';
import { AuthGuardService } from './services/auth-guard.service';
import { HttpModule } from '@angular/http';


@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NavComponent,
    ContactComponent,
    OpportunitiesComponent,
    FooterComponent,
    CardComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    HttpModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'Contact', component: ContactComponent, canActivate : [AuthGuardService]},
      { path: 'Opportunities', component: OpportunitiesComponent},
    ], {useHash : true})
  ],
  providers: [EmailService,
  AuthService,
AuthGuardService],
  bootstrap: [AppComponent]
})
export class AppModule { }
