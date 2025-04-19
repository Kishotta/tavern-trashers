import { booleanAttribute, Component, Input } from '@angular/core';
import { Profile } from '../../models/profile';

const ExtraSmall = 'xs';
const Small = 'sm';
const Medium = 'md';
const Large = 'lg';
const ExtraLarge = 'xl';

type AvatarSize =
  | typeof ExtraSmall
  | typeof Small
  | typeof Medium
  | typeof Large
  | typeof ExtraLarge;

const Circle = 'circle';
const Square = 'square';
type AvatarShape = typeof Circle | typeof Square;

const StatusNone = 'none';
const StatusTop = 'top';
const StatusBottom = 'bottom';

const StatusGray = 'gray';
const StatusGreen = 'green';
const StatusRed = 'red';

type StatusPosition =
  | typeof StatusNone
  | typeof StatusTop
  | typeof StatusBottom;
type StatusColor = typeof StatusGray | typeof StatusGreen | typeof StatusRed;

@Component({
  selector: 'tt-avatar',
  imports: [],
  templateUrl: './avatar.component.html',
  styleUrl: './avatar.component.css',
})
export class AvatarComponent {
  @Input() profile: Profile | null = null;

  @Input({ transform: booleanAttribute })
  withText: boolean = false;

  @Input() size: AvatarSize = Medium;

  get sizeClasses(): string {
    switch (this.size) {
      case ExtraSmall:
        return 'size-6';
      case Small:
        return 'size-8';
      case Medium:
        return 'size-10';
      case Large:
        return 'size-12';
      case ExtraLarge:
        return 'size-14';
    }
  }

  @Input() shape: AvatarShape = Circle;

  get shapeClasses(): string {
    return this.shape === 'circle' ? 'rounded-full' : 'rounded-md';
  }

  @Input() statusPosition: StatusPosition = StatusNone;
  @Input() statusColor: StatusColor = StatusGray;

  get statusPositionClasses(): string {
    switch (this.statusPosition) {
      case StatusNone:
        return 'hidden';
      case StatusTop:
        return 'top-0';
      case StatusBottom:
        return 'bottom-0';
    }
  }

  get statusPositionOffsetClasses(): string {
    if (this.shape === 'circle') return '';

    return this.statusPosition === StatusTop
      ? 'translate-x-1/2 -translate-y-1/2 transform'
      : 'translate-x-1/2 translate-y-1/2 transform';
  }

  get statusSizeClasses(): string {
    switch (this.size) {
      case ExtraSmall:
        return 'size-1.5';
      case Small:
        return 'size-2';
      case Medium:
        return 'size-2.5';
      case Large:
        return 'size-3';
      case ExtraLarge:
        return 'size-3.5';
    }
  }

  get statusColorClasses(): string {
    switch (this.statusColor) {
      case StatusGray:
        return 'bg-gray-300';
      case StatusGreen:
        return 'bg-green-400';
      case StatusRed:
        return 'bg-red-400';
    }
  }
}
