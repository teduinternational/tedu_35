import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FunctionsComponent } from './functions/functions.component';
import { UsersComponent } from './users/users.component';
import { RolesComponent } from './roles/roles.component';
import { PermissionsComponent } from './permissions/permissions.component';
import { SystemsRoutingModule } from './systems-routing.module';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { CheckboxModule } from 'primeng/checkbox';
import { CalendarModule } from 'primeng/calendar';
import { KeyFilterModule } from 'primeng/keyfilter';

import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { RolesDetailComponent } from './roles/roles-detail/roles-detail.component';
import { NotificationService } from '@app/shared/services';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { ValidationMessageModule } from '@app/shared/modules/validation-message/validation-message.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UsersDetailComponent } from './users/users-detail/users-detail.component';
import { RolesAssignComponent } from './users/roles-assign/roles-assign.component';

@NgModule({
  declarations: [
    FunctionsComponent,
    UsersComponent,
    RolesComponent,
    PermissionsComponent,
    RolesDetailComponent,
    UsersDetailComponent,
    RolesAssignComponent
  ],
  imports: [
    CommonModule,
    SystemsRoutingModule,
    PanelModule,
    ButtonModule,
    TableModule,
    PaginatorModule,
    CalendarModule,
    CheckboxModule,
    KeyFilterModule,
    BlockUIModule,
    InputTextModule,
    FormsModule,
    ReactiveFormsModule,
    ProgressSpinnerModule,
    ValidationMessageModule,
    ModalModule.forRoot()
  ],
  providers: [
    NotificationService,
    BsModalService,
    DatePipe
  ]
})
export class SystemsModule { }
