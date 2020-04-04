import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FunctionsComponent } from './functions/functions.component';
import { UsersComponent } from './users/users.component';
import { RolesComponent } from './roles/roles.component';
import { PermissionsComponent } from './permissions/permissions.component';
import { SystemsRoutingModule } from './systems-routing.module';



@NgModule({
  declarations: [FunctionsComponent, UsersComponent, RolesComponent, PermissionsComponent],
  imports: [
    CommonModule,
    SystemsRoutingModule
  ]
})
export class SystemsModule { }
