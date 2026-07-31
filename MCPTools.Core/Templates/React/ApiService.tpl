import { {{EntityName}}Model } from './{{Route}}Model';

const baseUrl = '/api/v{{ApiVersion}}/{{Route}}';

export async function get{{PluralEntityName}}Async(): Promise<{{EntityName}}Model[]> {
  const response = await fetch(baseUrl);
  return readJsonAsync<{{EntityName}}Model[]>(response);
}

export async function get{{EntityName}}ByIdAsync(id: {{PrimaryKeyType}}): Promise<{{EntityName}}Model> {
  const response = await fetch(`${baseUrl}/${id}`);
  return readJsonAsync<{{EntityName}}Model>(response);
}

export async function create{{EntityName}}Async(model: {{EntityName}}Model): Promise<{{PrimaryKeyType}}> {
  const response = await fetch(baseUrl, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(model)
  });

  return readJsonAsync<{{PrimaryKeyType}}>(response);
}

export async function update{{EntityName}}Async(id: {{PrimaryKeyType}}, model: {{EntityName}}Model): Promise<void> {
  const response = await fetch(`${baseUrl}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(model)
  });

  ensureSuccess(response);
}

export async function delete{{EntityName}}Async(id: {{PrimaryKeyType}}): Promise<void> {
  const response = await fetch(`${baseUrl}/${id}`, { method: 'DELETE' });
  ensureSuccess(response);
}

async function readJsonAsync<T>(response: Response): Promise<T> {
  ensureSuccess(response);
  return await response.json() as T;
}

function ensureSuccess(response: Response): void {
  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}.`);
  }
}
