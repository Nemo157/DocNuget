div class="page-header"
  h1
    = Title
    small: code = Version

p = Summary

h2 Assemblies
ul
  each Assemblies as |Assembly|
    li
      samp: a href="/packages/#{ Id }/#{ Version }/#{ Assembly.Name }" = Assembly.Name
      each Frameworks as |Framework|
        small: code: a href="/packages/#{ Id }/#{ Version }/#{ Assembly.Name }/#{ Framework }" = Framework

h2 Versions
ul
  each Versions as |Version|
    li: a href="/packages/#{ Id }/#{ Version }" = Version
