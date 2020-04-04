import { Component, OnInit } from '@angular/core';
import { AuthService } from '@app/shared/services';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.sass']
})
export class LogoutComponent implements OnInit {

  constructor(private authService: AuthService) {

  }

  ngOnInit() {
  }
}