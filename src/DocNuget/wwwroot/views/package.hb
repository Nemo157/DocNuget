<div class="page-header">
  <h1>{{ Package.Title }} <small><code>{{ Package.Version }}</code></small></h1>
</div>

<p>{{ Package.Summary }}</p>

{{# if Package.Assemblies }}
  <h2>Assemblies</h2>
  <ul>
    {{# each Package.Assemblies as |Assembly| }}
      <li>
        <samp>{{> assembly.link Package=../Package Assembly=Assembly }}</samp>
        {{# each Assembly.TargetFrameworks as |TargetFramework| }}
          <small><code><a href="/packages/{{ ../../Package.Id }}/{{ ../../Package.Version }}/assemblies/{{ ../Assembly.Name }}/{{ TargetFramework }}">{{ TargetFramework }}</a></code></small>
        {{/ each }}
      </li>
    {{/ each }}
  </ul>
{{/ if }}

{{# if Package.DependencySets }}
  <h2>Dependencies</h2>
  <ul>
  {{# each Package.DependencySets as |DependencySet| }}
    <li>
      <b>{{ DependencySet.TargetFramework.FullName }}</b>
      <ul>
        {{# each DependencySet.Dependencies as |Dependency| }}
          <li>{{> package.link Package=Dependency }} <small><code>{{ Dependency.VersionSpec }}</code></small></li>
        {{/ each }}
      </ul>
    </li>
  {{/ each }}
  </ul>
{{/ if }}

<h2>Versions</h2>
<ul>
  {{# each Package.Versions as |Version| }}
    <li><a href="/packages/{{ ../Package.Id }}/{{ Version }}">{{ Version }}</a></li>
  {{/ each }}
</ul>
