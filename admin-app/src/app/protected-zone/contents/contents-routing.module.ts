import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { KnowledgeBasesComponent } from './knowledge-bases/knowledge-bases.component';
import { CategoriesComponent } from './categories/categories.component';
import { CommentsComponent } from './comments/comments.component';
import { ReportsComponent } from './reports/reports.component';
import { AuthGuard } from '@app/shared';

const routes: Routes = [
    {
        path: '',
        component: KnowledgeBasesComponent,
        data: {
            functionCode: 'CONTENT_KNOWLEDGEBASE'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'knowledge-bases',
        component: KnowledgeBasesComponent,
        data: {
            functionCode: 'CONTENT_KNOWLEDGEBASE'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'categories',
        component: CategoriesComponent,
        data: {
            functionCode: 'CONTENT_CATEGORY'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'comments',
        component: CommentsComponent,
        data: {
            functionCode: 'CONTENT_COMMENT'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'reports',
        component: ReportsComponent,
        data: {
            functionCode: 'CONTENT_REPORT'
        },
        canActivate: [AuthGuard]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ContentsRoutingModule { }
