import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthFacade } from '../../../../state/auth/auth.facade';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-user-registration-page',
  imports: [ReactiveFormsModule, JsonPipe],
  templateUrl: './user-registration-page.component.html',
  styleUrl: './user-registration-page.component.css',
})
export class UserRegistrationPageComponent {
  protected formGroup: FormGroup;

  constructor(private formBuilder: FormBuilder, private auth: AuthFacade) {
    this.formGroup = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      //   confirmPassword: ['', Validators.required],
    });
  }

  register(): void {
    if (this.formGroup.valid) {
      const { firstName, lastName, email, password } = this.formGroup.value;
      this.auth.register({ firstName, lastName, email, password });
    }
  }
}
