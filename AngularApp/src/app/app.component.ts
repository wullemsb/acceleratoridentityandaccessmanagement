import { Component, OnDestroy, OnInit } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { filter, take } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
    selector: 'app-root',
    templateUrl: 'app.component.html',
})
export class AppComponent implements OnInit, OnDestroy {
    isAuthenticated: boolean;
    userData: any;

    constructor(public oidcSecurityService: OidcSecurityService, public httpClient: HttpClient) {
        if (this.oidcSecurityService.moduleSetup) {
            this.doCallbackLogicIfRequired();
        } else {
            this.oidcSecurityService.onModuleSetup.subscribe(() => {
                this.doCallbackLogicIfRequired();
            });
        }
    }

    ngOnInit() {
        this.oidcSecurityService.getIsAuthorized().subscribe(auth => {
            this.isAuthenticated = auth;
        });

        this.oidcSecurityService.getUserData().subscribe(userData => {
            this.userData = userData;
        });
    }

    ngOnDestroy(): void {}

    login() {
        this.oidcSecurityService.authorize();
    }

    logout() {
        this.oidcSecurityService.logoff();
    }

    async callApi() {

      const httpOptions = {
        headers: this.getHeaders()
      };

      const response = await this.httpClient.get('https://localhost:44332/identity', httpOptions).toPromise();
      console.log(response);
    }

    private  getHeaders(): HttpHeaders {
     let headers = new HttpHeaders();
     headers = headers.set('Content-Type', 'application/json');
     headers = headers.set('Accept', 'application/json');

     const token = this.oidcSecurityService.getToken();
     if (token !== '') {
        const tokenValue = 'Bearer ' + token;
        headers = headers.set('Authorization', tokenValue);
      }
     return headers;
    }

    private doCallbackLogicIfRequired() {
        // Will do a callback, if the url has a hash parameter.
        this.oidcSecurityService.authorizedCallbackWithCode(window.location.toString());
    }
}
