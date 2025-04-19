import { NgClass } from '@angular/common';
import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  Output,
  ViewChild,
} from '@angular/core';

@Component({
  selector: 'tt-dropdown',
  imports: [NgClass],
  templateUrl: './dropdown.component.html',
  styleUrl: './dropdown.component.css',
})
export class DropdownComponent {
  @Input() alignment: 'left' | 'right' = 'left';
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
}
