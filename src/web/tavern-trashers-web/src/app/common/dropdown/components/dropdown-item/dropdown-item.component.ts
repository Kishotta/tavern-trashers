import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'tt-dropdown-item',
  imports: [],
  templateUrl: './dropdown-item.component.html',
  styleUrl: './dropdown-item.component.css',
})
export class DropdownItemComponent {
  @Input() icon: string = '';

  @Output() clicked: EventEmitter<void> = new EventEmitter<void>();

  onClick() {
    this.clicked.emit();
  }
}
