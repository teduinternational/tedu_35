import { Component, OnInit } from '@angular/core';
import { routerTransition } from '../router.animations';
import { AuthService } from '../shared/services';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    animations: [routerTransition()]
})
export class LoginComponent implements OnInit {
    constructor(
      private authService: AuthService,
      private spinner: NgxSpinnerService
    ) {}

    ngOnInit() {}

    login() {
        this.spinner.show();
        this.authService.login();
      }

}
