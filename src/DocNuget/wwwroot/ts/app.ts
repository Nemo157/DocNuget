import Sammy = require('sammy')
import PushLocationProxy from './sammy/push_location_proxy'
import Handlebars = require('sammy.handlebars')
import fade from './sammy/fade'

var app = Sammy('#content', app => {
  app.debug = true
  app.use(Handlebars, 'hb')
  app.use(fade)
  app.setLocationProxy(new PushLocationProxy(app, 'a', 'body'))

  app.get('/', (route, next) => route.partial('/views/index.hb').then(next))
  app.get('/about', (route, next) => route.partial('/views/about.hb').then(next))
  app.get('/packages/:package', (route, next) => route
    .load('/api/packages/' + route.params.package)
    .then(JSON.parse)
    .partial('/views/package.hb')
    .then(next))
  app.get('/packages/:package/:version', (route, next) => route
    .load('/api/packages/' + route.params.package + '/' + route.params.version)
    .then(JSON.parse)
    .partial('/views/package.hb')
    .then(next))

  app.get('/404', (route, next) => route.partial('/views/404.hb', route.params).then(next))

  var notFound = app.notFound
  app.notFound = (verb, path) => {
    if (path === '/404') {
      notFound.call(app, verb, path)
    } else {
      app.runRoute('get', '/404', { path })
    }
  }
})

app.run()
