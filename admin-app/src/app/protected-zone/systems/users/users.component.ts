import { Component, OnInit } from '@angular/core';
import { UserService } from '@app/shared/services';
import { Observable } from 'rxjs';
import { User } from '@app/shared/models';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  public users$: Observable<User[]>;

  constructor(private userService: UserService) { }

  ngOnInit() {
    this.users$ = this.userService.getAll();
  }

}
