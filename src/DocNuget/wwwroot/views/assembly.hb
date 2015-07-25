<div class="page-header">
  <h1>{{ Name }} <small><code>{{ Framework }}</code></small></h1>
</div>

<h2>Classes</h2>
<ul style="list-style: none">
  <li>{{> namespace RootNamespace }}</li>
</ul>

<h2>Frameworks</h2>
<ul>
  {{# each Frameworks as |Framework| }}
    <li>
      <small><code><a href="/packages/{{ ../Package.Id }}/{{ ../Package.Version }}/assemblies/{{ ../Name }}/{{ Framework }}">{{ Framework }}</a></code></small>
    </li>
  {{/ each }}
</ul>
