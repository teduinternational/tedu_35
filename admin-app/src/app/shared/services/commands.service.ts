import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Command } from '../models';


@Injectable({ providedIn: 'root' })
export class CommandsService extends BaseService {
    private _sharedHeaders = new HttpHeaders();
    constructor(private http: HttpClient) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');
    }
    getAll() {
        return this.http.get<Command[]>(`${environment.apiUrl}/api/commands`, { headers: this._sharedHeaders })
            .pipe(map((response: Command[]) => {
                return response;
            }), catchError(this.handleError));
    }
}
