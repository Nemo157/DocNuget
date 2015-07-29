declare module 'sammy' {
  function Sammy(): Sammy.Application
  function Sammy(element: string): Sammy.Application
  function Sammy(init: (app: Sammy.Application) => void): Sammy.Application
  function Sammy(element: string, init: (app: Sammy.Application) => void): Sammy.Application

  module Sammy {
    class Application {
      debug: boolean
      constructor()
      constructor(init: (app: Sammy.Application) => void)
      setLocationProxy(proxy: any): void
      get(path: string, callback: (route: Sammy.EventContext, next?: () => void) => void): void
      getLocation(): string
      setLocation(loc: string): void
      trigger(ev: string): void
      run(): void
      runRoute(verb: string, path: string, params: any): void
      swap(content: any, callback?: any): void
      templateCache(key: string): string
      templateCache(key: string, value: string): void
      use(plugin: (app: Sammy.Application) => void): void
      use<T>(plugin: (app: Sammy.Application, param: T) => void, param: T): void
      helper(name: string, func: Function): void
      $element(): any
      before(callback: (route: Sammy.EventContext) => void): void
      onComplete(callback: (route: Sammy.EventContext) => void): void
      notFound(verb: string, path: string): void
    }

    class EventContext {
      load(location: string): RenderContext
      loadPartials(partials: { [key: string]: string }): RenderContext
      partial(location: string, data?: any, callback?: () => void, partials?: { [key: string]: string }): RenderContext
      params: any
      path: string
    }

    class RenderContext {
      then(callback: (data: any) => any): RenderContext
      load(location: string): RenderContext
      loadPartials(partials: { [key: string]: string }): RenderContext
      partial(location: string, data?: any, callback?: () => void, partials?: { [key: string]: string }): RenderContext
    }
  }
  export = Sammy
}

declare module 'sammy.handlebars' {
  import Sammy = require('sammy')
  var a: (app: Sammy.Application, alias: string) => void
  export = a
}

declare module 'raw!./views/index' {
  var a: string
  export = a
}
declare module 'raw!./views/about' {
  var a: string
  export = a
}

declare module 'emblem' {
  module Emblem {
    function compile(template: string): string
  }
  export default Emblem
}

declare module 'handlebars' {
  export function registerHelper(name: string, helper: Function): void
  export class SafeString {
    constructor(value: string)
  }
}
