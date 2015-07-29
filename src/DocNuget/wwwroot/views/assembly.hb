<div class="page-header">
  <h1>{{ Assembly.Name }} <small><code>{{ Assembly.Framework }}</code></small></h1>
</div>

<dl class="dl-horizontal">
  <dt>Package</dt><dd>{{> package.link Package=Package }}</dd>
</dl>

<h2>Classes</h2>
<ul style="list-style: none">
  <li>{{> namespace Package=Package Assembly=Assembly Namespace=Assembly.RootNamespace }}</li>
</ul>

<h2>Frameworks</h2>
<ul>
  {{# each Frameworks as |Framework| }}
    <li>
      <small><code><a href="/packages/{{ ../Package.Id }}/{{ ../Package.Version }}/assemblies/{{ ../Assembly.Name }}/{{ Assembly.Framework }}">{{ Assembly.Framework }}</a></code></small>
    </li>
  {{/ each }}
</ul>
