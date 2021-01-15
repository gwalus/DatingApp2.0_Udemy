import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}
  //loggedIn: boolean = false;
  // Pierwsze podejście

  //currentUser$: Observable<any>;
  // Drugie podejście

  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) { }
  // Zmiana z prywatnego na publiczny, żeby mieć dostęp do serwisu w pliku html

  ngOnInit(): void {
    // this.getCurrentUser();
    // Pierwsze podejście

    //this.currentUser$ = this.accountService.currentUser$;
    // Drugie podejście
  }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
      //this.loggedIn = true;
      this.toastr.success('Login successfully');
      this.router.navigateByUrl('members');
    })
    // }, error => {
    //   console.log(error);
    //   this.toastr.error(error.error);
    // });
  }

  logout() {
    this.accountService.logout();
    this.toastr.success('Logged out');
    //this.loggedIn = false;
    this.router.navigateByUrl('/');
  }

  // getCurrentUser() {
  //   this.accountService.currentUser$.subscribe(user => {
  //     //this.loggedIn = !!user;
  //   }, error => {
  //     console.log(error);
  //   })
  // }
  // Pierwsze podejście
}
