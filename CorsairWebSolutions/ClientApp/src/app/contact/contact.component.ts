import { Component, OnInit } from '@angular/core';
import { EmailService } from '../services/email.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {

  constructor(private emailSvc : EmailService) { }

message ="";
name ="";
email ="";


  ngOnInit() {
  }

  sendMessage()
  {
    this.emailSvc.sendEmail(this.name,this.email,this.message)
    .subscribe( s => {
      this.message ="";
      this.name ="";
      this.email ="";
    })
  }

}
