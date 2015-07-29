<div class="page-header">
  <h5><small>package</small> {{> package.link Package=Package }}</h5>
  <h4><small>assembly</small> {{> assembly.link Package=Package Assembly=Assembly }}</h4>
  <h3><small>namespace</small> {{ Namespace.FullName }}</h3>
</div>

<h4>Classes</h4>
<ul style="list-style: none">
  {{> namespace.tree Package=Package Assembly=Assembly Namespace=Namespace }}
</ul>
