import * as Handlebars from 'handlebars'
import { settings, Accessibility } from '../../settings'

function accessibilityDebug(item: { Accessibility: string }) {
  if (settings.accessibilityDebug) {
    if ((settings.accessibility & Accessibility[item.Accessibility]) === Accessibility.none) {
      return new Handlebars.SafeString('<i class="fa fa-exclamation-triangle text-warning" title="Would be hidden by accessibility option"></i>')
    }
  }
}

export default accessibilityDebug
