import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MonthlyNewKbsComponent } from './monthly-new-kbs/monthly-new-kbs.component';
import { MonthlyNewMembersComponent } from './monthly-new-members/monthly-new-members.component';
import { MonthlyNewCommentsComponent } from './monthly-new-comments/monthly-new-comments.component';

const routes: Routes = [
    {
        path: '',
        component: MonthlyNewKbsComponent
    },
    {
        path: 'monthly-new-kbs',
        component: MonthlyNewKbsComponent
    },
    {
        path: 'monthly-new-members',
        component: MonthlyNewMembersComponent
    },
    {
        path: 'monthly-new-comments',
        component: MonthlyNewCommentsComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class StatisticsRoutingModule {}
