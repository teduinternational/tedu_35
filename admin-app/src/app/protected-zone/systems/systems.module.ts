import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FunctionsComponent } from './functions/functions.component';
import { UsersComponent } from './users/users.component';
import { RolesComponent } from './roles/roles.component';
import { PermissionsComponent } from './permissions/permissions.component';
import { SystemsRoutingModule } from './systems-routing.module';
import { RolesDetailComponent } from './roles/roles-detail/roles-detail.component';
import { BsModalService } from 'ngx-bootstrap/modal';
import { ModalModule } from 'ngx-bootstrap';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { BlockUIModule } from 'primeng/blockui';
import { PaginatorModule } from 'primeng/paginator';
import { NotificationService } from '@app/shared/services';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ValidationMessageModule } from '@app/shared/modules/validation-message/validation-message.module';

@NgModule({
  declarations:
    [FunctionsComponent,
      UsersComponent,
      RolesComponent,
      PermissionsComponent,
      RolesDetailComponent
    ],
  imports: [
    CommonModule,
    SystemsRoutingModule,
    PanelModule,
    TableModule,
    PaginatorModule,
    ProgressSpinnerModule,
    BlockUIModule,
    FormsModule,
    ReactiveFormsModule,
    ButtonModule,
    InputTextModule,
    ValidationMessageModule,
    ModalModule.forRoot()
  ],
  providers: [BsModalService, NotificationService],
  entryComponents: [
    RolesDetailComponent
  ]
})
export class SystemsModule { }
