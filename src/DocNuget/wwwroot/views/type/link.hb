{{~# if Type.InAssembly ~}}
  <a href="/packages/{{ Package.Id }}/{{ Package.Version }}/assemblies/{{ Assembly.Name }}/types/{{ Type.FullName }}">
    {{~ replace Type.Name '`\d+$' '' ~}}
  </a>
{{~ else ~}}
  {{~ replace Type.Name '`\d+$' '' ~}}
{{~/ if ~}}
{{~# if Type.GenericParameters ~}}
  &lt;
  {{~# join Type.GenericParameters ', ' ~}}
    {{~> type.link Package=../Package Assembly=../Assembly Type=. ~}}
  {{~/ join ~}}
  &gt;
{{~/ if ~}}
