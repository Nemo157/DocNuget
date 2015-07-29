{{# each Namespace.Namespaces as |Namespace| }}
  {{# ifAccessible Namespace }}
    <li>
      {{ accessibilityDebug Namespace }}
      {{> namespace.tree-node Package=../../Package Assembly=../../Assembly Namespace=Namespace }}
    </li>
  {{/ ifAccessible }}
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
