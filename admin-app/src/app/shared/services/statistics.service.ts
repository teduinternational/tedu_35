import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { MonthlyNewComment, MonthlyNewKb, MonthlyNewMember } from '../models';


@Injectable({ providedIn: 'root' })
export class StatisticsService extends BaseService {
    private _sharedHeaders = new HttpHeaders();
    constructor(private http: HttpClient) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');
    }
    getMonthlyNewComments(year: number) {
        return this.http.get<MonthlyNewComment[]>(`${environment.apiUrl}/api/statistics/monthly-comments?year=${year}`,
            { headers: this._sharedHeaders })
            .pipe(map((response: MonthlyNewComment[]) => {
                return response;
            }), catchError(this.handleError));
    }

    getMonthlyNewKbs(year: number) {
        return this.http.get<MonthlyNewKb[]>(`${environment.apiUrl}/api/statistics/monthly-newkbs?year=${year}`,
            { headers: this._sharedHeaders })
            .pipe(map((response: MonthlyNewKb[]) => {
                return response;
            }), catchError(this.handleError));
    }

    getMonthlyNewMembers(year: number) {
        return this.http.get<MonthlyNewMember[]>(`${environment.apiUrl}/api/statistics/monthly-registers?year=${year}`,
            { headers: this._sharedHeaders })
            .pipe(map((response: MonthlyNewMember[]) => {
                return response;
            }), catchError(this.handleError));
    }
}
