{{~ replace Type.Name '`\d+$' '' ~}}
{{~# if Type.GenericParameters ~}}
  &lt;
  {{~# join Type.GenericParameters ', ' ~}}
    {{~> type.link Package=../Package Assembly=../Assembly Type=. ~}}
  {{~/ join ~}}
  &gt;
{{~/ if ~}}
