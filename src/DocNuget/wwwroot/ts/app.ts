import Sammy = require('sammy')
import PushLocationProxy from './sammy/push_location_proxy'
import emblem from './sammy/emblem'
import fade from './sammy/fade'

var app = Sammy('#content', app => {
  app.debug = true
  app.use(emblem, 'em')
  app.use(fade)
  app.setLocationProxy(new PushLocationProxy(app, 'a', 'body'))

  app.get('/', (route, next) => route.partial('/views/index.em').then(next))
  app.get('/about', (route, next) => route.partial('/views/about.em').then(next))
  app.get('/packages/:package', (route, next) => route
    .load('/api/packages/' + route.params.package)
    .then(JSON.parse)
    .partial('/views/package.em')
    .then(next))
  app.get('/packages/:package/:version', (route, next) => route
    .load('/api/packages/' + route.params.package + '/' + route.params.version)
    .then(JSON.parse)
    .partial('/views/package.em')
    .then(next))
})

app.run()
