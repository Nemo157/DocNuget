import fetch from 'fetch'

var cache: {
  [pkg: string]: {
    [version: string]: any
  }
} = {}

export var get = (pkg: string, version: string) => {
  if (!cache[pkg]) {
    cache[pkg] = {}
  }
  if (cache[pkg][version]) {
    return Promise.resolve(cache[pkg][version])
  } else {
    return fetch('/api/packages/' + pkg + (version ? '/' + version : ''))
      .then(res => res.json())
      .then(json => cache[pkg][version] = json)
  }
}
