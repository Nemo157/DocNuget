<div class="page-header">
  <h6>package {{> package.link Package=Package }}</h6>
  <h5>assembly {{> assembly.link Package=Package Assembly=Assembly }}</h5>
  <h4>namespace {{> namespace.link Package=Package Assembly=Assembly Namespace=Namespace }}</h4>
  <h3>
      {{ Type.Accessibility }}
      {{ Type.Construct }}
      {{> type.name Package=Package Assembly=Assembly Type=Type ~}}
      {{~# if Type.AllBaseTypes }}
        <small>:
        {{# join Type.AllBaseTypes ', ' ~}}
          {{~> type.link Package=../Package Assembly=../Assembly Type=. ~}}
        {{~/ join ~}}
        </small>
      {{~/ if ~}}
  </h3>
</div>

<p>{{ Type.Summary }}</p>

{{# ifEach Type.Constructors 'Constructors' }}
  {{> type.constructor Package=../Package Assembly=../Assembly Type=../Type Constructor=. }}
{{/ ifEach }}

{{# ifEach Type.Methods 'Methods' }}
  {{> type.method Package=../Package Assembly=../Assembly Type=../Type Method=. }}
{{/ ifEach }}
