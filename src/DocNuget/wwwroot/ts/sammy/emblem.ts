import $ = require('jquery')
import Emblem from 'emblem'
import Sammy = require('sammy')
import Handlebars = require('handlebars')

var emblem = (app: Sammy.Application, alias: string) => {
  var cache: {
    [name: string]: (data: any, partials: any) => any
  } = {}

  app.helper(alias || 'emblem', (template: string, data: any, partials: any, name?: string) => {
    if (typeof name == 'undefined')  {
      name = template
    }

    var fn = cache[name]
    if (!fn) {
      fn = cache[name] = Handlebars.compile(Emblem.compile(template))
    }

    data = $.extend({}, this, data)
    partials = $.extend({}, data.partials, partials)

    return fn(data, { partials })
  })
}

(<any>Sammy).Emblem = emblem

export default emblem
