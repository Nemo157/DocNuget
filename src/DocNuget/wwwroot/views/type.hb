<div class="page-header">
  <h1>{{> type.name }}{{# if AllBaseTypes }} <small>: {{# join AllBaseTypes ', ' }}{{> type.link }}{{/ join }}</small>{{/ if }}</h1>
</div>

<dl class="dl-horizontal">
  <dt>Namespace</dt><dd>{{> namespace.link Namespace }}</dd>
  <dt>Assembly</dt><dd>{{> assembly.link Assembly }}</dd>
</dl>

{{# if Methods }}
  <h2>Methods</h2>
  {{# each Methods as |Method| }}
    <div>
      <code>{{ Visibility }} {{# if IsStatic }}static {{/ if }}{{> type.link Method.ReturnType }} {{ Method.Name }}({{# join Parameters ', ' }}{{> type.link Type }} {{ Name }}{{/ join }})</code>
    </div>
  {{/ each }}
{{/ if }}
