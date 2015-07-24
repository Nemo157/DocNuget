import $ = require('jquery')
import Sammy = require('sammy')

class PushLocationProxy {
  public path: string

  constructor (public app: Sammy.Application, public selector: string = 'a', public element?: string) {
  }

  bind() {
    var proxy = this
    $(window).bind('popstate', () => this.app.trigger('location-changed'))
    this.$element().on('click', this.selector, function (e: Event) {
      if (location.hostname == this.hostname) {
        e.preventDefault()
        proxy.setLocation($(this).attr('href'))
        proxy.app.trigger('location-changed')
      }
    })
  }

  unbind() {
    this.$element().off('click', this.selector)
    $(window).unbind('popstate')
  }

  getLocation() {
    return window.location.pathname
  }

  setLocation(new_location: string) {
    history.pushState({ path: this.path }, '', new_location)
  }

  $element() {
    return this.element ? $(this.element) : this.app.$element()
  }
}

(<any>Sammy).PushLocationProxy = PushLocationProxy;

export default PushLocationProxy
