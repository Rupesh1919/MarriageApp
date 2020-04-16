import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/Auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }
  Login(){
    // console.log(this.model);
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('Logged Sucessfully');

    }, error => {
      this.alertify.error(error);
    });
   }
   loggedIn(){
     return this.authService.loggedIn();
   }
   logout(){
     localStorage.removeItem('token');
     this.alertify.message('logout sucessfully');
   }

}
