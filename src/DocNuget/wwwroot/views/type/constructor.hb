{{# ifAccessible Constructor }}
  <div>
    <h5>
      {{ accessibilityDebug Constructor }}
      {{ Accessibility }}
      {{ replace Type.Name '`\d+$' '' ~}}
      (
        {{~# join Constructor.Parameters ', ' ~}}
          {{~> type.link Package=../Package Assembly=../Assembly Type=Type }}
          {{ Name ~}}
        {{~/ join ~}}
      )
    </h5>
    <p>{{ Constructor.Summary }}</p>
  </div>
{{/ ifAccessible }}
