import { Component, Input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'tt-side-nav-link',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <a
      [routerLink]="routerLink"
      routerLinkActive="bg-gray-800 text-white"
      [routerLinkActiveOptions]="routerLinkActiveOptions"
      class="group flex gap-x-3 rounded-md p-3 text-sm/6 font-semibold text-gray-400 hover:bg-gray-800 hover:text-white"
    >
      <div class="size-6 shrink-0">
        <i [class]="iconClass"></i>
      </div>
      <span class="sr-only">{{ label }}</span>
    </a>
  `,
  styles: `
    :host {
        display: contents;
    }`,
})
export class SideNavLink {
  @Input() routerLink!: string | any[];
  @Input() label!: string;
  @Input() iconClass!: string;
  @Input() routerLinkActiveOptions: any = {};
}
