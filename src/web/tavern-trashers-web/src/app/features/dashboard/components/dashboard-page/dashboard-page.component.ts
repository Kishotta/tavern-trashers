import { Component } from '@angular/core';
import { AvatarComponent } from '../../../../common/avatar/components/avatar/avatar.component';
import { AvatarGroupComponent } from '../../../../common/avatar/components/avatar-group/avatar-group.component';
import { BadgeComponent } from '../../../../common/badge/components/badge/badge.component';
import { ButtonComponent } from '../../../../common/button/components/button/button.component';
import { DropdownComponent } from '../../../../common/dropdown/components/dropdown/dropdown.component';

@Component({
  selector: 'app-dashboard-page',
  imports: [
    AvatarComponent,
    AvatarGroupComponent,
    BadgeComponent,
    ButtonComponent,
    DropdownComponent,
  ],
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.css',
})
export class DashboardPageComponent {
  logDismissed() {
    console.log('dismissed');
  }
}
