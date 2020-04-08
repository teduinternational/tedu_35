import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-validation-message',
  templateUrl: './validation-message.component.html',
  styleUrls: ['./validation-message.component.scss']
})
export class ValidationMessageComponent implements OnInit {
  @Input() entityForm: FormGroup;
  @Input() fieldName: string;
  @Input() validationMessages: any;
  constructor() { }

  ngOnInit(): void {

  }

}
