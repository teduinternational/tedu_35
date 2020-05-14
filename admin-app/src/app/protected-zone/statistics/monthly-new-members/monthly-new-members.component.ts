import { Component, OnInit } from '@angular/core';
import { StatisticsService } from '@app/shared/services';
import { BaseComponent } from '@app/protected-zone/base/base.component';

@Component({
  selector: 'app-monthly-new-members',
  templateUrl: './monthly-new-members.component.html',
  styleUrls: ['./monthly-new-members.component.css']
})
export class MonthlyNewMembersComponent extends BaseComponent implements OnInit {

  public screenTitle: string;
  // Default
  public blockedPanel = false;
  // Customer Receivable
  public items: any[];
  public year: number = new Date().getFullYear();
  public totalItems = 0;
  constructor(
    private statisticService: StatisticsService) {
    super('STATISTIC_MONTHLY_NEWMEMBER');

  }
  ngOnInit() {
    super.ngOnInit();
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
