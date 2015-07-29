{{~ replace Type.Name '`\d+$' '' ~}}
{{~# if Type.GenericArguments ~}}
  &lt;
  {{~# join Type.GenericArguments ', ' ~}}
    {{~> type.link Package=../Package Assembly=../Assembly Type=. ~}}
  {{~/ join ~}}
  &gt;
{{~/ if ~}}
