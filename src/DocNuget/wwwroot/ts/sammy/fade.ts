import $ = require('jquery')
import Sammy = require('sammy')
import NProgress = require('nprogress')

NProgress.configure({ showSpinner: false })

var fade = (app: Sammy.Application) => {
  app.swap = function (content: any, callback: any) {
    NProgress.start()
    this.$element().fadeOut('fast', () => {
      NProgress.done()
      this.$element().html(content)
      this.$element().fadeIn('fast', () => {
        if (callback) {
          callback.apply()
        }
      })
    })
  }
}

export default fade
