<section class="{{Route}}">
  <header class="{{Route}}__header">
    <h1>{{PluralEntityName}}</h1>
  </header>

  @if (loading()) {
    <p>Loading...</p>
  } @else if (error()) {
    <p class="{{Route}}__error">{{ error() }}</p>
  } @else {
    <table class="{{Route}}__table">
      <thead>
        <tr>
{{Columns}}
        </tr>
      </thead>
      <tbody>
        @for (item of items(); track item.{{PrimaryKey}}) {
          <tr>
{{Properties}}
          </tr>
        }
      </tbody>
    </table>
  }
</section>
