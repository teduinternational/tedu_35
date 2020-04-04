import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PermissionsComponent } from './permissions/permissions.component';
import { UsersComponent } from './users/users.component';
import { FunctionsComponent } from './functions/functions.component';
import { RolesComponent } from './roles/roles.component';
import { SystemsRoutingModule } from './systems-routing.module';



@NgModule({
  declarations: [PermissionsComponent, UsersComponent, FunctionsComponent, RolesComponent],
  imports: [
    CommonModule,
    SystemsRoutingModule
  ]
})
export class SystemsModule { }
