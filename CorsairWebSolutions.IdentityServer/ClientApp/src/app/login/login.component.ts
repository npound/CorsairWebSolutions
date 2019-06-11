import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { ActivatedRoute} from '@angular/router';
import {map} from 'rxjs/operators'
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private auth : AuthService) { }
  username
  password
  ngOnInit() {

  }

  login()
  {

    let callback = decodeURIComponent(location.search.replace('?ReturnUrl=',""))
    this.auth.login(this.username,this.password,false,callback,true)
    .subscribe(s =>
      console.log(s),
      e => console.log(e))

    
  }
}
