{{# ifAccessible Constructor }}
  <div>
    <code>
      {{ Accessibility }}
      {{ replace Type.Name '`\d+$' '' ~}}
      (
        {{~# join Constructor.Parameters ', ' ~}}
          {{~> type.link Package=../Package Assembly=../Assembly Type=Type }}
          {{ Name ~}}
        {{~/ join ~}}
      )
    </code>
    <p>{{ Constructor.Summary }}</p>
  </div>
{{/ ifAccessible }}
