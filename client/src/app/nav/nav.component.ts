import { Component, OnInit } from '@angular/core';
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

  constructor(public accountService: AccountService) { }
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
    }, error => {
      console.log(error);
    });
  }

  logout() {
    this.accountService.logout();
    //this.loggedIn = false;
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
