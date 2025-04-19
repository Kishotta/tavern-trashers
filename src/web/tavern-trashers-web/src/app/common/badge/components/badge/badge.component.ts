import {
  booleanAttribute,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';

const Gray = 'gray';
const Red = 'red';
const Yellow = 'yellow';
const Green = 'green';
const Blue = 'blue';
const Indigo = 'indigo';
const Purple = 'purple';
const Pink = 'pink';

type BadgeColor =
  | typeof Gray
  | typeof Red
  | typeof Yellow
  | typeof Green
  | typeof Blue
  | typeof Indigo
  | typeof Purple
  | typeof Pink;

@Component({
  selector: 'tt-badge',
  imports: [],
  templateUrl: './badge.component.html',
  styleUrl: './badge.component.css',
})
export class BadgeComponent {
  @Input({ required: true }) text: string = '';

  @Input({ transform: booleanAttribute }) small: boolean = false;

  get sizeClasses() {
    return this.small ? 'px-1.5 py-0.5' : 'px-2 py-1';
  }

  @Input({ transform: booleanAttribute }) flat: boolean = false;

  @Input() color: BadgeColor = Gray;

  get colorClasses() {
    return this.flat ? this.flatColorClasses : this.borderedColorClasses;
  }

  get flatColorClasses() {
    switch (this.color) {
      case Gray:
        return 'bg-gray-100 text-gray-600';
      case Red:
        return 'bg-red-100 text-red-700';
      case Yellow:
        return 'bg-yellow-100 text-yellow-800';
      case Green:
        return 'bg-green-100 text-green-700';
      case Blue:
        return 'bg-blue-100 text-blue-700';
      case Indigo:
        return 'bg-indigo-100 text-indigo-700';
      case Purple:
        return 'bg-purple-100 text-purple-700';
      case Pink:
        return 'bg-pink-100 text-pink-700';
    }
  }

  get borderedColorClasses() {
    if (this.dot) return 'bg-white text-gray-900 ring-1 ring-gray-200';
    switch (this.color) {
      case Gray:
        return 'bg-gray-50 text-gray-600 ring-1 ring-gray-500/10';
      case Red:
        return 'bg-red-50 text-red-700 ring-1 ring-red-600/10';
      case Yellow:
        return 'bg-yellow-50 text-yellow-800 ring-1 ring-yellow-600/20';
      case Green:
        return 'bg-green-50 text-green-700 ring-1 ring-green-600/20';
      case Blue:
        return 'bg-blue-50 text-blue-700 ring-1 ring-blue-700/10';
      case Indigo:
        return 'bg-indigo-50 text-indigo-700 ring-1 ring-indigo-700/10';
      case Purple:
        return 'bg-purple-50 text-purple-700 ring-1 ring-purple-700/10';
      case Pink:
        return 'bg-pink-50 text-pink-700 ring-1 ring-pink-700/10';
    }
  }

  @Input({ transform: booleanAttribute }) pill: boolean = false;

  get borderRadiusClasses() {
    return this.pill ? 'rounded-full' : 'rounded-md';
  }

  @Input({ transform: booleanAttribute }) dot: boolean = false;

  get dotColorClasses() {
    switch (this.color) {
      case Gray:
        return 'fill-gray-500';
      case Red:
        return 'fill-red-500';
      case Yellow:
        return 'fill-yellow-500';
      case Green:
        return 'fill-green-500';
      case Blue:
        return 'fill-blue-500';
      case Indigo:
        return 'fill-indigo-500';
      case Purple:
        return 'fill-purple-500';
      case Pink:
        return 'fill-pink-500';
    }
  }

  @Input({ transform: booleanAttribute }) dismissable: boolean = false;

  @Output() dismissed: EventEmitter<void> = new EventEmitter<void>();

  get dismissButtonShapeClasses() {
    return this.pill ? 'rounded-full' : 'rounded-xs';
  }

  get dismissButtonColorClasses() {
    switch (this.color) {
      case Gray:
        return 'stroke-gray-700/50 group-hover:stroke-gray-700/75 hover:bg-gray-500/20  focus:bg-gray-500/20';
      case Red:
        return 'stroke-red-700/50 group-hover:stroke-red-700/75 hover:bg-red-600/20  focus:bg-red-600/20';
      case Yellow:
        return 'stroke-yellow-800/50 group-hover:stroke-yellow-800/75 hover:bg-yellow-600/20  focus:bg-yellow-600/20';
      case Green:
        return 'stroke-green-800/50 group-hover:stroke-green-800/75 hover:bg-green-600/20  focus:bg-green-600/20';
      case Blue:
        return 'stroke-blue-800/50 group-hover:stroke-blue-800/75 hover:bg-blue-600/20  focus:bg-blue-600/20';
      case Indigo:
        return 'stroke-indigo-700/50 group-hover:stroke-indigo-700/75 hover:bg-indigo-600/20  focus:bg-indigo-600/20';
      case Purple:
        return 'stroke-purple-700/50 group-hover:stroke-purple-700/75 hover:bg-purple-600/20  focus:bg-purple-600/20';
      case Pink:
        return 'stroke-pink-800/50 group-hover:stroke-pink-800/75 hover:bg-pink-600/20  focus:bg-pink-600/20';
    }
  }

  onDismiss() {
    this.dismissed.emit();
  }
}
