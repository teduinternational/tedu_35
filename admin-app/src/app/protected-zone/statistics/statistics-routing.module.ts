import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MonthlyRegisterUsersComponent } from './monthly-register-users/monthly-register-users.component';

const routes: Routes = [
    {
        path: '', component: MonthlyRegisterUsersComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class StatisticsRoutingModule {
}
