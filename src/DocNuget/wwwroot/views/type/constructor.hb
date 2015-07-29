{{# ifAccessible Constructor }}
  <div>
    {{ accessibilityDebug Constructor }}
    <h5>
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
