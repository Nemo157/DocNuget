{{# if Assembly }}
<a href="/packages/{{ Assembly.Package.Id }}/{{ Assembly.Package.Version }}/assemblies/{{ Assembly.Name }}/types/{{ FullName }}">{{ Name }}</a>
{{ else }}
{{ Name }}
{{/ if }}
{{# if GenericArguments }}&lt;{{# each GenericArguments }}{{> type.link }}{{/ each }}&gt;{{/ if }}
