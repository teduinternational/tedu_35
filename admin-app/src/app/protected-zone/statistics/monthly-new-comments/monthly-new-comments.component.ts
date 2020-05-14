import { Component, OnInit } from '@angular/core';
import { StatisticsService } from '@app/shared/services';
import { BaseComponent } from '@app/protected-zone/base/base.component';

@Component({
  selector: 'app-monthly-new-comments',
  templateUrl: './monthly-new-comments.component.html',
  styleUrls: ['./monthly-new-comments.component.css']
})
export class MonthlyNewCommentsComponent extends BaseComponent implements OnInit {
  // Default
  public blockedPanel = false;
  public screenTitle: string;

  // Customer Receivable
  public items: any[];
  public year: number = new Date().getFullYear();
  public totalItems = 0;
  constructor(
    private statisticService: StatisticsService) {
      super('STATISTIC_MONTHLY_COMMENT');
  }
  ngOnInit() {
    super.ngOnInit();
    this.loadData();
  }
  loadData() {
    this.blockedPanel = true;
    this.statisticService.getMonthlyNewComments(this.year)
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
