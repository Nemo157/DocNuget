<div class="page-header">
  <h1>{{ Title }} <small><code>{{ Version }}</code></small></h1>
</div>

<p>{{ Summary }}</p>

{{# if Assemblies }}
  <h2>Assemblies</h2>
  <ul>
    {{# each Assemblies as |Assembly| }}
      <li>
        <samp>{{> assembly.link Assembly }}</samp>
        {{# each TargetFrameworks as |TargetFramework| }}
          <small><code><a href="/packages/{{ ../../Id }}/{{ ../../Version }}/assemblies/{{ Assembly.Name }}/{{ TargetFramework }}">{{ TargetFramework }}</a></code></small>
        {{/ each }}
      </li>
    {{/ each }}
  </ul>
{{/ if }}

{{# if DependencySets }}
  <h2>Dependencies</h2>
  <ul>
  {{# each DependencySets as |DependencySet| }}
    <li>
      <b>{{ DependencySet.TargetFramework.FullName }}</b>
      <ul>
        {{# each Dependencies as |Dependency| }}
          <li>{{> package.link Dependency }} <small><code>{{ Dependency.VersionSpec }}</code></small></li>
        {{/ each }}
      </ul>
    </li>
  {{/ each }}
  </ul>
{{/ if }}

<h2>Versions</h2>
<ul>
  {{# each Versions as |Version| }}
    <li><a href="/packages/{{ ../Id }}/{{ Version }}">{{ Version }}</a></li>
  {{/ each }}
</ul>
