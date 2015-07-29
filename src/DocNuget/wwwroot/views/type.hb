<div class="page-header">
  <h1>{{> type.name Package=Package Assembly=Assembly Type=Type }}{{# if Type.AllBaseTypes }} <small>: {{# join Type.AllBaseTypes ', ' }}{{> type.link Package=../Package Assembly=../Assembly Type=. }}{{/ join }}</small>{{/ if }}</h1>
</div>

<p>{{ Type.Summary }}</p>

<dl class="dl-horizontal">
  <dt>Namespace</dt><dd>{{> namespace.link Package=Package Assembly=Assembly Namespace=Namespace }}</dd>
  <dt>Assembly</dt><dd>{{> assembly.link Package=Package Assembly=Assembly }}</dd>
</dl>

{{# if Type.Methods }}
  <h2>Methods</h2>
  {{# each Type.Methods as |Method| }}
    <div>
      <code>
        {{ Visibility }}
        {{# if Method.IsStatic }}static {{/ if }}
        {{> type.link Package=../Package Assembly=../Assembly Type=Method.ReturnType }}
        {{ Method.Name }}
        {{~# if Method.GenericParameters ~}}
          &lt;
            {{~# join Method.GenericParameters ', ' ~}}
              {{~ Name ~}}
            {{~/ join ~}}
          &gt;
        {{~/ if ~}}
        (
          {{~# join Method.Parameters ', ' ~}}
            {{~> type.link Package=../../Package Assembly=../../Assembly Type=Type }}
            {{ Name ~}}
          {{~/ join ~}}
        )
        {{~# each Method.GenericParameters as |GenericParameter| ~}}
          {{~# if GenericParameter.Constraints ~}}
            where {{ GenericParameter.Name }} :
            {{~# join GenericParameter.Constraints ', ' ~}}
              {{~> type.link Package=../../Package Assembly=../../Assembly Type=. ~}}
            {{~/ join ~}}
          {{~/ if ~}}
        {{~/ each ~}}
      </code>
      <p>{{ Method.Summary }}</p>
    </div>
  {{/ each }}
{{/ if }}
