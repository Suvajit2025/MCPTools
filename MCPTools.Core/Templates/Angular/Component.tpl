import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { {{EntityName}}Service } from './{{Route}}.service';
import { {{EntityName}}Model } from './{{Route}}.model';

@Component({
  selector: 'app-{{Route}}',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './{{Route}}.component.html',
  styleUrl: './{{Route}}.component.css'
})
export class {{EntityName}}Component {
  private readonly service = inject({{EntityName}}Service);

  protected readonly items = signal<{{EntityName}}Model[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  async ngOnInit(): Promise<void> {
    await this.loadAsync();
  }

  protected async loadAsync(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const items = await this.service.getAllAsync();
      this.items.set(items);
    } catch {
      this.error.set('Unable to load {{PluralEntityName}}.');
    } finally {
      this.loading.set(false);
    }
  }
}
