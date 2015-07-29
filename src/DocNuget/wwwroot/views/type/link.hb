{{# if Type.InAssembly }}
<a href="/packages/{{ Package.Id }}/{{ Package.Version }}/assemblies/{{ Assembly.Name }}/types/{{ Type.FullName }}">{{ Type.Name }}</a>
{{ else }}
{{ Type.Name }}
{{/ if }}
{{# if Type.GenericArguments }}&lt;{{# each Type.GenericArguments as |GenericArgument| }}{{> type.link Package=../Package Assembly=../Assembly Type=GenericArgument }}{{/ each }}&gt;{{/ if }}
