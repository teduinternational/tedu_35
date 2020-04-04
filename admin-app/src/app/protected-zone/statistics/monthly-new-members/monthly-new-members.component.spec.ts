import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MonthlyNewMembersComponent } from './monthly-new-members.component';

describe('MonthlyNewMembersComponent', () => {
  let component: MonthlyNewMembersComponent;
  let fixture: ComponentFixture<MonthlyNewMembersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MonthlyNewMembersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MonthlyNewMembersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
