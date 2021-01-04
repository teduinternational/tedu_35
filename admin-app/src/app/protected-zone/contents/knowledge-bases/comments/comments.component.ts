import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Pagination, Comment } from '@app/shared/models';
import { NotificationService, CommentsService } from '@app/shared/services';
import { MessageConstants } from '@app/shared/constants';
import { CommentsDetailComponent } from '../comments-detail/comments-detail.component';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-comments',
  templateUrl: './comments.component.html',
  styleUrls: ['./comments.component.scss']
})
export class CommentsComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();
  // Default
  public bsModalRef: BsModalRef;
  public blockedPanel = false;
  public entityId: number;
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
  constructor(private commentsService: CommentsService,
    private notificationService: NotificationService,
    private activeRoute: ActivatedRoute,
    private modalService: BsModalService) { }

  ngOnInit(): void {
    this.subscription.add(this.activeRoute.params.subscribe(params => {
      this.entityId = params['knowledgeBaseId'];
    }));
    this.loadData();
  }

  loadData(selectedId = null) {
    this.blockedPanel = true;
    this.subscription.add(this.commentsService.getAllPaging(this.entityId, this.keyword, this.pageIndex, this.pageSize)
      .subscribe((response: Pagination<Comment>) => {
        this.processLoadData(selectedId, response);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }
  private processLoadData(selectedId = null, response: Pagination<Comment>) {
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
      commentId: this.selectedItems[0].id,
      knowledgeBaseId: this.entityId
    };
    this.bsModalRef = this.modalService.show(CommentsDetailComponent,
      {
        initialState: initialState,
        class: 'modal-lg',
        backdrop: 'static'
      });
  }

  deleteItems() {
    const commentId = this.selectedItems[0].id;
    const knowledgeBaseId = this.selectedItems[0].knowledgeBaseId;
    this.notificationService.showConfirmation(MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemsConfirm(commentId,knowledgeBaseId));
  }
  deleteItemsConfirm(commentId,knowledgeBaseId) {
    this.blockedPanel = true;
    this.subscription.add(this.commentsService.delete(knowledgeBaseId, commentId).subscribe(() => {
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
