@page "/{{Route}}"
@inject {{EntityName}}Service {{EntityName}}Service

<PageTitle>{{PluralEntityName}}</PageTitle>

<h1>{{PluralEntityName}}</h1>

@if (_loading)
{
    <p>Loading...</p>
}
else if (!string.IsNullOrWhiteSpace(_error))
{
    <p class="text-danger">@_error</p>
}
else
{
    <table class="table">
        <thead>
            <tr>
{{Columns}}
            </tr>
        </thead>
        <tbody>
            @foreach (var item in _items)
            {
                <tr>
{{Properties}}
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private IReadOnlyList<{{EntityName}}Model> _items = [];
    private bool _loading;
    private string? _error;

    protected override async Task OnInitializedAsync()
    {
        _loading = true;

        try
        {
            _items = await {{EntityName}}Service.GetAllAsync();
        }
        catch
        {
            _error = "Unable to load {{PluralEntityName}}.";
        }
        finally
        {
            _loading = false;
        }
    }
}
