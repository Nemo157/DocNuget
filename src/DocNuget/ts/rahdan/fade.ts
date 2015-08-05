import { Builder } from '../node_modules/rahdan/src/rahdan'
import * as NProgress from 'nprogress'

function fade(builder: Builder, { element }: { element: JQuery }) {
  NProgress.configure({ showSpinner: false })
  builder.around((context, next) =>
    context.historical
      ? next()
      : new Promise(resolve => (NProgress.start(), element.fadeOut('fast', resolve)))
        .then(next)
        .then(() => new Promise(resolve => (NProgress.done(), element.fadeIn('fast', resolve)))))
}

export default fade
