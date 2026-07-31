import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { {{EntityName}}Model } from './{{Route}}.model';

@Injectable({
  providedIn: 'root'
})
export class {{EntityName}}Service {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = '/api/v{{ApiVersion}}/{{Route}}';

  getAllAsync(): Promise<{{EntityName}}Model[]> {
    return firstValueFrom(this.httpClient.get<{{EntityName}}Model[]>(this.baseUrl));
  }

  getByIdAsync(id: {{PrimaryKeyType}}): Promise<{{EntityName}}Model> {
    return firstValueFrom(this.httpClient.get<{{EntityName}}Model>(`${this.baseUrl}/${id}`));
  }

  createAsync(model: {{EntityName}}Model): Promise<{{PrimaryKeyType}}> {
    return firstValueFrom(this.httpClient.post<{{PrimaryKeyType}}>(this.baseUrl, model));
  }

  updateAsync(id: {{PrimaryKeyType}}, model: {{EntityName}}Model): Promise<void> {
    return firstValueFrom(this.httpClient.put<void>(`${this.baseUrl}/${id}`, model));
  }

  deleteAsync(id: {{PrimaryKeyType}}): Promise<void> {
    return firstValueFrom(this.httpClient.delete<void>(`${this.baseUrl}/${id}`));
  }
}
