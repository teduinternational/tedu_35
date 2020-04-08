import { Component, OnInit, EventEmitter, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { NotificationService } from '@app/shared/services';
import { MessageConstants } from '@app/shared/constants/messages.constant';
import { RolesService } from '@app/shared/services/roles.service';
import { BsModalRef } from 'ngx-bootstrap';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-roles-detail',
  templateUrl: './roles-detail.component.html',
  styleUrls: ['./roles-detail.component.scss']
})
export class RolesDetailComponent implements OnInit, OnDestroy {
  constructor(
    public bsModalRef: BsModalRef,
    private rolesService: RolesService,
    private notificationService: NotificationService,
    private fb: FormBuilder) {
  }

  private subscription = new Subscription();
  public entityForm: FormGroup;
  public title: string;
  private savedEvent: EventEmitter<any> = new EventEmitter();
  public id: string;
  public btnDisabled = false;

  public blockedPanel = false;

  // Validate
  validation_messages = {
    'id': [
      { type: 'required', message: 'Trường này bắt buộc' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 25 kí tự' }
    ],
    'name': [
      { type: 'required', message: 'Trường này bắt buộc' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 30 kí tự' }
    ]
  };

  ngOnInit() {
    this.entityForm = this.fb.group({
      'id': new FormControl({ value: '', disabled: true }, Validators.compose([
        Validators.required,
        Validators.maxLength(50)
      ])),
      'name': new FormControl('', Validators.compose([
        Validators.required,
        Validators.maxLength(50)
      ]))
    });
    if (this.id) {
      this.loadFormDetails(this.id);
      this.title = 'Thêm mới';
      this.entityForm.controls['id'].disable({ onlySelf: true });
    } else {
      this.title = 'Cập nhật';
      this.entityForm.controls['id'].enable({ onlySelf: true });
    }
  }

  private loadFormDetails(id: any) {
    this.blockedPanel = true;
    this.subscription.add(this.rolesService.getDetail(id).subscribe((response: any) => {
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
    if (this.id) {
      this.subscription.add(this.rolesService.update(this.id, this.entityForm.getRawValue())
        .subscribe(() => {
          this.savedEvent.emit(this.entityForm.value);
          this.notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);
          this.btnDisabled = false;
          setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
        }, error => {
          setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
        }));
    } else {
      this.subscription.add(this.rolesService.add(this.entityForm.getRawValue())
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
