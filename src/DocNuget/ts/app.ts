import $ = require('jquery')
import bootstrap = require('bootstrap')
import Sammy = require('sammy')
import PushLocationProxy from './sammy/push_location_proxy'
import Handlebars = require('handlebars')
import SammyHandlebars = require('sammy.handlebars')
import fade from './sammy/fade'

var a = $
var b = bootstrap

var Accessibility: {
  [key: string]: number
  none: number
  public: number
  protected: number
  internal: number
  private: number
  all: number
} = {
  none: 0,
  public: 1,
  protected: 2,
  internal: 4,
  private: 8,
  all: 15,
}

Accessibility['<unknown>'] = Accessibility.all
Accessibility['protected internal'] = 6

var settings = {
  accessibility: Accessibility.public | Accessibility.protected,
  accessibilityDebug: false,
}

Handlebars.registerHelper('replace', (str: string, substr: string, newSubStr: string) => str && str.replace(new RegExp(substr, 'g'), newSubStr))
Handlebars.registerHelper('join', (context: any[], sep: string, options: any) => (context || []).map(item => options.fn(item).trim()).join(sep))
Handlebars.registerHelper('ifAccessible', function (item: { Accessibility: string }, options: any) {
  if ((settings.accessibility & Accessibility[item.Accessibility]) === Accessibility.none) {
    if (settings.accessibilityDebug) {
      return options.fn(this)
    } else {
      return options.inverse(this)
    }
  } else {
    return options.fn(this)
  }
})
Handlebars.registerHelper('accessibilityDebug', function (item: { Accessibility: string }) {
  if (settings.accessibilityDebug) {
    if ((settings.accessibility & Accessibility[item.Accessibility]) === Accessibility.none) {
      return new Handlebars.SafeString('<i class="fa fa-exclamation-triangle text-warning" title="Would be hidden by accessibility option"></i>')
    }
  }
})

Handlebars.registerHelper('ifEach', (items: any[], title: string, options: any) => items ? '<h4>' + title + '</h4>' + items.map(options.fn).join('\n') : options.inverse(this))

var app = Sammy('#content', app => {
  app.debug = true
  app.use(SammyHandlebars, 'hb')
  app.use(fade)
  app.setLocationProxy(new PushLocationProxy(app, 'a', 'body'))

  var partials : { [key: string]: string } = {
    'type.name': '/views/type/name.hb',
    'type.link': '/views/type/link.hb',
    'type.method': '/views/type/method.hb',
    'type.constructor': '/views/type/constructor.hb',
    'assembly.link': '/views/assembly/link.hb',
    'namespace.link': '/views/namespace/link.hb',
    'namespace.tree': '/views/namespace/tree.hb',
    'namespace.tree-node': '/views/namespace/tree-node.hb',
    'package.link': '/views/package/link.hb',
  }

  var basicRoute = (partial: string) => (route: Sammy.EventContext, next: () => void) => route
    .partial('/views/' + partial + '.hb')
    .then(next)

  var apiRoute = (partial: string, getData: (route: Sammy.EventContext, pkg: any) => any = (route, pkg) => ({ Package: pkg })) => {
    return (route: Sammy.EventContext, next: () => void) => route
      .loadPartials(partials)
      .load('/api/packages/' + route.params.package + (route.params.version ? '/' + route.params.version : ''))
      .then(JSON.parse)
      .then(pkg => getData(route, pkg))
      .partial('/views/' + partial + '.hb')
      .then(next)
  }

  var findAssembly = (pkg: any, name: string, framework?: string) => pkg
    .Assemblies
    .find((assembly: any) => assembly.Name === name && (!framework || (assembly.TargetFramework && assembly.TargetFramework.FullName === framework)))

  var allTypes = (namespace: any) => namespace.Types.map((type: any) => ({ namespace, type }))
    .concat(...namespace.Namespaces.map(allTypes))

  var allNamespaces = (namespace: any) => namespace.Namespaces
    .concat(...namespace.Namespaces.map(allNamespaces))

  var findType = (assembly: any, name: string) => allTypes(assembly.RootNamespace)
    .find((pair: any) => pair.type.FullName === name)

  var findNamespace = (assembly: any, name: string) => allNamespaces(assembly.RootNamespace)
    .find((namespace: any) => namespace.FullName === name)

  app.get('/', basicRoute('index'))
  app.get('/about', basicRoute('about'))
  app.get('/packages/:package', apiRoute('package'))
  app.get('/packages/:package/:version', apiRoute('package'))

  app.get('/packages/:package/:version/assemblies/:assembly',
    apiRoute('assembly',
      (route, pkg) => ({ Package: pkg, Assembly: findAssembly(pkg, route.params.assembly) })))

  app.get('/packages/:package/:version/assemblies/:assembly/:framework',
    apiRoute('assembly',
      (route, pkg) => ({ Package: pkg, Assembly: findAssembly(pkg, route.params.assembly, route.params.framework) })))

  app.get('/packages/:package/:version/assemblies/:assembly/namespaces/:namespace',
    apiRoute('namespace',
      (route, pkg) => {
        var assembly = findAssembly(pkg, route.params.assembly)
        var namespace = findNamespace(assembly, route.params.namespace)
        return {
          Package: pkg,
          Assembly: assembly,
          Namespace: namespace,
        }
      }))

  app.get('/packages/:package/:version/assemblies/:assembly/:framework/namespaces/:namespace',
    apiRoute('namespace',
      (route, pkg) => {
        var assembly = findAssembly(pkg, route.params.assembly, route.params.framework)
        var namespace = findNamespace(assembly, route.params.namespace)
        return {
          Package: pkg,
          Assembly: assembly,
          Namespace: namespace,
        }
      }))

  app.get('/packages/:package/:version/assemblies/:assembly/types/:type',
    apiRoute('type',
      (route, pkg) => {
        var assembly = findAssembly(pkg, route.params.assembly)
        var { namespace, type } = findType(assembly, route.params.type)
        return {
          Package: pkg,
          Assembly: assembly,
          Type: type,
          Namespace: namespace,
        }
      }))

  app.get('/packages/:package/:version/assemblies/:assembly/:framework/types/:type',
    apiRoute('type',
      (route, pkg) => {
        var assembly = findAssembly(pkg, route.params.assembly, route.params.framework)
        var { namespace, type } = findType(assembly, route.params.type)
        return {
          Package: pkg,
          Assembly: assembly,
          Type: type,
          Namespace: namespace,
        }
      }))

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

interface BootswatchTheme {
  name: string
  description: string
  cssCdn: string
}

interface BootswatchThemes {
  version: string
  themes: BootswatchTheme[]
}

$(() => {
  $.get('//bootswatch.aws.af.cm/3/', (data: BootswatchThemes) => {
    var original = $('#bootstrap').attr('href')
    var originalLi = $('#theme-selector ul li')
    $('#theme-selector ul li a').click(() => {
      $('#theme-selector ul li').removeClass('active')
      originalLi.addClass('active')
      $('#bootstrap').attr('href', original)
    })
    $('#theme-selector ul')
      .append(
        data.themes.map((value, index) => {
          var li = $('<li />')
            .append(
              $('<a />')
                .attr('href', '#')
                .append(
                  $('<b />').text(value.name),
                  ' ',
                  $('<small />').text(value.description))
                .click(() => {
                  $('#theme-selector ul li').removeClass('active')
                  li.addClass('active')
                  $('#bootstrap').attr('href', value.cssCdn)
                }))
            return li
        }))
  }, 'json')
})
