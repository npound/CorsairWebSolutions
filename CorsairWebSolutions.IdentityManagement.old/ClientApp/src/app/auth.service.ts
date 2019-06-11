import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http : HttpClient) { }

  login(username,password,rememberLogin,returnUrl,login){

    var params = new HttpParams()
    .set('username',username)
    .set('password', password)
    .set('rememberLogin',rememberLogin)
    .set('returnUrl',returnUrl)
    .set('login', login)
    
    return this.http.post('/Account/Login',{},{ params : params});
  }
}
