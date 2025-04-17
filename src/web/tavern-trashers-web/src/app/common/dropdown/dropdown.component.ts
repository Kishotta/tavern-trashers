import { AsyncPipe, NgTemplateOutlet } from '@angular/common';
import {
  Component,
  ContentChild,
  ElementRef,
  HostListener,
  Injectable,
  TemplateRef,
} from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { Observable } from 'rxjs';
import {
  trigger,
  transition,
  style,
  animate,
  state,
} from '@angular/animations';

export interface DropdownState {
  isOpen: boolean;
}

@Injectable()
export class DropdownStore extends ComponentStore<DropdownState> {
  constructor() {
    super({ isOpen: false });
  }

  readonly close = this.updater((state) => ({
    ...state,
    isOpen: false,
  }));

  readonly toggle = this.updater((state) => ({
    ...state,
    isOpen: !state.isOpen,
  }));

  readonly isOpen$: Observable<boolean> = this.select((state) => state.isOpen);
}

@Component({
  selector: 'tt-dropdown',
  imports: [AsyncPipe, NgTemplateOutlet],
  templateUrl: './dropdown.component.html',
  styleUrl: './dropdown.component.css',
  animations: [
    trigger('dropdown', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(.95)' }),
        animate('100ms ease-out', style({ opacity: 1, transform: 'scale(1)' })),
      ]),
      transition(':leave', [
        style({ opacity: 1, transform: 'scale(1)' }),
        animate('75ms ease-in', style({ opacity: 0, transform: 'scale(.95)' })),
      ]),
    ]),
  ],
  providers: [DropdownStore],
})
export class DropdownComponent {
  @ContentChild('trigger', { static: true })
  trigger!: TemplateRef<any>;

  @ContentChild('menu', { static: true })
  menu!: TemplateRef<any>;

  @HostListener('document:mousedown', ['$event'])
  onGlobalClick(event: MouseEvent) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.dropdownStore.close();
    }
  }

  // Close when focus moves outside the dropdown
  @HostListener('document:focusout', ['$event'])
  onGlobalFocusOut(event: FocusEvent) {
    const target = event.relatedTarget as HTMLElement;
    if (target && !this.elementRef.nativeElement.contains(target)) {
      this.dropdownStore.close();
    }
  }

  readonly isOpen$: Observable<boolean>;

  constructor(
    private elementRef: ElementRef,
    private dropdownStore: DropdownStore
  ) {
    this.isOpen$ = this.dropdownStore.isOpen$;
  }

  toggle() {
    this.dropdownStore.toggle();
  }

  close() {
    this.dropdownStore.close();
  }
}
