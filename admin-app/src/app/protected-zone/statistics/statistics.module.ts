import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MonthlyRegisterUsersComponent } from './monthly-register-users/monthly-register-users.component';
import { StatisticsRoutingModule } from './statistics-routing.module';



@NgModule({
  declarations: [MonthlyRegisterUsersComponent],
  imports: [
    CommonModule,
    StatisticsRoutingModule
  ]
})
export class StatisticsModule { }
