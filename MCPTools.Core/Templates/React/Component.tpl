import { use{{EntityName}} } from './use{{EntityName}}';

export function {{EntityName}}Component() {
  const { items, loading, error, reloadAsync } = use{{EntityName}}();

  if (loading) {
    return <p>Loading...</p>;
  }

  if (error) {
    return <p role="alert">{error}</p>;
  }

  return (
    <section className="{{Route}}">
      <header>
        <h1>{{PluralEntityName}}</h1>
        <button type="button" onClick={reloadAsync}>Refresh</button>
      </header>
      <table>
        <thead>
          <tr>
{{Columns}}
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.{{PrimaryKey}}>
{{Properties}}
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
