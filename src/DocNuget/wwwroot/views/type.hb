<div class="page-header">
  <h1>
    {{~ Type.Accessibility }}
    {{> type.name Package=Package Assembly=Assembly Type=Type ~}}
    {{~# if Type.AllBaseTypes }}
      <small>:
      {{# join Type.AllBaseTypes ', ' ~}}
        {{~> type.link Package=../Package Assembly=../Assembly Type=. ~}}
      {{~/ join ~}}
      </small>
    {{~/ if ~}}
  </h1>
</div>

<p>{{ Type.Summary }}</p>

<dl class="dl-horizontal">
  <dt>Namespace</dt><dd>{{> namespace.link Package=Package Assembly=Assembly Namespace=Namespace }}</dd>
  <dt>Assembly</dt><dd>{{> assembly.link Package=Package Assembly=Assembly }}</dd>
</dl>

{{# if Type.Constructors }}
  <h2>Constructors</h2>
  {{# each Type.Constructors as |Constructor| }}
    {{> type.constructor Package=../Package Assembly=../Assembly Type=../Type Constructor=Constructor }}
  {{/ each }}
{{/ if }}

{{# if Type.Methods }}
  <h2>Methods</h2>
  {{# each Type.Methods as |Method| }}
    {{> type.method Package=../Package Assembly=../Assembly Type=../Type Method=Method }}
  {{/ each }}
{{/ if }}
