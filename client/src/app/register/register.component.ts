import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  @Input() usersFromHomeComponent: any;
  @Output() registerCancel = new EventEmitter();

  constructor(private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  register() {
    // console.log(this.model);
    this.accountService.register(this.model).subscribe(response => {
      console.log(response);
      this.toastr.success('You created an account successfully');
      this.cancel();
    }, error => {
      console.log(error);
      this.toastr.error(error.error);
    });
  }

  cancel() {
    // console.log('canceled');
    this.registerCancel.emit(false);
  }
}
