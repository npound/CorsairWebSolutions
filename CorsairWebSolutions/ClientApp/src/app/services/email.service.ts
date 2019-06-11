import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class EmailService {

  constructor(private http : HttpClient) { }

sendEmail(name,email,message)
{
  var content = {
    name,
    email,
    message
  }
return this.http.post("/api/email",content)
}


}
