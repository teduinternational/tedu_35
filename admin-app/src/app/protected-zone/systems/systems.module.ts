import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PermissionsComponent } from './permissions/permissions.component';
import { UsersComponent } from './users/users.component';
import { FunctionsComponent } from './functions/functions.component';
import { RolesComponent } from './roles/roles.component';



@NgModule({
  declarations: [PermissionsComponent, UsersComponent, FunctionsComponent, RolesComponent],
  imports: [
    CommonModule
  ]
})
export class SystemsModule { }
