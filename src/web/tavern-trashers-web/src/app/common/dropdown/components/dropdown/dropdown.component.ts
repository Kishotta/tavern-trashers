import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  Output,
  ViewChild,
} from '@angular/core';
import { trigger, transition, style, animate } from '@angular/animations';

const Fit = 'fit';
const ExtraSmall = 'xs';
const Small = 'sm';
const Medium = 'md';
const Large = 'lg';
const ExtraLarge = 'xl';

type DropdownSize =
  | typeof Fit
  | typeof ExtraSmall
  | typeof Small
  | typeof Medium
  | typeof Large
  | typeof ExtraLarge;

@Component({
  selector: 'tt-dropdown',
  imports: [],
  templateUrl: './dropdown.component.html',
  styleUrl: './dropdown.component.css',
  animations: [
    trigger('dropdownAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(0.95)' }),
        animate('100ms ease-out', style({ opacity: 1, transform: 'scale(1)' })),
      ]),
      transition(':leave', [
        animate(
          '100ms ease-in',
          style({ opacity: 0, transform: 'scale(0.95)' })
        ),
      ]),
    ]),
  ],
})
export class DropdownComponent {
  @Input() alignment: 'left' | 'right' = 'left';
  @Input() size: DropdownSize = Fit;

  @Output() toggled = new EventEmitter<boolean>();

  @ViewChild('dropdownRoot') dropdownRoot!: ElementRef;

  isOpen = false;

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  closeDropdown() {
    this.isOpen = false;
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: MouseEvent): void {
    if (!this.dropdownRoot?.nativeElement.contains(event.target)) {
      this.closeDropdown();
    }
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    this.closeDropdown();
  }

  @HostListener('document:focusin', ['$event'])
  onFocusOut(event: FocusEvent): void {
    if (!this.dropdownRoot?.nativeElement.contains(event.target)) {
      this.closeDropdown();
    }
  }

  get alignmentClasses(): string {
    switch (this.alignment) {
      case 'left':
        return 'left-0';
      case 'right':
        return 'right-0';
      default:
        return ''; // Default alignment
    }
  }

  get sizeClasses(): string {
    switch (this.size) {
      case Fit:
        return 'w-screen mobile:w-auto';
      case ExtraSmall:
        return 'w-screen mobile:w-24';
      case Small:
        return 'w-screen mobile:w-36';
      case Medium:
        return 'w-screen mobile:w-54';
      case Large:
        return 'w-screen mobile:w-72';
      case ExtraLarge:
        return 'w-screen mobile:w-96';
      default:
        return 'w-screen mobile:w-54';
    }
  }
}
