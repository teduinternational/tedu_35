import { Directive, ElementRef, Input, OnInit } from '@angular/core';
import { AuthService } from '../services';

@Directive({
    selector: '[appPermission]'
})
export class PermissionDirective implements OnInit {
    @Input() appFunction: string;
    @Input() appAction: string;

    constructor(private el: ElementRef, private authService: AuthService) {

    }
    ngOnInit() {
        const loggedInUser = this.authService.isAuthenticated();
        if (loggedInUser) {
            const permissions = JSON.parse(this.authService.profile.permissions);
            if (permissions && permissions.filter(x => x === this.appFunction + '_' + this.appAction).length > 0) {
                this.el.nativeElement.style.display = '';
            } else {
                this.el.nativeElement.style.display = 'none';
            }
        } else {
            this.el.nativeElement.style.display = 'none';
        }
    }
}