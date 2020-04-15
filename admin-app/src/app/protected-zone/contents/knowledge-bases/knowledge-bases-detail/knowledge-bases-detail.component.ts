import { Component, OnInit, EventEmitter, OnDestroy } from '@angular/core';
import { KnowledgeBasesService, NotificationService, CategoriesService, UtilitiesService } from '@app/shared/services';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { MessageConstants } from '@app/shared/constants';
import { Category } from '@app/shared/models';

@Component({
  selector: 'app-knowledge-bases-detail',
  templateUrl: './knowledge-bases-detail.component.html',
  styleUrls: ['./knowledge-bases-detail.component.scss']
})
export class KnowledgeBasesDetailComponent implements OnInit, OnDestroy {

  constructor(
    private knowledgeBasesService: KnowledgeBasesService,
    private categoriesService: CategoriesService,
    private notificationService: NotificationService,
    private utilitiesService: UtilitiesService,
    private fb: FormBuilder) {
  }

  private subscription = new Subscription();
  public entityForm: FormGroup;
  public dialogTitle: string;
  private savedEvent: EventEmitter<any> = new EventEmitter();
  public entityId: string;
  public categories = [];
  public btnDisabled = false;

  public blockedPanel = false;

  // Validate
  validation_messages = {
    'title': [
      { type: 'required', message: 'Trường này bắt buộc' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 30 kí tự' }
    ],
    'categoryId': [
      { type: 'required', message: 'Trường này bắt buộc' },
    ],
    'seoAlias': [
      { type: 'required', message: 'Trường này bắt buộc' },
    ]
  };

  ngOnInit() {
    this.entityForm = this.fb.group({
      'categoryId': new FormControl('', Validators.compose([
        Validators.required
      ])),
      'title': new FormControl('', Validators.compose([
        Validators.required
      ])),
      'seoAlias': new FormControl('', Validators.compose([
        Validators.required
      ])),
      'description': new FormControl(''),
      'environment': new FormControl(''),
      'problem': new FormControl('', Validators.compose([
        Validators.required
      ])),
      'stepToReproduce': new FormControl(''),
      'errorMessage': new FormControl(''),
      'workaround': new FormControl(''),
      'note': new FormControl(''),
      'labels': new FormControl(''),
    });
    this.subscription.add(this.categoriesService.getAll()
      .subscribe((response: Category[]) => {
        this.categories = response;

        if (this.entityId) {
          this.loadFormDetails(this.entityId);
          this.dialogTitle = 'Cập nhật';
        } else {
          this.dialogTitle = 'Thêm mới';
        }
      }));
  }
  public generateSeoAlias() {
    const seoAlias = this.utilitiesService.MakeSeoTitle(this.entityForm.controls['title'].value);
    this.entityForm.controls['seoAlias'].setValue(seoAlias);
  }
  private loadFormDetails(id: any) {
    this.blockedPanel = true;
    this.subscription.add(this.knowledgeBasesService.getDetail(id).subscribe((response: any) => {
      this.entityForm.setValue({
        id: response.id,
        name: response.name,
      });
      setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
    }, error => {
      setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
    }));
  }
  public saveChange() {
    this.btnDisabled = true;
    this.blockedPanel = true;
    const value = this.entityForm.getRawValue();
    value.categoryId = value.categoryId.id;
    const formValue = this.utilitiesService.ToFormData(value);
    if (this.entityId) {
      this.subscription.add(this.knowledgeBasesService.update(this.entityId, formValue)
        .subscribe(() => {
          this.savedEvent.emit(this.entityForm.value);
          this.notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);
          this.btnDisabled = false;
          setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
        }, error => {
          setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
        }));
    } else {
      this.subscription.add(this.knowledgeBasesService.add(formValue)
        .subscribe(() => {
          this.savedEvent.emit(this.entityForm.value);
          this.notificationService.showSuccess(MessageConstants.CREATED_OK_MSG);
          this.btnDisabled = false;
          setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
        }, error => {
          setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
        }));
    }
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
