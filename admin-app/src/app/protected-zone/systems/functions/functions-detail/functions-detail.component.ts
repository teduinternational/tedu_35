import { Component, OnInit, EventEmitter } from '@angular/core';
import { UtilitiesService, NotificationService, FunctionsService } from '@app/shared/services';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MessageConstants } from '@app/shared/constants';

@Component({
  selector: 'app-functions-detail',
  templateUrl: './functions-detail.component.html',
  styleUrls: ['./functions-detail.component.scss']
})
export class FunctionsDetailComponent implements OnInit {

  constructor(private utilityService: UtilitiesService,
    public bsModalRef: BsModalRef,
    private functionsService: FunctionsService,
    private notificationService: NotificationService,
    private fb: FormBuilder) {
  }
  public blockedPanel = false;
  public entityForm: FormGroup;
  public dialogTitle: string;
  public entityId: string;
  public btnDisabled = false;

  saved: EventEmitter<any> = new EventEmitter();
  public rootFunctions: any[] = [];

  // Validate
  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validation_messages = {
    'name': [
      { type: 'required', message: 'Bạn phải nhập tên trang' },
      { type: 'minlength', message: 'Bạn phải nhập ít nhất 3 kí tự' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' }
    ],
    'id': [
      { type: 'required', message: 'Bạn phải nhập mã duy nhất' }
    ],
    'sortOrder': [
      { type: 'required', message: 'Bạn phải nhập thứ tự' }
    ]
  };

  ngOnInit() {
    this.entityForm = this.fb.group({
      'id': new FormControl('', Validators.required),
      'parentId': new FormControl(),
      'name': new FormControl('', Validators.compose([
        Validators.required,
        Validators.maxLength(255),
        Validators.minLength(3)
      ])),
      'url': new FormControl(),
      'icon': new FormControl(),
      'sortOrder': new FormControl(1, Validators.required)
    });
    if (this.entityId) {
      this.dialogTitle = 'Cập nhật';
      this.loadParents(this.entityId);
      this.loadDetail(this.entityId);
      this.entityForm.controls['id'].disable({ onlySelf: true });

    } else {
      this.loadParents(null);
      this.dialogTitle = 'Thêm mới';
    }
  }

  loadDetail(id: any) {
    this.btnDisabled = true;
    this.blockedPanel = true;
    this.functionsService.getDetail(id)
      .subscribe((response: any) => {
        this.entityForm.setValue({
          id: response.id,
          parentId: response.parentId,
          name: response.name,
          url: response.url,
          icon: response.icon,
          sortOrder: response.sortOrder
        });
        setTimeout(() => {
          this.btnDisabled = false;
          this.blockedPanel = false;
        }, 1000);
      }, error => {
        setTimeout(() => {
          this.btnDisabled = false;
          this.blockedPanel = false;
        }, 1000);
      });
  }

  loadParents(id) {
    this.functionsService.getAllByParentId(id)
      .subscribe((response: any) => {
        this.rootFunctions = [];
        response.forEach(element => {
          this.rootFunctions.push({
            value: element.id,
            label: element.name
          });
        });
      });
  }

  saveChange() {
    this.btnDisabled = true;
    this.blockedPanel = true;
    if (this.entityId) {
      this.functionsService.update(this.entityId, this.entityForm.getRawValue())
        .subscribe(() => {
          this.notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);
          this.saved.emit(this.entityForm.value);

          setTimeout(() => {
            this.btnDisabled = false;
            this.blockedPanel = false;
          }, 1000);
        }, error => {
          setTimeout(() => {
            this.btnDisabled = false;
            this.blockedPanel = false;
          }, 1000);
        });
    } else {
      this.functionsService.add(this.entityForm.value)
        .subscribe(() => {

          this.notificationService.showSuccess(MessageConstants.CREATED_OK_MSG);
          this.saved.emit(this.entityForm.value);
          setTimeout(() => {
            this.btnDisabled = false;
            this.blockedPanel = false;
          }, 1000);

        }, error => {
          setTimeout(() => {
            this.btnDisabled = false;
            this.blockedPanel = false;
          }, 1000);
        });

    }
  }

}
