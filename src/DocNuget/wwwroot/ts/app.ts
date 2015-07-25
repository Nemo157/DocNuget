import $ = require('jquery')
import bootstrap = require('bootstrap')
import Sammy = require('sammy')
import PushLocationProxy from './sammy/push_location_proxy'
import Handlebars = require('handlebars')
import SammyHandlebars = require('sammy.handlebars')
import fade from './sammy/fade'
import resolve from './resolve'

var a = $
var b = bootstrap

Handlebars.registerHelper('replace', (str: string, substr: string, newSubStr: string) => str.replace(substr, newSubStr))

var app = Sammy('#content', app => {
  app.debug = true
  app.use(SammyHandlebars, 'hb')
  app.use(fade)
  app.setLocationProxy(new PushLocationProxy(app, 'a', 'body'))

  var basicRoute = (partial: string) => (route: Sammy.EventContext, next: () => void) => route
    .partial('/views/' + partial + '.hb')
    .then(next)

  var apiRoute = (partial: string, partials: { [key: string]: string } = {}) => {
    var map: { [key: string]: string } = {}
    Object.keys(partials).forEach(key => map[key] = '/views/' + partials[key] + '.hb')
    return (route: Sammy.EventContext, next: () => void) => route
      .loadPartials(map)
      .load('/api' + route.path)
      .then(JSON.parse)
      .then(resolve)
      .partial('/views/' + partial + '.hb')
      .then(next)
  }

  app.get('/', basicRoute('index'))
  app.get('/about', basicRoute('about'))
  app.get('/packages/:package', apiRoute('package'))
  app.get('/packages/:package/:version', apiRoute('package'))
  app.get('/packages/:package/:version/assemblies/:assembly', apiRoute('assembly', { namespace: 'assembly/namespace' }))
  app.get('/packages/:package/:version/assemblies/:assembly/:framework', apiRoute('assembly'))

  app.get('/404', (route, next) => route.partial('/views/404.hb', route.params).then(next))

  var notFound = app.notFound
  app.notFound = (verb: string, path: string) => {
    if (path === '/404') {
      notFound.call(app, verb, path)
    } else {
      app.runRoute('get', '/404', { path })
    }
  }
})

app.run()
