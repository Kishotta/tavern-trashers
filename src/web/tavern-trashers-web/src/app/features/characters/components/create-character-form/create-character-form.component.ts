import { Component, Input } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { createCharacter } from '../../store/characters.actions';

@Component({
  selector: 'app-create-character-form',
  imports: [ReactiveFormsModule],
  templateUrl: './create-character-form.component.html',
  styleUrl: './create-character-form.component.css',
})
export class CreateCharacterFormComponent {
  @Input() campaignId!: string;

  protected form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private store: Store
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      level: [1, [Validators.required, Validators.min(1), Validators.max(20)]],
    });
  }

  onSubmit(): void {
    if (this.form.valid && this.campaignId) {
      const { name, level } = this.form.value;
      this.store.dispatch(createCharacter({ name, level, campaignId: this.campaignId }));
      this.form.reset({ name: '', level: 1 });
    }
  }
}
