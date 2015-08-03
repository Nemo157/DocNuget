import * as $ from 'jquery'
import { Builder } from '../node_modules/rahdan/src/rahdan'
import * as NProgress from 'nprogress'

function fade(builder: Builder, { element }: { element: JQuery }) {
  NProgress.configure({ showSpinner: false })
  builder
    .before(() => new Promise(resolve => {
      NProgress.start()
      element.fadeOut('fast', resolve)
    }))
    .after(() => new Promise(resolve => {
      NProgress.done()
      element.fadeIn('fast', resolve)
    }))
    .error(() => new Promise(resolve => {
      NProgress.done()
      element.fadeIn('fast', resolve)
    }))
}

export default fade
