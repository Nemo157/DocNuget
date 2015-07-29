<button class="btn btn-link btn-xs" data-toggle="collapse" data-target="#collapse-ns-{{ replace Namespace.FullName '\.' '-' }}">
  <span class="glyphicon glyphicon-triangle-right"></span>
</button>
<b>
  <a href="/packages/{{ Package.Id }}/{{ Package.Version }}/assemblies/{{ Assembly.Name }}/{{ Assembly.Framework }}/namespaces/{{ Namespace.FullName }}">{{ Namespace.Name }}</a>
</b>
<ul id="collapse-ns-{{ replace Namespace.FullName '\.' '-' }}" class="collapse in" style="list-style: none">
  {{# each Namespace.Namespaces as |Namespace| }}
    <li>{{> namespace Package=../Package Assembly=../Assembly Namespace=Namespace }}</li>
  {{/ each }}
  {{# each Namespace.Types as |Type| }}
    {{# ifAccessible Type }}
      <li>
        {{ accessibilityDebug Type }}
        <a href="/packages/{{ ../../Package.Id }}/{{ ../../Package.Version }}/assemblies/{{ ../../Assembly.Name }}/types/{{ Type.FullName }}">
          {{ Type.Name }}
        </a>
        </li>
    {{/ ifAccessible }}
  {{/ each }}
</ul>
