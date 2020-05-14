import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { BaseComponent } from '@app/protected-zone/base/base.component';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { NotificationService, ReportsService } from '@app/shared/services';
import { Pagination, Report } from '@app/shared/models';
import { MessageConstants } from '@app/shared/constants';
import { ReportsDetailComponent } from '../reports-detail/reports-detail.component';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent extends BaseComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();
  // Default
  public bsModalRef: BsModalRef;
  public blockedPanel = false;
  public entityId: number;
  public screenTitle: string;
  /**
   * Paging
   */
  public pageIndex = 1;
  public pageSize = 10;
  public pageDisplay = 10;
  public totalRecords: number;
  public keyword = '';
  // Role
  public items: any[];
  public selectedItems = [];
  constructor(private reportsService: ReportsService,
    private notificationService: NotificationService,
    private activeRoute: ActivatedRoute,
    private modalService: BsModalService) {
      super('CONTENT_REPORT');
     }

  ngOnInit(): void {
    super.ngOnInit();
    this.subscription.add(this.activeRoute.params.subscribe(params => {
      this.entityId = params['knowledgeBaseId'];
    }));
    this.loadData();
  }

  loadData(selectedId = null) {
    this.blockedPanel = true;
    this.subscription.add(this.reportsService.getAllPaging(this.entityId, this.keyword, this.pageIndex, this.pageSize)
      .subscribe((response: Pagination<Report>) => {
        this.processLoadData(selectedId, response);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }
  private processLoadData(selectedId = null, response: Pagination<Report>) {
    this.items = response.items;
    this.pageIndex = this.pageIndex;
    this.pageSize = this.pageSize;
    this.totalRecords = response.totalRecords;
    if (this.selectedItems.length === 0 && this.items.length > 0) {
      this.selectedItems.push(this.items[0]);
    }
    if (selectedId != null && this.items.length > 0) {
      this.selectedItems = this.items.filter(x => x.Id === selectedId);
    }
  }
  pageChanged(event: any): void {
    this.pageIndex = event.page + 1;
    this.pageSize = event.rows;
    this.loadData();
  }

  showDetailModel() {
    if (this.selectedItems.length === 0) {
      this.notificationService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    const initialState = {
      reportId: this.selectedItems[0].id,
      knowledgeBaseId: this.selectedItems[0].knowledgeBaseId
    };
    this.bsModalRef = this.modalService.show(ReportsDetailComponent,
      {
        initialState: initialState,
        class: 'modal-lg',
        backdrop: 'static'
      });
  }

  deleteItems() {
    const reportId = this.selectedItems[0].id;
    const knowledgeBaseId = this.selectedItems[0].knowledgeBaseId;
    this.notificationService.showConfirmation(MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemsConfirm(knowledgeBaseId, reportId));
  }
  deleteItemsConfirm(knowledgeBaseId, reportId) {
    this.blockedPanel = true;
    this.subscription.add(this.reportsService.delete(knowledgeBaseId, reportId).subscribe(() => {
      this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
      this.loadData();
      this.selectedItems = [];
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, error => {
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
