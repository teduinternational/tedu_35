import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CategoriesComponent } from './categories/categories.component';
import { KnowledgeBaseComponent } from './knowledge-base/knowledge-base.component';
import { CommentsComponent } from './comments/comments.component';
import { ReportsComponent } from './reports/reports.component';
import { AttachmentsComponent } from './attachments/attachments.component';

const routes: Routes = [
    {
        path: '', component: KnowledgeBaseComponent,
    },
    {
        path: 'categories', component: CategoriesComponent
    },
    {
        path: 'knowledge-bases', component: KnowledgeBaseComponent
    },
    {
        path: 'comments', component: CommentsComponent
    },
    {
        path: 'reports', component: ReportsComponent
    },
    {
        path: 'attachments', component: AttachmentsComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ContentsRoutingModule {
}
