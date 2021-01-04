import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ValidationMessageComponent } from './validation-message.component';



@NgModule({
  declarations: [ValidationMessageComponent],
  imports: [
    CommonModule
  ],
  exports:[
    ValidationMessageComponent
  ]
})
export class ValidationMessageModule { }
