import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UsersComponent } from './users/users.component';
import { FunctionsComponent } from './functions/functions.component';
import { RolesComponent } from './roles/roles.component';
import { PermissionsComponent } from './permissions/permissions.component';

const routes: Routes = [
    {
        path: '',
        component: UsersComponent
    },
    {
        path: 'users',
        component: UsersComponent
    },
    {
        path: 'functions',
        component: FunctionsComponent
    },
    {
        path: 'roles',
        component: RolesComponent
    },
    {
        path: 'permissions',
        component: PermissionsComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SystemsRoutingModule {}
