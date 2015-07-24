import Sammy = require('sammy')
import PushLocationProxy from './sammy/push_location_proxy'
import emblem from './sammy/emblem'
import fade from './sammy/fade'

var app = Sammy('#content', app => {
  app.use(emblem, 'em')
  app.use(fade)
  app.setLocationProxy(new PushLocationProxy(app, 'a', 'body'))

  app.get('/', route => route.partial('/views/index.em'))
  app.get('/about', route => route.partial('/views/about.em'))
  app.get('/packages/:package', route => route
    .load('/api/packages/' + route.params.package + '.json')
    .partial('/views/package.em'))
  app.get('/packages/:package/:version', route => route
    .load('/api/packages/' + route.params.package + '/' + route.params.version + '.json')
    .partial('/views/package.em'))
})

app.run()
