<button class="btn btn-link btn-xs" data-toggle="collapse" data-target="#collapse-ns-{{ replace FullName '.' '-' }}">
  <span class="glyphicon glyphicon-triangle-right"></span>
</button>
<b>
  <a href="/packages/{{ Assembly.Package.Id }}/{{ Assembly.Package.Version }}/assemblies/{{ Assembly.Name }}/{{ Assembly.Framework }}/namespaces/{{ FullName }}">{{ Name }}</a>
</b>
<ul id="collapse-ns-{{ replace FullName '.' '-' }}" class="collapse" style="list-style: none">
  {{# each Namespaces as |Namespace| }}
    <li>{{> namespace Namespace }}</li>
  {{/ each }}
  {{# each Types as |Type| }}
    <li><a href="/packages/{{ ../Assembly.Package.Id }}/{{ ../Assembly.Package.Version }}/assemblies/{{ ../Assembly.Name }}/{{ ../Assembly.Framework }}/namespaces/{{ ../Name }}/types/{{ Type.Name }}">{{ Type.Name }}</a></li>
  {{/ each }}
</ul>
