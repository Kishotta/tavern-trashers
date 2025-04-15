import { Component } from '@angular/core';
import { SideNavComponent } from '../side-nav/side-nav.component';
import { MainNavComponent } from '../main-nav/main-nav.component';
import { LayoutService } from '../../state/layout/layout.service';

@Component({
  selector: 'app-layout',
  imports: [SideNavComponent, MainNavComponent],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
})
export class LayoutComponent {
  constructor(private _: LayoutService) {}
}
