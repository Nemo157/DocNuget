import rest from 'rest'
import mime from 'rest/interceptor/mime'
import errorCode from 'rest/interceptor/errorCode'
import pathPrefix from 'rest/interceptor/pathPrefix'
import * as when from 'when'

var cache: {
  [pkg: string]: {
    [version: string]: any
  }
} = {}

var apiClient = rest.wrap(mime).wrap(errorCode).wrap(pathPrefix, { prefix: '/api' })

export var get = (pkg: string, version: string) =>
  cache[pkg] && cache[pkg][version]
    ? when.resolve(cache[pkg][version])
    : apiClient('packages/' + pkg + (version ? '/' + version : '')).entity()
