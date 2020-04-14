import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { PermissionUpdateRequest, PermissionScreen } from '../models';


@Injectable({ providedIn: 'root' })
export class PermissionsService extends BaseService {
    private _sharedHeaders = new HttpHeaders();
    constructor(private http: HttpClient) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');
    }
    save(roleId: string, request: PermissionUpdateRequest) {
        return this.http.put(`${environment.apiUrl}/api/roles/${roleId}/permissions`, JSON.stringify(request),
            { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    getFunctionWithCommands() {
        return this.http.get<PermissionScreen>(`${environment.apiUrl}/api/permissions`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
}
