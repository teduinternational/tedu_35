import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReportsService } from '@app/shared/services';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { Report } from '@app/shared/models';

@Component({
  selector: 'app-reports-detail',
  templateUrl: './reports-detail.component.html',
  styleUrls: ['./reports-detail.component.scss']
})
export class ReportsDetailComponent implements OnInit, OnDestroy {

  constructor(
    public bsModalRef: BsModalRef,
    private reportServices: ReportsService) {
  }

  private subscription = new Subscription();
  public dialogTitle: string;
  public knowledgeBaseId: number;
  public commentId: number;
  public btnDisabled = false;
  public blockedPanel = false;
  public report: Report;

  ngOnInit() {
    if (this.commentId) {
      this.loadFormDetails(this.knowledgeBaseId, this.commentId);
    }
  }
  private loadFormDetails(commentId, knowledgeBaseId) {
    this.blockedPanel = true;
    this.subscription.add(this.reportServices.getDetail(knowledgeBaseId, commentId)
      .subscribe((response: Report) => {
        this.report = response;
        setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
      }));
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

}
