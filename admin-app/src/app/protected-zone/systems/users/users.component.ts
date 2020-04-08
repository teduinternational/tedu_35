import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '@app/shared/models';
import { UsersService } from '@app/shared/services/users.services';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

  public users$: Observable<User[]>;

  constructor(private userService: UsersService) { }

  ngOnInit() {
    this.users$ = this.userService.getAll();
  }

}
