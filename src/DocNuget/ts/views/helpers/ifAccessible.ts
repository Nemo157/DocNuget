import { settings, Accessibility } from '../../settings'

function ifAccessible(item: { Accessibility: string }, options: any) {
  if ((settings.accessibility & Accessibility[item.Accessibility]) === Accessibility.none) {
    if (settings.accessibilityDebug) {
      return options.fn(this)
    } else {
      return options.inverse(this)
    }
  } else {
    return options.fn(this)
  }
}

export default ifAccessible
