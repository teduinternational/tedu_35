import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { User } from '../models';
import { UtilitiesService } from './utilities.service';

@Injectable({ providedIn: 'root' })
export class UserService extends BaseService {
    constructor(private http: HttpClient, private utilitiesService: UtilitiesService) {
        super();
    }
    getAll() {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        };
        return this.http.get<User[]>(`${environment.apiUrl}/api/users`, httpOptions)
            .pipe(catchError(this.handleError));
    }

    getMenuByUser(userId: string) {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        };
        return this.http.get<Function[]>(`${environment.apiUrl}/api/users/${userId}/menu`, httpOptions)
            .pipe(map(response => {
                const functions = this.utilitiesService.UnflatteringForLeftMenu(response);
                return functions;
            }), catchError(this.handleError));
    }
}