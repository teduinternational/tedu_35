import { Component, OnInit, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MessageConstants } from '@app/shared/constants';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { NotificationService, UsersService } from '@app/shared/services';
import { DatePipe } from '@angular/common';

@Component({
    selector: 'app-users-detail',
    templateUrl: './users-detail.component.html',
    styleUrls: ['./users-detail.component.scss']
})
export class UsersDetailComponent implements OnInit {

    constructor(
        public bsModalRef: BsModalRef,
        private usersService: UsersService,
        private notificationService: NotificationService,
        private fb: FormBuilder,
        private datePipe: DatePipe
        ) {
    }
    public blockedPanel = false;
    public myRoles: string[] = [];
    public entityForm: FormGroup;
    public dialogTitle: string;
    public entityId: string;

    public btnDisabled = false;
    public saveBtnName: string;
    public closeBtnName: string;
    public vi: any;
    saved: EventEmitter<any> = new EventEmitter();

    // Validate
    noSpecial: RegExp = /^[^<>*!_~]+$/;
    validation_messages = {
        'firstName': [
            { type: 'required', message: 'Bạn phải nhập tên người dùng' },
            { type: 'minlength', message: 'Bạn phải nhập ít nhất 3 kí tự' },
            { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' }
        ],
        'lastName': [
            { type: 'required', message: 'Bạn phải nhập tên người dùng' },
            { type: 'minlength', message: 'Bạn phải nhập ít nhất 3 kí tự' },
            { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' }
        ],
        'userName': [
            { type: 'required', message: 'Bạn phải nhập tên tài khoản' },
            { type: 'minlength', message: 'Bạn phải nhập ít nhất 3 kí tự' },
            { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' }
        ],
        'password': [
            { type: 'required', message: 'Bạn phải nhập tên tài khoản' },
            { type: 'minlength', message: 'Bạn phải nhập ít nhất 6 kí tự' },
            { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
            { type: 'pattern', message: 'Mật khẩu không đủ độ phức tạp' }
        ],
        'email': [
            { type: 'required', message: 'Bạn phải nhập email' },
            { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
            { type: 'pattern', message: 'Bạn phải nhập đúng định dạng Email' }
        ]
    };

    ngOnInit() {
        this.entityForm = this.fb.group({
            'id': new FormControl(''),
            'firstName': new FormControl('', Validators.compose([
                Validators.required,
                Validators.maxLength(255),
                Validators.minLength(3)
            ])),
            'lastName': new FormControl('', Validators.compose([
                Validators.required,
                Validators.maxLength(255),
                Validators.minLength(3)
            ])),
            'userName': new FormControl('', Validators.compose([
                Validators.required,
                Validators.maxLength(255),
                Validators.minLength(3)
            ])),
            'password': new FormControl('', Validators.compose([
                Validators.required,
                Validators.maxLength(255),
                Validators.minLength(8),
                Validators.pattern('^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$')
            ])),
            'email': new FormControl('', Validators.compose([
                Validators.required,
                Validators.maxLength(255),
                Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')
            ])),
            'phoneNumber': new FormControl(),
            'dob': new FormControl()
        });
        if (this.entityId) {
            this.loadUserDetail(this.entityId);
            this.dialogTitle = 'Cập nhật';
            this.entityForm.controls['userName'].disable({ onlySelf: true });
            this.entityForm.controls['password'].disable({ onlySelf: true });
        } else {
            this.dialogTitle = 'Thêm mới';
        }

        this.vi = {
            firstDayOfWeek: 0,
            dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
            dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
            dayNamesMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
            monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            today: 'Today',
            clear: 'Clear'
        };
    }

    loadUserDetail(id: any) {
        this.btnDisabled = true;
        this.blockedPanel = true;
        this.usersService.getDetail(id)
            .subscribe((response: any) => {
                const dob: Date = new Date(response.dob);
                this.entityForm.setValue({
                    id: response.id,
                    firstName: response.firstName,
                    lastName: response.lastName,
                    userName: response.userName,
                    email: response.email,
                    password: '',
                    phoneNumber: response.phoneNumber,
                    dob: dob
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


    saveChange() {
        this.btnDisabled = true;
        this.blockedPanel = true;
        const rawValues = this.entityForm.getRawValue();
         rawValues.dob = this.datePipe.transform(this.entityForm.controls['dob'].value, 'MM/dd/yyyy');
        if (this.entityId) {
            this.usersService.update(this.entityId, rawValues)
                .subscribe(() => {
                    this.notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);

                    setTimeout(() => {
                        this.btnDisabled = false;
                        this.blockedPanel = false;
                    }, 1000);
                    this.saved.emit(this.entityForm.value);
                }, error => {
                    this.notificationService.showError(error);
                    setTimeout(() => {
                        this.btnDisabled = false;
                        this.blockedPanel = false;
                    }, 1000);
                });
        } else {
            this.usersService.add(rawValues)
                .subscribe(() => {
                    this.notificationService.showSuccess(MessageConstants.CREATED_OK_MSG);
                    this.saved.emit(this.entityForm.value);

                    setTimeout(() => {
                        this.btnDisabled = false;
                        this.blockedPanel = false;
                    }, 1000);
                }, error => {
                    this.notificationService.showError(error);

                    setTimeout(() => {
                        this.btnDisabled = false;
                        this.blockedPanel = false;
                    }, 1000);
                });

        }
    }
}
