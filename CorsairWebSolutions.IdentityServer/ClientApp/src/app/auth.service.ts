import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http : HttpClient) { }

  login(username,password,rememberLogin,returnUrl,login){

    let headers = new HttpHeaders()
    .set('Content-Type','application/x-www-form-urlencoded')
let body = returnUrl.replace('/connect/authorize?','')+'&username='+username+'&password='+password;
    return this.http.post(returnUrl,body, {headers : headers});
  }
}
