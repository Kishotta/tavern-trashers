import { Component, Input, booleanAttribute } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ButtonComponent } from "../../../button/components/button/button.component";

@Component({
  selector: 'tt-page-heading',
  imports: [ButtonComponent],
  templateUrl: './page-heading.component.html',
  styleUrl: './page-heading.component.css',
})
export class PageHeadingComponent {
  @Input({ required: true }) title: string = 'Page Title';

  @Input({ transform: booleanAttribute }) backButton: boolean = false;

  constructor(private router: Router, private route: ActivatedRoute) {}

  onBackButtonClick() {
    this.router.navigate(['..'], { relativeTo: this.route });
  }
}
