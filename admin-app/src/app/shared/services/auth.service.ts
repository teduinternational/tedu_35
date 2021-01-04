import { Injectable } from "@angular/core";
import { UserManager, UserManagerSettings, User } from "oidc-client";
import { BehaviorSubject } from "rxjs";
import { BaseService } from "./base.service";
import { Profile } from "../models";
import { environment } from "@environments/environment";

@Injectable({
    providedIn: "root",
})
export class AuthService extends BaseService {
    // Observable navItem source
    private _authNavStatusSource = new BehaviorSubject<boolean>(false);
    // Observable navItem stream
    authNavStatus$ = this._authNavStatusSource.asObservable();

    private manager = new UserManager(getClientSettings());
    private user: User | null;

    constructor() {
        super();

        this.manager.getUser().then((user) => {
            this.user = user;
            this._authNavStatusSource.next(this.isAuthenticated());
        });
    }

    login() {
        return this.manager.signinRedirect();
    }

    async completeAuthentication() {
        this.user = await this.manager.signinRedirectCallback();
        this._authNavStatusSource.next(this.isAuthenticated());
    }

    isAuthenticated(): boolean {
        return this.user != null && !this.user.expired;
    }

    get authorizationHeaderValue(): string {
        if (this.user) {
            return `${this.user.token_type} ${this.user.access_token}`;
        }
        return null;
    }

    get name(): string {
        return this.user != null ? this.user.profile.name : "";
    }

    get profile() {
        return this.user != null ? this.user.profile : null;
    }
    async signout() {
        await this.manager.signoutRedirect();
    }
}

export function getClientSettings(): UserManagerSettings {
    return {
        authority: environment.authorityUrl,
        client_id: environment.clientId,
        redirect_uri: environment.adminUrl + "/auth-callback",
        post_logout_redirect_uri: environment.adminUrl,
        response_type: "code",
        scope: "api.knowledgespace openid profile",
        filterProtocolClaims: true,
        loadUserInfo: true,
        automaticSilentRenew: true,
        silent_redirect_uri: environment.adminUrl + "/silent-refresh.html",
    };
}
