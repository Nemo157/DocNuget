<div class="page-header">
  <h1>{{> type.name }}{{# if AllBaseTypes }} <small>: {{# join AllBaseTypes ', ' }}{{> type.link }}{{/ join }}</small>{{/ if }}</h1>
</div>

<p>{{ Summary }}</p>

<dl class="dl-horizontal">
  <dt>Namespace</dt><dd>{{> namespace.link Namespace }}</dd>
  <dt>Assembly</dt><dd>{{> assembly.link Assembly }}</dd>
</dl>

{{# if Methods }}
  <h2>Methods</h2>
  {{# each Methods as |Method| }}
    <div>
      <code>
        {{ Visibility }}
        {{# if IsStatic }}static {{/ if }}
        {{> type.link Method.ReturnType }}
        {{ Method.Name }}{{# if GenericParameters }}&lt;{{# join GenericParameters ', ' }}{{ Name }}{{/ join }}&gt;{{/ if }}({{# join Parameters ', ' }}{{> type.link Type }} {{ Name }}{{/ join }})
        {{# each GenericParameters }}
          {{# if Constraints }}
            where {{ Name }} : {{# join Constraints ', ' }}{{> type.link }}{{/ join }}
          {{/ if }}
        {{/ each }}
      </code>
      <p>{{ Summary }}</p>
    </div>
  {{/ each }}
{{/ if }}
