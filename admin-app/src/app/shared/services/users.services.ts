import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { User, Pagination } from '../models';
import { UtilitiesService } from './utilities.service';

@Injectable({ providedIn: 'root' })
export class UsersService extends BaseService {
    private _sharedHeaders = new HttpHeaders();

    constructor(private http: HttpClient, private utilitiesService: UtilitiesService) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');

    }
    add(entity: User) {
        return this.http.post(`${environment.apiUrl}/api/users`, JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    update(id: string, entity: User) {
        return this.http.put(`${environment.apiUrl}/api/users/${id}`, JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    getDetail(id) {
        return this.http.get<User>(`${environment.apiUrl}/api/users/${id}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    getAllPaging(filter, pageIndex, pageSize) {
        return this.http.get<Pagination<User>>(`${environment.apiUrl}/api/users/filter?pageIndex=${pageIndex}&pageSize=${pageSize}&filter=${filter}`, { headers: this._sharedHeaders })
            .pipe(map((response: Pagination<User>) => {
                return response;
            }), catchError(this.handleError));
    }

    delete(id) {
        return this.http.delete(environment.apiUrl + '/api/users/' + id, { headers: this._sharedHeaders })
            .pipe(
                catchError(this.handleError)
            );
    }

    getMenuByUser(userId: string) {
        return this.http.get<Function[]>(`${environment.apiUrl}/api/users/${userId}/menu`, { headers: this._sharedHeaders })
            .pipe(map(response => {
                const functions = this.utilitiesService.UnflatteringForLeftMenu(response);
                return functions;
            }), catchError(this.handleError));
    }

    getUserRoles(userId: string) {
        return this.http.get<string[]>(`${environment.apiUrl}/api/users/${userId}/roles`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    removeRolesFromUser(id, roleNames: string[]) {
        let rolesQuery = '';
        for (const roleName of roleNames) {
            rolesQuery += 'roleNames' + '=' + roleName + '&';
        }
        return this.http.delete(environment.apiUrl + '/api/users/' + id + '/roles?' + rolesQuery, { headers: this._sharedHeaders })
            .pipe(
                catchError(this.handleError)
            );
    }

    assignRolesToUser(userId: string, assignRolesToUser: any) {
        return this.http.post(`${environment.apiUrl}/api/users/${userId}/roles`,
            JSON.stringify(assignRolesToUser), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }
}
