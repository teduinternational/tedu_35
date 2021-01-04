import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MonthlyNewMembersComponent } from './monthly-new-members/monthly-new-members.component';
import { MonthlyNewKbsComponent } from './monthly-new-kbs/monthly-new-kbs.component';
import { MonthlyNewCommentsComponent } from './monthly-new-comments/monthly-new-comments.component';
import { StatisticsRoutingModule } from './statistics-routing.module';

import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { BlockUIModule } from 'primeng/blockui';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { FormsModule } from '@angular/forms';



@NgModule({
  declarations: [MonthlyNewMembersComponent, MonthlyNewKbsComponent, MonthlyNewCommentsComponent],
  imports: [
    CommonModule,
    StatisticsRoutingModule,
    PanelModule,
    FormsModule,
    ButtonModule,
    TableModule,
    BlockUIModule,
    InputTextModule,
    ProgressSpinnerModule
  ]
})
export class StatisticsModule { }
