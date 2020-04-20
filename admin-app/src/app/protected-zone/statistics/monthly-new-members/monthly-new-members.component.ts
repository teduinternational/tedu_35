import { Component, OnInit } from '@angular/core';
import { StatisticsService } from '@app/shared/services';

@Component({
  selector: 'app-monthly-new-members',
  templateUrl: './monthly-new-members.component.html',
  styleUrls: ['./monthly-new-members.component.css']
})
export class MonthlyNewMembersComponent implements OnInit {

 // Default
 public blockedPanel = false;
 // Customer Receivable
 public items: any[];
 public year: number = new Date().getFullYear();
 public totalItems = 0;
 constructor(
   private statisticService: StatisticsService) {
 }
 ngOnInit() {
   this.loadData();
 }
 loadData() {
   this.blockedPanel = true;
   this.statisticService.getMonthlyNewMembers(this.year)
     .subscribe((response: any) => {
       this.totalItems = 0;
       this.items = response;
       response.forEach(element => {
         this.totalItems += element.NumberOfUsers;
       });
       setTimeout(() => { this.blockedPanel = false; }, 1000);
     }, error => {
       setTimeout(() => { this.blockedPanel = false; }, 1000);
     });
 }
}
