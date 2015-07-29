<div class="page-header">
  <h6><small>package</small> {{> package.link Package=Package }}</h6>
  <h5><small>assembly</small> {{> assembly.link Package=Package Assembly=Assembly }}</h5>
  <h4><small>namespace</small> {{> namespace.link Package=Package Assembly=Assembly Namespace=Namespace }}</h4>
  <h3>
    <small>
      {{ Type.Accessibility }}
      {{ Type.Construct }}
    </small>
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
