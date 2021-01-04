import { throwError } from 'rxjs';

export abstract class BaseService {

    constructor() { }

    protected handleError(errorResponse: any) {
        if (errorResponse.error.message) {
            return throwError(errorResponse.error.message || 'Server error');
        }

        if (errorResponse.error.errors) {
            let modelStateErrors = '';

            // for now just concatenate the error descriptions, alternative we could simply pass the entire error response upstream
            for (const errorMsg of errorResponse.error.errors) {
                modelStateErrors += errorMsg + '<br/>';
            }
            return throwError(modelStateErrors || 'Server error');
        }
        return throwError('Server error');
    }
}