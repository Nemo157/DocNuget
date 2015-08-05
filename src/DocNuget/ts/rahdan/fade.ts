import { Builder } from '../node_modules/rahdan/src/rahdan'
import * as NProgress from 'nprogress'

function fade(builder: Builder, { element }: { element: JQuery }) {
  NProgress.configure({ showSpinner: false })
  builder.around((context, next) =>
    context.historical
      ? next()
      : new Promise(resolve => element.fadeOut(50, resolve))
        .then(() => {
          var n = next()
          return Promise.race<void | string>([new Promise<string>(resolve => setTimeout(() => resolve('timeout'), 1)), n])
            .then<void>(res => res === 'timeout' ? (NProgress.start(), n.then<any>(() => NProgress.done())) : n)
        })
        .then(() => new Promise(resolve => element.fadeIn(50, resolve))))
}

export default fade
