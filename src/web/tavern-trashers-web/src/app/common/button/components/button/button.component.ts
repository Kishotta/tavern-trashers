import {
  booleanAttribute,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';

const ExtraSmall = 'xs';
const Small = 'sm';
const Medium = 'md';
const Large = 'lg';
const ExtraLarge = 'xl';

type ButtonSize =
  | typeof ExtraSmall
  | typeof Small
  | typeof Medium
  | typeof Large
  | typeof ExtraLarge;

const Gray = 'gray';
const Red = 'red';
const Yellow = 'yellow';
const Green = 'green';
const Blue = 'blue';
const Indigo = 'indigo';
const Purple = 'purple';
const Pink = 'pink';

type ButtonColor =
  | typeof Gray
  | typeof Red
  | typeof Yellow
  | typeof Green
  | typeof Blue
  | typeof Indigo
  | typeof Purple
  | typeof Pink;

const Primary = 'primary';
const Secondary = 'secondary';
const Soft = 'soft';

type ButtonVariant = typeof Primary | typeof Secondary | typeof Soft;

@Component({
  selector: 'tt-button',
  imports: [],
  templateUrl: './button.component.html',
  styleUrl: './button.component.css',
})
export class ButtonComponent {
  @Input() text: string = '';

  @Input() size: ButtonSize = Medium;
  @Input() icon: string | null = null;

  get shapeClasses(): string {
    return this.icon ? this.circularSizeClasses : this.regularSizeClasses;
  }

  get circularSizeClasses(): string {
    switch (this.size) {
      case ExtraSmall:
        return 'rounded-full p-1 text-xs';
      case Small:
        return 'rounded-full p-1 text-sm';
      case Medium:
        return 'rounded-full p-1.5 text-sm';
      case Large:
        return 'rounded-full p-2 text-sm';
      case ExtraLarge:
        return 'rounded-full p-2.5 text-sm';
    }
  }

  get regularSizeClasses(): string {
    switch (this.size) {
      case ExtraSmall:
        return 'rounded-sm px-2 py-1 text-xs gap-x-1';
      case Small:
        return 'rounded-sm px-2 py-1 text-sm gap-x-1';
      case Medium:
        return 'rounded-md px-2.5 py-1.5 text-sm gap-x-1.5';
      case Large:
        return 'rounded-md px-3 py-2 text-sm gap-x-1.5';
      case ExtraLarge:
        return 'rounded-md px-3.5 py-2.5 text-sm gap-x-2';
    }
  }

  @Input() color: ButtonColor = Gray;
  @Input() variant: ButtonVariant = Primary;

  get colorClasses(): string {
    switch (this.variant) {
      case Primary:
        return this.primaryColorClasses;
      case Secondary:
        return this.secondaryColorClasses;
      case Soft:
        return this.softColorClasses;
    }
  }

  get iconColorClasses(): string {
    switch (this.variant) {
      case Primary:
      case Soft:
        return 'text-white';
      case Secondary:
        return 'text-gray-400 group-hover:text-gray-500 group-active:text-gray-600 group-focus-visible:outline-gray-500';
    }
  }

  get primaryColorClasses(): string {
    switch (this.color) {
      case Gray:
        return 'text-white bg-gray-600 hover:bg-gray-700 active:bg-gray-800 focus-visible:outline-gray-700';
      case Red:
        return 'text-white bg-red-600 hover:bg-red-500 active:bg-red-400 focus-visible:outline-red-500';
      case Yellow:
        return 'text-white bg-yellow-500 hover:bg-yellow-400 active:bg-yellow-300 focus-visible:outline-yellow-400';
      case Green:
        return 'text-white bg-green-600 hover:bg-green-500 active:bg-green-400 focus-visible:outline-green-500';
      case Blue:
        return 'text-white bg-blue-600 hover:bg-blue-500 active:bg-blue-400 focus-visible:outline-blue-500';
      case Indigo:
        return 'text-white bg-indigo-600 hover:bg-indigo-500 active:bg-indigo-400 focus-visible:outline-indigo-500';
      case Purple:
        return 'text-white bg-purple-600 hover:bg-purple-500 active:bg-purple-400 focus-visible:outline-purple-500';
      case Pink:
        return 'text-white bg-pink-600 hover:bg-pink-500 active:bg-pink-400 focus-visible:outline-pink-500';
    }
  }

  get secondaryColorClasses(): string {
    return 'bg-white hover:bg-gray-50 active:bg-gray-200 text-gray-900 ring-gray-300 focus-visible:outline-gray-300 ring-1';
  }

  get softColorClasses(): string {
    switch (this.color) {
      case Gray:
        return 'text-white bg-gray-500 hover:bg-gray-400 active:bg-gray-300 focus-visible:outline-gray-500';
      case Red:
        return 'text-white bg-red-500 hover:bg-red-400 active:bg-red-300 focus-visible:outline-red-500';
      case Yellow:
        return 'text-white bg-yellow-400 hover:bg-yellow-300 active:bg-yellow-200 focus-visible:outline-yellow-400';
      case Green:
        return 'text-white bg-green-500 hover:bg-green-400 active:bg-green-300 focus-visible:outline-green-500';
      case Blue:
        return 'text-white bg-blue-500 hover:bg-blue-400 active:bg-blue-300 focus-visible:outline-blue-500';
      case Indigo:
        return 'text-white bg-indigo-500 hover:bg-indigo-400 active:bg-indigo-300 focus-visible:outline-indigo-500';
      case Purple:
        return 'text-white bg-purple-500 hover:bg-purple-400 active:bg-purple-300 focus-visible:outline-purple-500';
      case Pink:
        return 'text-white bg-pink-500 hover:bg-pink-400 active:bg-pink-300 focus-visible:outline-pink-500';
    }
  }

  @Input({ transform: booleanAttribute }) rounded: boolean = false;

  @Input() leadingIcon: string | null = null;
  @Input() trailingIcon: string | null = null;

  @Output() clicked = new EventEmitter<MouseEvent>();

  onClick(event: MouseEvent): void {
    this.clicked.emit(event);
  }
}
