import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LayoutComponent } from './layout.component';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            { path: '', redirectTo: 'dashboard', pathMatch: 'prefix' },
            { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
            { path: 'contents', loadChildren: () => import('./contents/contents.module').then(m => m.ContentsModule) },
            { path: 'systems', loadChildren: () => import('./systems/systems.module').then(m => m.SystemsModule) },
            { path: 'statistics', loadChildren: () => import('./statistics/statistics.module').then(m => m.StatisticsModule) },

        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LayoutRoutingModule { }
