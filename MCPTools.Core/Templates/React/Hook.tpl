import { useCallback, useEffect, useState } from 'react';
import { get{{PluralEntityName}}Async } from './{{Route}}ApiService';
import { {{EntityName}}Model } from './{{Route}}Model';

export function use{{EntityName}}() {
  const [items, setItems] = useState<{{EntityName}}Model[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const reloadAsync = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const result = await get{{PluralEntityName}}Async();
      setItems(result);
    } catch {
      setError('Unable to load {{PluralEntityName}}.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void reloadAsync();
  }, [reloadAsync]);

  return { items, loading, error, reloadAsync };
}
