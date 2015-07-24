declare module 'sammy' {
  function Sammy(): Sammy.Application
  function Sammy(element: string): Sammy.Application
  function Sammy(init: (app: Sammy.Application) => void): Sammy.Application
  function Sammy(element: string, init: (app: Sammy.Application) => void): Sammy.Application

  module Sammy {
    class Application {
      constructor()
      constructor(init: (app: Sammy.Application) => void)
      setLocationProxy(proxy: any): void
      get(path: string, callback: (route: Sammy.EventContext) => void): void
      getLocation(): string
      setLocation(loc: string): void
      trigger(ev: string): void
      run(): void
      swap(content: any, callback?: any): void
      templateCache(key: string): string
      templateCache(key: string, value: string): void
      use(plugin: (app: Sammy.Application) => void): void
      use<T>(plugin: (app: Sammy.Application, param: T) => void, param: T): void
      helper(name: string, func: Function): void
      $element(): any
    }

    class EventContext {
      load(location: string): any
      partial(location: string, data?: any, callback?: any, partials?: any): any
      params: any
    }
  }
  export = Sammy
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
  export function compile(template: string): (data: any, partials: any) => any
}
