import * as rahdan from './node_modules/rahdan/src/rahdan'
import fade from './rahdan/fade'
import * as cache from './cache'

var content = $('#content')

var basicRoute = (view: string) => (route: rahdan.EventContext) => content.html(view)

var apiRoute = (view: (data: any) => string, getData: (route: rahdan.EventContext, pkg: any) => any = (route, pkg) => ({ Package: pkg })) => {
  return (route: rahdan.EventContext) =>
    cache.get(route.params['package'], route.params['version'])
      .then((pkg: any) => content.html(view(getData(route, pkg))))
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


//   var notFound = app.notFound
//   app.notFound = (verb: string, path: string) => {
//     if (path === '/404') {
//       notFound.call(app, verb, path)
//     } else {
//       app.runRoute('get', '/404', { path })
//     }
//   }

var app = rahdan.builder()
  .use(fade, { element: content })
  .around((context, next) => next().catch(err => {
    console.log('Error loading path', context.path, err)
    content.html(require<(data: any) => string>('./views/500.hbs')({ path: context.path, error: err }))
  }))
  .get('/', basicRoute(require<() => string>('./views/index.hbs')()))
  .get('/about', basicRoute(require<() => string>('./views/about.hbs')()))
  .get('/404', (route: rahdan.EventContext) => content.html(require<(data: any) => string>('./views/404.hbs')(route.params)))
  .get('/packages/:package', apiRoute(require<(data: any) => string>('./views/package.hbs')))
  .get('/packages/:package/:version', apiRoute(require<(data: any) => string>('./views/package.hbs')))
  .get('/packages/:package/:version/assemblies/:assembly',
    apiRoute(require<(data: any) => string>('./views/assembly.hbs'),
      (route, pkg) => ({ Package: pkg, Assembly: findAssembly(pkg, route.params['assembly']) })))
  .get('/packages/:package/:version/assemblies/:assembly/:framework',
    apiRoute(require<(data: any) => string>('./views/assembly.hbs'),
      (route, pkg) => ({ Package: pkg, Assembly: findAssembly(pkg, route.params['assembly'], route.params['framework']) })))
  .get('/packages/:package/:version/assemblies/:assembly/namespaces/:namespace',
    apiRoute(require<(data: any) => string>('./views/namespace.hbs'),
      (route, pkg) => {
        var assembly = findAssembly(pkg, route.params['assembly'])
        var namespace = findNamespace(assembly, route.params['namespace'])
        return {
          Package: pkg,
          Assembly: assembly,
          Namespace: namespace,
        }
      }))
  .get('/packages/:package/:version/assemblies/:assembly/:framework/namespaces/:namespace',
    apiRoute(require<(data: any) => string>('./views/namespace.hbs'),
      (route, pkg) => {
        var assembly = findAssembly(pkg, route.params['assembly'], route.params['framework'])
        var namespace = findNamespace(assembly, route.params['namespace'])
        return {
          Package: pkg,
          Assembly: assembly,
          Namespace: namespace,
        }
      }))
  .get('/packages/:package/:version/assemblies/:assembly/types/:type',
    apiRoute(require<(data: any) => string>('./views/type.hbs'),
      (route, pkg) => {
        var assembly = findAssembly(pkg, route.params['assembly'])
        var { namespace, type } = findType(assembly, route.params['type'])
        return {
          Package: pkg,
          Assembly: assembly,
          Type: type,
          Namespace: namespace,
        }
      }))
  .get('/packages/:package/:version/assemblies/:assembly/:framework/types/:type',
    apiRoute(require<(data: any) => string>('./views/type.hbs'),
      (route, pkg) => {
        var assembly = findAssembly(pkg, route.params['assembly'], route.params['framework'])
        var { namespace, type } = findType(assembly, route.params['type'])
        return {
          Package: pkg,
          Assembly: assembly,
          Type: type,
          Namespace: namespace,
        }
      }))
  .run()

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
