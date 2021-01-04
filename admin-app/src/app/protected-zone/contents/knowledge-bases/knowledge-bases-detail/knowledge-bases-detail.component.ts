import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { KnowledgeBasesService, NotificationService, CategoriesService, UtilitiesService } from '@app/shared/services';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { MessageConstants } from '@app/shared/constants';
import { Category } from '@app/shared/models';
import { Router, ActivatedRoute } from '@angular/router';
import { environment } from '@environments/environment';
import { SelectItem } from 'primeng/api/selectitem';

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
    private activeRoute: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder) {
  }
  private subscription: Subscription[] = [];
  public entityForm: FormGroup;
  public dialogTitle: string;
  public entityId: string;
  public categories: SelectItem[] = [];
  public blockedPanel = false;
  public selectedFiles: File[] = [];
  public attachments: any[] = [];
  public backendApiUrl = environment.apiUrl;

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
    this.subscription.push(this.activeRoute.params.subscribe(params => {
      this.entityId = params['id'];
    }));

    this.subscription.push(this.categoriesService.getAll()
      .subscribe((response: Category[]) => {
        response.forEach(element => {
          this.categories.push({ label: element.name, value: element.id });
        });

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
    this.subscription.push(this.knowledgeBasesService.getDetail(id).subscribe((response: any) => {
      this.entityForm.setValue({
        title: response.title,
        categoryId: response.categoryId,
        seoAlias: response.seoAlias,
        description: response.description,
        environment: response.environment,
        problem: response.problem,
        stepToReproduce: response.stepToReproduce,
        errorMessage: response.errorMessage,
        workaround: response.workaround,
        note: response.note,
        labels: response.labels
      });
      this.attachments = response.attachments;
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, error => {
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }
  public selectAttachments($event) {
    if ($event.currentFiles) {
      $event.currentFiles.forEach(element => {
        this.selectedFiles.push(element);
      });
    }
  }
  public removeAttachments($event) {
    if ($event.file) {
      this.selectedFiles.splice(this.selectedFiles.findIndex(item => item.name === $event.file.name), 1);
    }

  }
  public deleteAttachment(attachmentId) {
    this.blockedPanel = true;
    this.subscription.push(this.knowledgeBasesService.deleteAttachment(this.entityId, attachmentId)
      .subscribe(() => {
        this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
        this.attachments.splice(this.attachments.findIndex(item => item.id === attachmentId), 1);
        setTimeout(() => { this.blockedPanel = false; }, 1000);

      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
    return false;
  }
  goBackToList() {
    this.router.navigateByUrl('/contents/knowledge-bases');
  }
  public saveChange() {
    this.blockedPanel = true;
    const formValues = this.entityForm.getRawValue();
    const formData = this.utilitiesService.ToFormData(formValues);
    this.selectedFiles.forEach(file => {
      formData.append('attachments', file, file.name);
    });

    if (this.entityId) {
      this.subscription.push(this.knowledgeBasesService.update(this.entityId, formData)
        .subscribe((response: any) => {
          if (response.status === 204) {
            this.notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);
            this.router.navigateByUrl('/contents/knowledge-bases');
          }

        }, error => {
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    } else {
      this.subscription.push(this.knowledgeBasesService.add(formData)
        .subscribe((response: any) => {
          if (response.status === 201) {
            this.notificationService.showSuccess(MessageConstants.CREATED_OK_MSG);
            this.router.navigateByUrl('/contents/knowledge-bases');
          }
        }, error => {
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    }
    return false;
  }
  ngOnDestroy(): void {
    this.subscription.forEach(element => {
      element.unsubscribe();
    });
  }
}
