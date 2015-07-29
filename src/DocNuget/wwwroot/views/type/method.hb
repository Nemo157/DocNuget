{{# ifAccessible Method }}
  <div>
    <code>
      {{ Method.Accessibility }}
      {{# if Method.IsStatic }}static {{/ if }}
      {{> type.link Package=Package Assembly=Assembly Type=Method.ReturnType }}
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
          {{~> type.link Package=../Package Assembly=../Assembly Type=Type }}
          {{ Name ~}}
        {{~/ join ~}}
      )
      {{~# each Method.GenericParameters as |GenericParameter| ~}}
        {{~# if GenericParameter.Constraints ~}}
          where {{ GenericParameter.Name }} :
          {{~# join GenericParameter.Constraints ', ' ~}}
            {{~> type.link Package=../Package Assembly=../Assembly Type=. ~}}
          {{~/ join ~}}
        {{~/ if ~}}
      {{~/ each ~}}
    </code>
    <p>{{ Method.Summary }}</p>
  </div>
{{/ ifAccessible }}
