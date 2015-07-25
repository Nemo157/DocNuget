<div class="page-header">
  <h1>{{ Title }} <small><code>{{ Version }}</code></small></h1>
</div>

<p>{{ Summary }}</p>

<h2>Assemblies</h2>
<ul>
  {{# each Assemblies as |Assembly| }}
    <li>
      <samp><a href="/packages/{{ ../Id }}/{{ ../Version }}/assemblies/{{ Assembly.Name }}">{{ Assembly.Name }}</a></samp>
      {{# each Frameworks as |Framework| }}
        <small><code><a href="/packages/{{ ../../Id }}/{{ ../../Version }}/assemblies/{{ Assembly.Name }}/{{ Framework }}">{{ Framework }}</a></code></small>
      {{/ each }}
    </li>
  {{/ each }}
</ul>

<h2>Versions</h2>
<ul>
  {{# each Versions as |Version| }}
    <li><a href="/packages/{{ ../Id }}/{{ Version }}">{{ Version }}</a></li>
  {{/ each }}
</ul>
