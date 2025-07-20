import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SideNavLink } from './side-nav-link.component';

@Component({
  selector: 'app-side-nav',
  imports: [RouterLink, SideNavLink],
  templateUrl: './side-nav.component.html',
  styleUrl: './side-nav.component.css',
})
export class SideNavComponent {}
