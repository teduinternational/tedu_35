import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map, share } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Pagination, KnowledgeBase } from '../models';


@Injectable({ providedIn: 'root' })
export class KnowledgeBasesService extends BaseService {
    private _sharedHeaders = new HttpHeaders();
    constructor(private http: HttpClient) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');
    }
    add(formData: FormData) {
        return this.http.post(`${environment.apiUrl}/api/knowledgeBases`, formData,
            {
                reportProgress: true,
                observe: 'events'
            }).pipe(catchError(this.handleError));
    }

    update(id: string, formData: FormData) {
        return this.http.put(`${environment.apiUrl}/api/knowledgeBases/${id}`, formData,
            {
                reportProgress: true,
                observe: 'events'
            }).pipe(catchError(this.handleError));
    }

    getDetail(id) {
        return this.http.get<KnowledgeBase>(`${environment.apiUrl}/api/knowledgeBases/${id}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    getAllPaging(filter, pageIndex, pageSize) {
        return this.http.get<Pagination<KnowledgeBase>>(`${environment.apiUrl}/api/knowledgeBases/filter?pageIndex=${pageIndex}&pageSize=${pageSize}&filter=${filter}`, { headers: this._sharedHeaders })
            .pipe(map((response: Pagination<KnowledgeBase>) => {
                return response;
            }), catchError(this.handleError));
    }

    delete(id) {
        return this.http.delete(environment.apiUrl + '/api/knowledgeBases/' + id, { headers: this._sharedHeaders })
            .pipe(
                catchError(this.handleError)
            );
    }

    getAll() {
        return this.http.get<KnowledgeBase[]>(`${environment.apiUrl}/api/knowledgeBases`, { headers: this._sharedHeaders })
            .pipe(map((response: KnowledgeBase[]) => {
                return response;
            }), catchError(this.handleError));
    }

    deleteAttachment(knowledgeBaseId, attachmentId) {
        return this.http.delete(environment.apiUrl + '/api/knowledgeBases/' + knowledgeBaseId
            + '/attachments/' + attachmentId, { headers: this._sharedHeaders })
            .pipe(
                catchError(this.handleError)
            );
    }

}
