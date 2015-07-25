import $ = require('jquery')
import Sammy = require('sammy')
import NProgress = require('nprogress')

NProgress.configure({ showSpinner: false })

var fade = (app: Sammy.Application) => {
  var fadeIn = false, fadeOut = false
  var next: () => void

  var before = () => {
    fadeOut = true
    NProgress.start()
    app.$element().fadeOut('fast', () => {
      fadeOut = false
      if (next) {
        next()
        next = null
      }
    })
  }

  var complete = (callback?: () => void) => {
    if (!fadeOut) {
      fadeIn = true
      NProgress.done()
      app.$element().fadeIn('fast', () => {
        fadeIn = false
        if (callback) {
          callback()
        }
      })
    }
  }

  var swap = (content: any, callback?: () => void) => {
    if (fadeOut) {
      next = () => {
        app.$element().html(content)
        complete(callback)
      }
    } else {
      app.$element().html(content)
    }
  }

  app.before(before)
  app.onComplete(() => complete())
  app.swap = swap
}

export default fade
