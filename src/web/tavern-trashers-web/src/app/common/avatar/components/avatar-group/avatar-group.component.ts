import { Component, Input } from '@angular/core';
import { Profile } from '../../models/profile';

const Small = 'sm';
const Medium = 'md';
const Large = 'lg';

type AvatarGroupSize = typeof Small | typeof Medium | typeof Large;

@Component({
  selector: 'tt-avatar-group',
  imports: [],
  templateUrl: './avatar-group.component.html',
  styleUrl: './avatar-group.component.css',
})
export class AvatarGroupComponent {
  @Input() size: AvatarGroupSize = Medium;

  @Input() profiles: Profile[] = [];

  get spacingClasses(): string {
    switch (this.size) {
      case Small:
        return '-space-x-1';
      case Medium:
        return '-space-x-2';
      case Large:
        return '-space-x-2';
    }
  }

  get sizingClasses(): string {
    switch (this.size) {
      case Small:
        return 'size-6';
      case Medium:
        return 'size-8';
      case Large:
        return 'size-10';
    }
  }

  get profileCount(): number {
    return this.profiles.length;
  }
}
